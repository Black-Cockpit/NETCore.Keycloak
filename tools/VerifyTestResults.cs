#:property TargetFramework=net10.0
#:property PublishAot=false
#:property EnforceCodeStyleInBuild=true
#:property GenerateDocumentationFile=true

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

try
{
    if ( args is ["--self-test"] )
    {
        ReportVerifier.SelfTest();
    }
    else if ( args is ["--audit-json", var audit] )
    {
        ReportVerifier.VerifyAudit(audit);
    }
    else if ( args.Length is 1 or 2 && (args.Length == 1 || args[1] == "--require-http-method-coverage") )
    {
        ReportVerifier.Verify(args[0], args.Length == 2);
    }
    else
    {
        throw new ArgumentException("Usage: <reports-directory> [--require-http-method-coverage] " +
            "| --self-test | --audit-json <file>");
    }

    return 0;
}
catch ( Exception exception )
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

internal static class ReportVerifier
{
    private static string SourcePath([CallerFilePath] string path = "") => path;

    internal static void Verify(string directory, bool requireCoverage)
    {
        var reports = Directory.GetFiles(directory, "*.trx", SearchOption.AllDirectories);
        if ( reports.Length == 0 ) throw new InvalidDataException("No TRX reports found.");
        var total = reports.Sum(report => VerifyRun(ReadXml(report)));
        Console.WriteLine($"Verified {total} passed tests; no failures or skips.");
        if ( !requireCoverage ) return;
        var interfaces = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(SourcePath())!,
            "..", "NETCore.Keycloak.Client", "HttpClients", "Abstraction"));
        var expected = new HashSet<(string Module, string Method)>();
        foreach ( var path in Directory.GetFiles(interfaces, "IKc*.cs") )
        {
            var module = "NETCore.Keycloak.Client.HttpClients.Implementation." +
                Path.GetFileNameWithoutExtension(path)[1..];
            foreach ( Match match in Regex.Matches(File.ReadAllText(path), @"(?m)^\s*Task<[^;]*?>\s+(\w+Async)\s*\(") )
                _ = expected.Add((module, match.Groups[1].Value));
        }

        if ( expected.Count == 0 )
            throw new InvalidDataException("No declared client methods found for coverage verification.");
        var coverage = Directory.GetFiles(directory, "coverage.cobertura.xml", SearchOption.AllDirectories);
        VerifyCoverage(coverage.Select(ReadXml), expected);
        var modules = expected.Select(m => m.Module).Distinct().Count();
        Console.WriteLine($"Verified execution of {expected.Count} declared client methods across {modules} modules.");
    }

    private static XDocument ReadXml(string path)
    {
        using var reader = XmlReader.Create(path, XmlSettings());
        return XDocument.Load(reader);
    }

    private static XmlReaderSettings XmlSettings() => new()
    {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null,
        MaxCharactersInDocument = 128L * 1024 * 1024
    };

    private static int Number(XElement element, string attribute) =>
        int.Parse(( string? ) element.Attribute(attribute) ?? "0", CultureInfo.InvariantCulture);

    private static int VerifyRun(XDocument document)
    {
        var root = document.Root ?? throw new InvalidDataException("Missing report root.");
        var ns = root.Name.Namespace;
        var counters = root.Element(ns + "ResultSummary")?.Element(ns + "Counters") ??
            throw new InvalidDataException("Missing counters.");
        var results = root.Element(ns + "Results")?.Elements(ns + "UnitTestResult").ToArray() ?? [];
        var count = Number(counters, "total");
        if ( count <= 0 || count != Number(counters, "passed") || count != results.Length )
            throw new InvalidDataException("Incomplete test run: totals, passed tests and result records must match.");
        if ( results.Any(result => ( string? ) result.Attribute("outcome") != "Passed") )
            throw new InvalidDataException("Failed, skipped, or aborted tests.");
        var incomplete = new[]
        {
            "failed", "error", "timeout", "aborted", "inconclusive", "notRunnable", "notExecuted", "disconnected"
        };
        foreach ( var name in incomplete )
            if ( Number(counters, name) != 0 ) throw new InvalidDataException($"Nonzero {name} test counter.");
        return count;
    }

    private static void VerifyCoverage(
        IEnumerable<XDocument> documents, HashSet<(string Module, string Method)> expected)
    {
        var covered = new HashSet<(string Module, string Method)>();
        foreach ( var document in documents )
        {
            foreach ( var implementation in document.Descendants("class") )
            {
                var name = ( string? ) implementation.Attribute("name") ?? "";
                var module = name.Split('/')[0];
                var stateMachine = Regex.Match(name, "^" + Regex.Escape(module) + @"/<(\w+Async)>d__\d+$");
                foreach ( var method in implementation.Element("methods")?.Elements("method") ?? [] )
                {
                    var methodName = ( string? ) method.Attribute("name") ?? "";
                    (string Module, string Method) identity;
                    if ( stateMachine.Success && methodName == "MoveNext" )
                        identity = (module, stateMachine.Groups[1].Value);
                    else if ( name == module ) identity = (module, methodName);
                    else continue; // An async lambda must not satisfy the enclosing public method.
                    if ( expected.Contains(identity) &&
                        method.Element("lines")?.Elements("line").Any(line => Number(line, "hits") > 0) == true )
                        _ = covered.Add(identity);
                }
            }
        }

        var missing = expected.Except(covered).OrderBy(m => m.Module, StringComparer.Ordinal)
            .ThenBy(m => m.Method, StringComparer.Ordinal).ToArray();
        if ( missing.Length > 0 )
            throw new InvalidDataException("Unexecuted HTTP methods:\n" +
                string.Join('\n', missing.Select(m => m.Module + "." + m.Method)));
    }

    internal static void VerifyAudit(string path)
    {
        var document = JsonNode.Parse(File.ReadAllText(path)) ??
            throw new InvalidDataException("Missing NuGet audit document.");
        var parameters = (( string? ) document["parameters"] ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if ( ( int? ) document["version"] != 1 || !parameters.Contains("--vulnerable") ||
            !parameters.Contains("--include-transitive")
            || document["sources"] is not JsonArray sources || sources.Count == 0 )
            throw new InvalidDataException("Expected a version 1 NuGet vulnerability audit " +
                "including transitive packages and sources.");
        if ( document["projects"] is not JsonArray projects || projects.Count == 0 )
            throw new InvalidDataException("No projects in NuGet audit.");
        if ( document["problems"] is JsonArray problems && problems.Count > 0 )
            throw new InvalidDataException("NuGet audit reported problems.");
        foreach ( var project in projects )
        {
            if ( string.IsNullOrWhiteSpace(( string? ) project?["path"]) )
                throw new InvalidDataException("Missing project path in NuGet audit.");
            // --vulnerable omits frameworks entirely when a project has no findings.
            if ( project?["frameworks"] is not JsonArray targets ) continue;
            foreach ( var framework in targets )
            {
                foreach ( var group in new[] { "topLevelPackages", "transitivePackages" } )
                {
                    if ( framework?[group] is not JsonArray packages ) continue;
                    foreach ( var package in packages )
                        if ( package?["vulnerabilities"] is JsonArray vulnerabilities && vulnerabilities.Count > 0 )
                            throw new InvalidDataException($"Vulnerable NuGet dependency: {package["id"]}.");
                }
            }
        }

        Console.WriteLine($"Verified NuGet audit: {projects.Count} projects, no reported vulnerabilities.");
    }

    internal static void SelfTest()
    {
        const string passed = """
            <TestRun><Results><UnitTestResult outcome="Passed" /></Results>
            <ResultSummary><Counters total="1" passed="1" /></ResultSummary></TestRun>
            """;
        var tests = 0;
        void Accept(Action action) { action(); tests++; }
        void Reject<T>(Action action) where T : Exception
        {
            try { action(); }
            catch ( T ) { tests++; return; }
            throw new InvalidOperationException("A negative verifier regression unexpectedly passed.");
        }

        XDocument Parse(string xml)
        {
            using var input = new StringReader(xml);
            using var reader = XmlReader.Create(input, XmlSettings());
            return XDocument.Load(reader);
        }

        Accept(() => VerifyRun(Parse(passed)));
        Accept(() => VerifyRun(Parse(passed.Replace("<TestRun>", "<TestRun xmlns=\"urn:trx\">",
            StringComparison.Ordinal))));
        Reject<InvalidDataException>(() => VerifyRun(Parse("<TestRun/>")));
        Reject<InvalidDataException>(() => VerifyRun(Parse(passed.Replace("total=\"1\"", "total=\"0\"",
            StringComparison.Ordinal))));
        Reject<InvalidDataException>(() => VerifyRun(Parse(passed.Replace("passed=\"1\"", "passed=\"0\"",
            StringComparison.Ordinal))));
        Reject<InvalidDataException>(() => VerifyRun(Parse(passed.Replace(
            "outcome=\"Passed\"", "outcome=\"NotExecuted\"",
            StringComparison.Ordinal))));
        Reject<InvalidDataException>(() => VerifyRun(Parse(passed.Replace("<Counters", "<Counters failed=\"1\"",
            StringComparison.Ordinal))));
        Reject<XmlException>(() => Parse("<!DOCTYPE report [<!ENTITY payload 'expanded'>]><report>&payload;</report>"));
        Reject<XmlException>(() => Parse("<!DOCTYPE report SYSTEM 'file:///does-not-exist'><report/>"));
        var expected = new HashSet<(string Module, string Method)> { ("Client", "GetAsync") };
        XDocument Coverage(string name, string method, int hits) => Parse($"""
            <coverage><class name="{name}"><methods><method name="{method}">
            <lines><line hits="{hits}"/></lines></method></methods></class></coverage>
            """);
        Accept(() => VerifyCoverage([Coverage("Client", "GetAsync", 1)], expected));
        Accept(() => VerifyCoverage([Coverage("Client/&lt;GetAsync&gt;d__7", "MoveNext", 1)], expected));
        Reject<InvalidDataException>(() => VerifyCoverage([Coverage("Client", "GetAsync", 0)], expected));
        Reject<InvalidDataException>(() => VerifyCoverage(
            [Coverage("Client/&lt;&gt;c__DisplayClass/&lt;GetAsync&gt;b__0", "MoveNext", 1)], expected));
        Reject<InvalidDataException>(() => VerifyCoverage([], expected));
        var temp = Path.Combine(Path.GetTempPath(), "keycloak-report-check-" + Guid.NewGuid().ToString("N"));
        _ = Directory.CreateDirectory(temp);
        try
        {
            Reject<InvalidDataException>(() => Verify(temp, false));
            File.WriteAllText(Path.Combine(temp, "passed.trx"), passed);
            File.WriteAllText(Path.Combine(temp, "coverage.cobertura.xml"), "<!DOCTYPE coverage><coverage/>");
            Reject<XmlException>(() => Verify(temp, true));
            var audit = Path.Combine(temp, "audit.json");
            const string auditHeader = "\"version\":1,\"parameters\":\"--vulnerable --include-transitive\"," +
                "\"sources\":[\"https://api.nuget.org/v3/index.json\"],";
            File.WriteAllText(audit, "{" + auditHeader + "\"projects\":[{\"path\":\"sample.csproj\"}]}");
            Accept(() => VerifyAudit(audit));
            File.WriteAllText(audit, "{" + auditHeader + """
                "projects":[{"path":"sample.csproj","frameworks":[{
                    "topLevelPackages":[{"id":"unsafe","vulnerabilities":[{}]}]}]}]}
                """);
            Reject<InvalidDataException>(() => VerifyAudit(audit));
            File.WriteAllText(audit, "{" + auditHeader + "\"projects\":[]}");
            Reject<InvalidDataException>(() => VerifyAudit(audit));
            File.WriteAllText(audit, "{" + auditHeader + """
                "projects":[{"path":"sample.csproj","frameworks":[{
                    "transitivePackages":[{"id":"unsafe","vulnerabilities":[{}]}]}]}]}
                """);
            Reject<InvalidDataException>(() => VerifyAudit(audit));
            File.WriteAllText(audit, "{" + auditHeader + """
                "projects":[{"path":"sample.csproj"}],"problems":[{"message":"network failure"}]}
                """);
            Reject<InvalidDataException>(() => VerifyAudit(audit));
            File.WriteAllText(audit, "{\"projects\":[{\"path\":\"sample.csproj\"}]}");
            Reject<InvalidDataException>(() => VerifyAudit(audit));
        }
        finally
        {
            foreach ( var file in Directory.GetFiles(temp) ) File.Delete(file);
            Directory.Delete(temp);
        }

        Console.WriteLine($"Passed {tests} native verifier regressions " +
            "(TRX, XML security, async coverage and NuGet audit).");
    }
}
