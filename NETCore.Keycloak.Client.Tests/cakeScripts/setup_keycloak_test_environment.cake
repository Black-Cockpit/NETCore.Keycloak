/// <summary>
/// Task to set up the testing environment by executing a `make` command.
/// Resolves the `make` executable, processes the specified parameter for the task,
/// and captures the output and error streams for evaluation.
/// </summary>
Task("Setup-Testing-Environment")
    .Does(() =>
    {
        // Resolve the path to the `make` executable
        FilePath makePath = Context.Tools.Resolve("make");
        
        // Access the parameter defined in the main file or use a default value
        var testingVersion = Environment.GetEnvironmentVariable("KC_TEST_VERSION") ?? "prepare_keycloak_20_environment";
        
        // Configure process settings to execute the `make` command
        var processSettings = new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append(testingVersion), // Use the provided parameter
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Execute the process and capture output and error streams
        var result = StartProcess(makePath, processSettings, out var output, out var error);

        // Evaluate the result of the process execution
        if (result != 0)
        {
            // Log the error message and throw an exception if the command failed
            Error("Cannot prepare testing environment. Error:\n{0}", string.Join(Environment.NewLine, error));
            Environment.Exit(255);
        }
    });