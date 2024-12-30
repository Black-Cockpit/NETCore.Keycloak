/// <summary>
/// Task to check if Docker Compose is installed on the system.
/// It resolves the path to the Docker Compose executable, executes the `--version` command,
/// and captures the output or errors for validation.
/// </summary>
Task("Check-Docker-Compose")
    .Does(() =>
{
    // Resolve the path to the Docker Compose executable
    FilePath dockerComposePath = Context.Tools.Resolve("docker-compose");

    // Configure process settings to execute the `docker-compose --version` command
    var processSettings = new ProcessSettings
    {
        Arguments = new ProcessArgumentBuilder()
            .Append("--version"), 
        RedirectStandardOutput = true, 
        RedirectStandardError = true  
    };

    // Execute the process and capture output and error streams
    var result = StartProcess(dockerComposePath, processSettings, out var output, out var error);

    // Evaluate the result of the process execution
    if (result == 0)
    {
        // Log the captured output if the command executed successfully
        Information("Docker Compose is installed. Version info:\n{0}", string.Join(Environment.NewLine, output));
    }
    else
    {
        // Log the error message and throw an exception if the command failed
        Error("Docker Compose is not installed or not accessible. Error:\n{0}", string.Join(Environment.NewLine, error));
        Environment.Exit(255);
    }    
});

// Define the entry point for the script by running the Check-Docker-Compose task
RunTarget("Check-Docker-Compose");
