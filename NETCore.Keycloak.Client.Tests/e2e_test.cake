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
        DotNetTest($"{slnContext}/NETCore.Keycloak.slnx", new DotNetTestSettings
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

        var kcMajorVersion = Argument<int>("kc_major_version", 26);
        if (kcMajorVersion != 26)
        {
            throw new Exception("The managed integration fixture targets Keycloak 26.8.1; use --kc_major_version=26.");
        }

        RunTarget("Setup-Testing-Environment");
        try
        {
            RunTarget("Test");
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

// Execute the E2E-Tests task
RunTarget("E2E-Tests");
