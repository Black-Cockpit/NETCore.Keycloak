/// <summary>
/// Main Cake build script to manage the build, restore, test, and setup of the Keycloak testing environment.
/// Includes and calls tasks from external scripts.
/// </summary>

var configuration = Argument("configuration", "Release");

// Load external task scripts
#load "cakeScripts/check_tools.cake";
#load "cakeScripts/setup_keycloak_test_environment.cake";

/// <summary>
/// Task to clean build output directories.
/// Ensures a clean state before restoring or building the solution.
/// </summary>
Task("Clean")
    .Does(() =>
    {
        Information("Cleaning build output directories...");
        DotNetClean("../NETCore.Keycloak.sln");
    });

/// <summary>
/// Task to restore NuGet packages for the solution.
/// Depends on the Clean task to ensure a clean environment.
/// </summary>
Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        Information("Restoring NuGet packages...");
        DotNetRestore("../NETCore.Keycloak.sln");
    });

/// <summary>
/// Task to build the solution.
/// Depends on the Restore task to ensure all packages are restored before the build.
/// </summary>
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        Information("Building the solution...");
        DotNetBuild("../NETCore.Keycloak.sln", new DotNetBuildSettings
        {
            Configuration = configuration
        });
    });

/// <summary>
/// Task to run tests for the solution.
/// Depends on the Build task and ensures all tests are executed with detailed verbosity.
/// </summary>
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        Information("Running unit tests...");
        DotNetTest("../NETCore.Keycloak.sln", new DotNetTestSettings
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

// Execute the Default task
RunTarget("E2E-Tests");
