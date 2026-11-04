#:property TargetFramework=net10.0
#:property PublishAot=false
#:property EnforceCodeStyleInBuild=true
#:property GenerateDocumentationFile=true

using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

// Run from any directory: dotnet run --file tools/KeycloakTestEnvironment.cs -- up|down [options]
return await EnvironmentTool.RunAsync(args);

internal static class EnvironmentTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static readonly string Repository =
        Path.GetFullPath(Path.Combine(Path.GetDirectoryName(SourcePath())!, ".."));
    private const string Version = "26.8.1";

    private static string SourcePath([CallerFilePath] string path = "") => path;

    internal static async Task<int> RunAsync(string[] args)
    {
        try
        {
            if ( args.Length == 0 || args[0] is "--help" or "-h" )
            {
                Console.WriteLine("up|down [--project netcore-keycloak-tests] [--fgap v1|v2] " +
                    "[--port 8080] [--invalid-port 8899] [--config path]");
                return 0;
            }

            if ( args[0] is not ("up" or "down") ) throw new ArgumentException("Expected up or down.");
            var options = new Dictionary<string, string>(StringComparer.Ordinal);
            var allowed = new[] { "--project", "--fgap", "--port", "--invalid-port", "--config" };
            for ( var i = 1; i < args.Length; i += 2 )
            {
                if ( i + 1 >= args.Length || !allowed.Contains(args[i]) || !options.TryAdd(args[i], args[i + 1]) )
                    throw new ArgumentException("Invalid or duplicate option. Use --help.");
            }

            var project = options.GetValueOrDefault("--project", "netcore-keycloak-tests");
            if ( !Regex.IsMatch(project, "^netcore-keycloak-[a-z0-9][a-z0-9-]{0,40}$") )
                throw new ArgumentException("Project must start with netcore-keycloak- " +
                    "and contain lowercase letters, digits or hyphens.");
            var state = Path.Combine(Repository, "NETCore.Keycloak.Client.Tests", "obj", "native-environment", project);
            var compose = Path.Combine(state, "compose.json");
            var marker = Path.Combine(state, "owner.json");

            if ( args[0] == "down" )
            {
                if ( !File.Exists(marker) )
                {
                    Console.WriteLine("No managed environment found; nothing to stop.");
                    return 0;
                }

                var owner = JsonNode.Parse(await File.ReadAllTextAsync(marker))!;
                if ( ( string? ) owner["project"] != project || ( string? ) owner["repository"] != Repository )
                    throw new InvalidOperationException("Environment ownership does not match this checkout.");
                await DockerAsync("compose", "--project-name", project, "--file", compose,
                    "down", "--volumes", "--remove-orphans");
                // Delete only the generated files for this exact managed project.
                foreach ( var file in new[] { compose, marker, Path.Combine(state, "policies.jar") } )
                    File.Delete(file);
                Console.WriteLine($"Stopped managed environment {project}.");
                return 0;
            }

            if ( File.Exists(marker) )
                throw new InvalidOperationException("This environment already exists. " +
                    "Run down for the same project before recreating it.");
            var fgap = options.GetValueOrDefault("--fgap",
                Environment.GetEnvironmentVariable("KC_FGAP_VERSION") ?? "v1");
            if ( fgap is not ("v1" or "v2") ) throw new ArgumentException("FGAP must be v1 or v2.");
            var port = Port(options.GetValueOrDefault("--port", "8080"));
            var invalidPort = Port(options.GetValueOrDefault("--invalid-port", "8899"));
            if ( port == invalidPort ) throw new ArgumentException("Service ports must differ.");
            var defaultConfig = Path.Combine(Repository, "NETCore.Keycloak.Client.Tests",
                "Assets", "testing_environment.json");
            var config = Path.GetFullPath(options.GetValueOrDefault("--config", defaultConfig));
            _ = Directory.CreateDirectory(state);
            RestrictDirectory(state);
            var fixtures = Path.Combine(Repository, "tools", "test-environment");
            var jar = Path.Combine(state, "policies.jar");
            File.Delete(jar);
            ZipFile.CreateFromDirectory(Path.Combine(fixtures, "js-policies"), jar);
            // Docker's non-root Keycloak user must be able to read the provider JAR.
            var password = Secret();
            var adminPassword = Secret();
            var userPassword = Secret();
            var clientSecret = Secret();
            var baseUrl = $"http://localhost:{port}";
            var services = new
            {
                db = new
                {
                    image = "postgres:16",
                    environment = new
                    {
                        POSTGRES_DB = "keycloak",
                        POSTGRES_USER = "keycloak",
                        POSTGRES_PASSWORD = password
                    },
                    volumes = new[] { "database:/var/lib/postgresql/data" },
                    healthcheck = new
                    {
                        test = new[] { "CMD-SHELL", "pg_isready -U keycloak -d keycloak" },
                        interval = "2s",
                        timeout = "5s",
                        retries = 60
                    }
                },
                keycloak = new
                {
                    image = $"quay.io/keycloak/keycloak:{Version}",
                    command = new[] { "start-dev" },
                    environment = new
                    {
                        KC_DB = "postgres",
                        KC_DB_USERNAME = "keycloak",
                        KC_DB_PASSWORD = password,
                        KC_DB_URL = "jdbc:postgresql://db:5432/keycloak",
                        KC_HEALTH_ENABLED = "true",
                        KC_METRICS_ENABLED = "true",
                        KC_BOOTSTRAP_ADMIN_USERNAME = "admin",
                        KC_BOOTSTRAP_ADMIN_PASSWORD = adminPassword,
                        KC_FEATURES = $"scripts,client-policies,admin-fine-grained-authz:{fgap},organization"
                    },
                    ports = new[] { $"127.0.0.1:{port}:8080" },
                    volumes = new[] { $"{DockerPath(jar)}:/opt/keycloak/providers/policies.jar:ro" },
                    depends_on = new { db = new { condition = "service_healthy" } }
                },
                invalid_response = new
                {
                    image = "nginx:stable-alpine",
                    ports = new[] { $"127.0.0.1:{invalidPort}:80" },
                    volumes = new[]
                    {
                        $"{DockerPath(Path.Combine(fixtures, "nginx.default.conf"))}" +
                        ":/etc/nginx/conf.d/default.conf:ro"
                    }
                }
            };
            await WritePrivateAsync(compose, new { services, volumes = new { database = new { } } });
            await WritePrivateAsync(marker, new { project, repository = Repository });
            await DockerAsync("compose", "--project-name", project, "--file", compose,
                "up", "--detach", "--wait", "--wait-timeout", "180");
            using var http = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(30) };
            await WaitAsync(http);
            using ( var response = await http.PostAsync("/realms/master/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "password",
                    ["client_id"] = "admin-cli",
                    ["username"] = "admin",
                    ["password"] = adminPassword
                })) )
            {
                if ( !response.IsSuccessStatusCode )
                    throw new InvalidOperationException(
                        $"Bootstrap authentication failed: {( int ) response.StatusCode}.");
                var token = await response.Content.ReadFromJsonAsync<JsonObject>();
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ( string ) token!["access_token"]!);
            }

            var fixture = JsonNode.Parse(await File.ReadAllTextAsync(Path.Combine(fixtures, "realm-fixture.json")))!;
            var realm = ( string ) fixture["realm"]!["realm"]!;
            var api = $"/admin/realms/{Uri.EscapeDataString(realm)}";
            _ = await RequestAsync(http, HttpMethod.Post, "/admin/realms", fixture["realm"]);
            _ = await RequestAsync(http, HttpMethod.Put, api + "/users/profile", fixture["userProfile"]);
            foreach ( var role in fixture["realmRoles"]!.AsArray() )
                _ = await RequestAsync(http, HttpMethod.Post, api + "/roles",
                    new JsonObject { ["name"] = ( string ) role! });
            foreach ( var scope in fixture["clientScopes"]!.AsArray() )
                _ = await RequestAsync(http, HttpMethod.Post, api + "/client-scopes", scope);
            foreach ( var client in fixture["clients"]!.AsArray() )
            {
                if ( ( string? ) client!["clientId"] == "auth_client" ) client["secret"] = clientSecret;
                _ = await RequestAsync(http, HttpMethod.Post, api + "/clients", client);
            }

            var clients = (await RequestAsync(http, HttpMethod.Get, api + "/clients"))!.AsArray();
            var authId = ( string ) clients.Single(c => ( string? ) c!["clientId"] == "auth_client")!["id"]!;
            var authz = api + $"/clients/{authId}/authz/resource-server";
            var defaults = new[]
            {
                ("permission", "Default Permission"), ("policy", "Default Policy"), ("resource", "Default Resource")
            };
            foreach ( var (endpoint, name) in defaults )
            {
                var items = (await RequestAsync(http, HttpMethod.Get, authz + "/" + endpoint))!.AsArray();
                foreach ( var item in items.Where(item => ( string? ) item!["name"] == name) )
                    _ = await RequestAsync(http, HttpMethod.Delete,
                        authz + "/" + endpoint + "/" + ( string? ) (item!["id"] ?? item["_id"]));
            }

            var username = realm + "_admin";
            _ = await RequestAsync(http, HttpMethod.Post, api + "/users", new JsonObject
            {
                ["username"] = username,
                ["email"] = "admin@example.com",
                ["firstName"] = "John",
                ["lastName"] = "Doe",
                ["enabled"] = true,
                ["credentials"] = new JsonArray(new JsonObject
                {
                    ["type"] = "password",
                    ["value"] = userPassword,
                    ["temporary"] = false
                })
            });
            var users = (await RequestAsync(http, HttpMethod.Get,
                api + "/users?exact=true&username=" + Uri.EscapeDataString(username)))!.AsArray();
            var userId = ( string ) users.Single()!["id"]!;
            var managementId = ( string ) clients.Single(c => ( string? ) c!["clientId"] == "realm-management")!["id"]!;
            var managementRoles = (await RequestAsync(http, HttpMethod.Get,
                api + $"/clients/{managementId}/roles"))!.AsArray();
            var adminRoles = fixture["adminRoles"]!.AsArray()
                .Select(r => ( string ) r!).ToHashSet(StringComparer.Ordinal);
            var assignments = new JsonArray(managementRoles
                .Where(r => adminRoles.Contains(( string ) r!["name"]!)).Select(r => r!.DeepClone()).ToArray());
            if ( assignments.Count != adminRoles.Count )
                throw new InvalidOperationException("Missing realm-management roles.");
            _ = await RequestAsync(http, HttpMethod.Post,
                api + $"/users/{userId}/role-mappings/clients/{managementId}", assignments);
            await WritePrivateAsync(config, new
            {
                kc_version = Version,
                baseUrl,
                invalidBaseUrl = $"http://localhost:{invalidPort}",
                auth_realm = "master",
                client_id = "admin-cli",
                admin = new { username = "admin", password = adminPassword },
                testing_realm = new
                {
                    name = realm,
                    public_client = new { client_id = "app_client" },
                    private_client = new { client_id = "auth_client", client_secret = clientSecret },
                    user = new
                    {
                        username,
                        password = userPassword,
                        permissions = new[] { new { name = "auth_client", scopes = new[] { "Profile#Get" } } }
                    }
                }
            });
            Console.WriteLine($"Prepared Keycloak {Version}, FGAP {fgap}, project {project}. Configuration: {config}");
            return 0;
        }
        catch ( Exception exception )
        {
            // Do not log HTTP response bodies or generated secrets.
            Console.Error.WriteLine(exception.Message);
            Console.Error.WriteLine("If provisioning failed after startup, " +
                "run down with the same --project before retrying.");
            return 1;
        }
    }

    private static int Port(string value) => int.TryParse(value, out var port) && port is >= 1024 and <= 65535
        ? port : throw new ArgumentException("Ports must be between 1024 and 65535.");

    private static string Secret() => Convert.ToHexString(RandomNumberGenerator.GetBytes(24)) + "@aA1";
    private static string DockerPath(string path) => path.Replace('\\', '/');

    private static void RestrictDirectory(string path)
    {
        if ( !OperatingSystem.IsWindows() )
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
    }

    private static async Task WritePrivateAsync(string path, object value)
    {
        _ = Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var options = new FileStreamOptions { Mode = FileMode.Create, Access = FileAccess.Write };
        if ( !OperatingSystem.IsWindows() ) options.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
        await using var stream = new FileStream(path, options);
        await JsonSerializer.SerializeAsync(stream, value, JsonOptions);
        if ( !OperatingSystem.IsWindows() ) File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite);
    }

    private static async Task<JsonNode?> RequestAsync(
        HttpClient http, HttpMethod method, string path, JsonNode? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        if ( body is not null ) request.Content = JsonContent.Create(body);
        using var response = await http.SendAsync(request);
        if ( !response.IsSuccessStatusCode )
            throw new InvalidOperationException(
                $"Fixture request {method} {path} failed: {( int ) response.StatusCode}.");
        var content = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(content) ? null : JsonNode.Parse(content);
    }

    private static async Task WaitAsync(HttpClient http)
    {
        var deadline = DateTime.UtcNow.AddMinutes(4);
        while ( DateTime.UtcNow < deadline )
        {
            try
            {
                using var response = await http.GetAsync("/realms/master/.well-known/openid-configuration");
                if ( response.IsSuccessStatusCode ) return;
            }
            catch ( HttpRequestException ) { }
            catch ( TaskCanceledException ) { }
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new TimeoutException("Keycloak did not become ready within four minutes.");
    }

    private static async Task DockerAsync(params string[] arguments)
    {
        var start = new ProcessStartInfo("docker") { UseShellExecute = false };
        foreach ( var argument in arguments ) start.ArgumentList.Add(argument);
        using var process = Process.Start(start) ?? throw new InvalidOperationException("Cannot start Docker.");
        await process.WaitForExitAsync();
        if ( process.ExitCode != 0 )
            throw new InvalidOperationException($"Docker Compose failed with exit code {process.ExitCode}.");
    }
}
