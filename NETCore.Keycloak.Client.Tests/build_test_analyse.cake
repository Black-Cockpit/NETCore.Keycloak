/// <summary>
/// Main Cake build script to manage the build, restore, test, and setup of the Keycloak testing environment.
/// Includes and calls tasks from external scripts.
/// </summary>

// Load external task scripts
#load "cakeScripts/check_tools.cake";
#load "cakeScripts/setup_keycloak_test_environment.cake";
#load "cakeScripts/sonar_analysis.cake";
#load "../build.cake";

// Update the solution context
slnContext = "..";

/// <summary>
/// Executes unit tests using JetBrains dotCover for code coverage analysis.
/// Runs the tests without rebuilding the project.
/// </summary>
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        Information("Running tests with dotCover...");

        // Ensure dotnet is installed
        var dotnetPath = Context.Tools.Resolve("dotnet");
        if (dotnetPath == null)
        {
            Error("dotnet is not installed or cannot be found.");
            Environment.Exit(255);
        }

        // Define the test command
        var testCommand = $"dotcover test {slnContext}/NETCore.Keycloak.sln --configuration {configuration} -l:\"console;verbosity=normal\" --no-restore --no-build --dcReportType=HTML";

        // Configure dotCover settings
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append(testCommand),
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Run tests with dotCover
        var result = StartProcess(dotnetPath, processSettings, out var output, out var error);

        // Evaluate the result of the process execution
        if (result != 0)
        {
            Error("Unit tests failed with dotCover. Error:\n{0}", string.Join(Environment.NewLine, error));
            Environment.Exit(255);
        }

        Information("Unit tests executed successfully with dotCover.");
    });


/// <summary>
/// The BuildTestAnalyse task orchestrates the setup, testing, and analysis of the Keycloak client.
/// It ensures that the environment is correctly configured, executes end-to-end tests, 
/// and performs SonarQube analysis while validating required parameters.
/// </summary>
Task("BuildTestAnalyse")
    .Does(() =>
    {
        Information("Executing Keycloak client build, test, and analysis...");

        // Generate a list of supported Keycloak versions from 20 to 26
        var versions = Enumerable.Range(20, 26 - 20 + 1).ToList();

        // Get the major version argument if provided
        var kcMajorVersion = Argument<int?>("kc_major_version", null);

        // Validate the provided version
        if (!kcMajorVersion.HasValue || !versions.Contains(kcMajorVersion.Value))
        {
            Error($"Invalid Keycloak version: {kcMajorVersion}. Supported versions: {string.Join(", ", versions)}");
            Environment.Exit(255);
        }

        // Retrieve the Sonar token from the arguments
        var sonarToken = Argument<string>("sonar_token", null);

        if (string.IsNullOrWhiteSpace(sonarToken))
        {
            // Ensure the Sonar token is provided before proceeding
            Error("Sonar token is required.");
            Environment.Exit(255);
        }

        // Log the processing of the specific Keycloak version
        Information($"Processing Keycloak Version: {kcMajorVersion.Value}");

        // Set environment variables for Keycloak and SonarQube
        Environment.SetEnvironmentVariable("KC_TEST_VERSION", $"prepare_keycloak_{kcMajorVersion.Value}_environment");
        Environment.SetEnvironmentVariable("KC_SONAR_TOKEN", sonarToken);

        // Execute the required setup, testing, and analysis tasks
        RunTarget("Setup-Testing-Environment");
        RunTarget("SonarBegin");
        RunTarget("Test");
        RunTarget("SonarEnd");
    });

// Execute the BuildTestAnalyse task
RunTarget("BuildTestAnalyse");
