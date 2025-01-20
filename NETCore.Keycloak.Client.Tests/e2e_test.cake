/// <summary>
/// Main Cake build script to manage the build, restore, test, and setup of the Keycloak testing environment.
/// Includes and calls tasks from external scripts.
/// </summary>

// Load external task scripts
#load "cakeScripts/check_tools.cake";
#load "cakeScripts/setup_keycloak_test_environment.cake";
#load "../build.cake";

// Update the solution context
slnContext = "..";

/// <summary>
/// Task to run tests for the solution.
/// Depends on the Build task and ensures all tests are executed with detailed verbosity.
/// </summary>
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        Information("Running unit tests...");
        DotNetTest($"{slnContext}/NETCore.Keycloak.sln", new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            Verbosity = DotNetVerbosity.Normal
        });
    });

/// <summary>
/// The E2E-Tests task to orchestrate the setup of the Keycloak testing environment and run tests.
/// Sets environment variables and ensures tasks are executed in the correct order.
/// </summary>
Task("E2E-Tests")
    .Does(() =>
    {
        Information("Executing Keycloak client E2E tests...");

        // Generate a list of supported Keycloak versions from 20 to 26
        var versions = Enumerable.Range(20, 26 - 20 + 1).ToList();

        // Get the major version argument if provided
        var kcMajorVersion = Argument<int?>("kc_major_version", null);

        // Check if a specific version is provided
        if (kcMajorVersion.HasValue)
        {
            // Validate the version
            if (!versions.Contains(kcMajorVersion.Value))
            {
                Error($"Invalid Keycloak version: {kcMajorVersion.Value}. Supported versions: {string.Join(", ", versions)}");
                Environment.Exit(255);
            }

            // Process the specific version
            Information($"Processing specific Keycloak Version: {kcMajorVersion.Value}");

            Environment.SetEnvironmentVariable("KC_TEST_VERSION", $"prepare_keycloak_{kcMajorVersion.Value}_environment");

            RunTarget("Setup-Testing-Environment");
            RunTarget("Test");
        }
        else
        {
            // Process all supported versions
            foreach (var version in versions)
            {
                Information($"Processing Keycloak Version: {version}");

                Environment.SetEnvironmentVariable("KC_TEST_VERSION", $"prepare_keycloak_{version}_environment");

                RunTarget("Setup-Testing-Environment");
                RunTarget("Test");
            }
        }
    });

// Execute the E2E-Tests task
RunTarget("E2E-Tests");
