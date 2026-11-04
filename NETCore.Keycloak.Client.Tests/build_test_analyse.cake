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

        // Resolve solution path to absolute for dotCover compatibility
        var slnPath = MakeAbsolute(new FilePath($"{slnContext}/NETCore.Keycloak.slnx"));

        // Define the dotCover cover command with XML report output
        var testCommand = $"cover --xml-report-output=dotCover.Output.xml -- test {slnPath} --configuration {configuration} -l:\"console;verbosity=normal\" --no-restore --no-build";

        // Configure dotCover settings
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append(testCommand),
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Run tests with dotCover
        var result = StartProcess("dotCover", processSettings, out var output, out var error);

        // Evaluate the result of the process execution
        if (result != 0)
        {
            throw new Exception("Unit tests failed with dotCover. Error:\n" + string.Join(Environment.NewLine, error));
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

        // The managed integration fixture targets the pinned Keycloak 26 release.
        var versions = new List<int> { 26 };

        // Get the major version argument if provided
        var kcMajorVersion = Argument<int?>("kc_major_version", 26);

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
        Environment.SetEnvironmentVariable("KC_SONAR_TOKEN", sonarToken);

        RunTarget("Setup-Testing-Environment");
        try
        {
            RunTarget("SonarBegin");
            RunTarget("Test");
            RunTarget("SonarEnd");
        }
        finally
        {
            var result = StartProcess("dotnet", new ProcessSettings
            {
                Arguments = new ProcessArgumentBuilder().Append("run --file")
                    .AppendQuoted(MakeAbsolute(File("../tools/KeycloakTestEnvironment.cs")).FullPath)
                    .Append("-- down")
            });
            if (result != 0) throw new Exception("Cannot stop the managed test environment.");
        }
    });

// Execute the BuildTestAnalyse task
RunTarget("BuildTestAnalyse");
