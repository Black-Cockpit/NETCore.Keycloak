/// <summary>
/// Main Cake build script to manage the build, restore, and clean tasks.
/// Supports conditional execution based on provided arguments.
/// </summary>

var configuration = Argument("configuration", "Release");

// Define the solution context
var slnContext = ".";

/// <summary>
/// Task to clean build output directories.
/// Ensures a clean state before restoring or building the solution.
/// </summary>
Task("Clean")
    .Does(() =>
    {
        Information("Cleaning build output directories...");
        DotNetClean($"{slnContext}/NETCore.Keycloak.sln");
    });

/// <summary>
/// Task to restore NuGet packages for the solution.
/// Depends on the Clean task to ensure a clean environment before restoring.
/// </summary>
Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        Information("Restoring NuGet packages...");
        DotNetRestore($"{slnContext}/NETCore.Keycloak.sln");
    });

/// <summary>
/// Task to build the solution.
/// Depends on the Restore task to ensure all packages are restored before building.
/// </summary>
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        Information("Building the solution...");
        DotNetBuild($"{slnContext}/NETCore.Keycloak.sln", new DotNetBuildSettings
        {
            Configuration = configuration
        });
    });

/// <summary>
/// Default task to orchestrate conditional execution based on the provided "target" argument.
/// If a target is specified, runs the "Test" task.
/// </summary>
Task("Default")
    .Does(() =>
    {
        // Get the target argument if provided
        var target = Argument<string>("target", null);

        // Check if a specific target is provided and execute the Test task
        if (!string.IsNullOrEmpty(target))
        {
            RunTarget("Build");
        }
    });


// Execute the Default task
RunTarget("Default");