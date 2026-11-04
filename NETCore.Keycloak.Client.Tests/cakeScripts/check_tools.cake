/// <summary>Check the .NET SDK and Docker Compose v2 through the system PATH.</summary>
Task("Check-Tools")
    .Does(() =>
    {
        var tools = new Dictionary<string, string>
        {
            { "dotnet", "--version" },
            { "docker", "compose version" }
        };

        foreach (var tool in tools)
        {
            var settings = new ProcessSettings
            {
                Arguments = new ProcessArgumentBuilder().Append(tool.Value),
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            // Tools.Resolve only checks explicitly registered Cake tools; StartProcess also uses PATH.
            var result = StartProcess(tool.Key, settings, out var output, out var error);
            if (result != 0)
            {
                throw new Exception("Cannot run " + tool.Key + ": " + string.Join(Environment.NewLine, error));
            }

            Information("{0}: {1}", tool.Key, string.Join(Environment.NewLine, output));
        }
    });
