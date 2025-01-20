/// <summary>
/// Task to check if required tools are installed on the system.
/// Iterates over a list of tools, resolves their paths, executes the `--version` command,
/// and captures the output or errors for validation.
/// </summary>
Task("Check-Tools")
    .Does(() =>
    {
        // List of tools to check with their respective version commands
        var tools = new Dictionary<string, string>
        {
            { "make", "--version" },
            { "docker-compose", "--version" }
        };

        var index = 0;

        foreach (var tool in tools)
        {
            var toolName = tool.Key;
            var toolCommand = tool.Value;

            try
            {
                // Resolve the path to the tool executable
                FilePath toolPath = Context.Tools.Resolve(toolName);

                // Configure process settings to execute the version command
                var processSettings = new ProcessSettings
                {
                    Arguments = new ProcessArgumentBuilder().Append(toolCommand),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                // Execute the process and capture output and error streams
                var result = StartProcess(toolPath, processSettings, out var output, out var error);

                // Evaluate the result of the process execution
                if (result == 0)
                {
                    // Log the captured output if the command executed successfully
                    Information(
                        index == 0
                            ? "{0} is installed. Version info:\n{1}"
                            : "\n{0} is installed. Version info:\n{1}",
                        toolName,
                        string.Join(Environment.NewLine, output)
                    );
                }
                else
                {
                    // Log the error message and throw an exception if the command failed
                    Error(
                        index == 0
                            ? "{0} is not installed or not accessible. Error:\n{1}"
                            : "\n{0} is not installed or not accessible. Error:\n{1}",
                        toolName,
                        string.Join(Environment.NewLine, error)
                    );
                    Environment.Exit(255);
                }
            }
            catch (Exception ex)
            {
                // Handle cases where the tool is not found or other exceptions occur
                Error("Failed to check {0}. Exception: {1}", toolName, ex.Message);
                Environment.Exit(255);
            }

            index++;
        }
    });
