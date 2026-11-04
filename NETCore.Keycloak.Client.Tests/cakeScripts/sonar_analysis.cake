/// <summary>
/// Initiates SonarCloud analysis by executing the `dotnet sonarscanner begin` command.
/// This task resolves the `dotnet` executable, ensures the required Sonar token is provided, 
/// and configures the necessary SonarCloud parameters for project analysis.
/// </summary>
Task("SonarBegin")
    .Does(() =>
    {
        // Retrieve the Sonar token from the environment variable
        var sonarToken = Environment.GetEnvironmentVariable("KC_SONAR_TOKEN") ?? "";

        if (string.IsNullOrWhiteSpace(sonarToken))
        {
            // Ensure the token is provided before proceeding
            throw new Exception("Sonar token is required.");
        }

        // Configure process settings for executing the SonarScanner command
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("sonarscanner begin")
                .AppendSecret($"/d:sonar.token={sonarToken}")
                .Append("/k:Black-Cockpit_NETCore.Keycloak")
                .Append("/o:black-cockpit")
                .Append("/d:sonar.host.url=\"https://sonarcloud.io\"")
                .Append("/d:sonar.coverage.exclusions=\"**/NETCore.Keycloak.Client.Tests/**/*.*\"")
                .Append("/d:sonar.test.exclusions=\"**/NETCore.Keycloak.Client.Tests/**/*.*\"")
                .Append("/d:sonar.exclusions=\"**/NETCore.Keycloak.Client.Tests/**/*.*\"")
                .Append("/d:sonar.cs.dotcover.reportsPaths=dotCover.Output.xml"),
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Execute the SonarScanner command
        var result = StartProcess("dotnet", processSettings, out var output, out var error);
        if (result != 0)
        {
            throw new Exception("Sonar analysis initialization failed. Error:\n" + string.Join(Environment.NewLine, error));
        }
    });

/// <summary>
/// Finalizes the SonarCloud analysis by executing the `dotnet sonarscanner end` command.
/// This task ensures the required Sonar token is provided, resolves the `dotnet` executable, 
/// and captures the process output and error streams for evaluation.
/// </summary>
Task("SonarEnd")
    .IsDependentOn("Build")
    .Does(() =>
    {
        // Retrieve the Sonar token from the environment variable
        var sonarToken = Environment.GetEnvironmentVariable("KC_SONAR_TOKEN") ?? "";

        if (string.IsNullOrWhiteSpace(sonarToken))
        {
            // Ensure the token is provided before proceeding
            throw new Exception("Sonar token is required.");
        }

        // Configure process settings for executing the SonarScanner command
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("sonarscanner end")
                .AppendSecret($"/d:sonar.token={sonarToken}"),
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Execute the SonarScanner command and capture output/error streams
        var result = StartProcess("dotnet", processSettings, out var output, out var error);

        // Evaluate the result of the process execution
        if (result != 0)
        {
            // Log the error message and exit if the command failed
            throw new Exception("Sonar analysis finalization failed. Error:\n" + string.Join(Environment.NewLine, error));
        }
    });
