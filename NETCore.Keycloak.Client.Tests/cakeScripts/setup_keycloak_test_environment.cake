/// <summary>Provision the pinned Keycloak fixture with the .NET 10 file-based application.</summary>
Task("Setup-Testing-Environment")
    .IsDependentOn("Check-Tools")
    .Does(() =>
    {
        var arguments = new ProcessArgumentBuilder()
            .Append("run --file")
            .AppendQuoted(MakeAbsolute(File("../tools/KeycloakTestEnvironment.cs")).FullPath)
            .Append("-- up");
        var result = StartProcess("dotnet", new ProcessSettings { Arguments = arguments });
        if (result != 0)
        {
            throw new Exception("Cannot prepare the managed Keycloak environment. Run the native tool with down before retrying.");
        }
    });
