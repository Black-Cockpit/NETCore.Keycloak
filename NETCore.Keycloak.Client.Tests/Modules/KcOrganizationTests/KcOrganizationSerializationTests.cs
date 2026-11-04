using NETCore.Keycloak.Client.Models.Organizations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NETCore.Keycloak.Client.Tests.Modules.KcOrganizationTests;

/// <summary>
/// Verifies the organization JSON contract for both supported serializers.
/// </summary>
[TestClass]
public class KcOrganizationSerializationTests
{
    /// <summary>
    /// Keeps organization and nested domain field names compatible with the Keycloak REST API.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ShouldUseKeycloakFieldNames(bool useSystemTextJson)
    {
        var organization = new KcOrganization
        {
            Id = "organization-id",
            Name = "Example",
            Alias = "example",
            Enabled = true,
            Description = "Test organization",
            RedirectUrl = "https://example.com",
            Attributes = new Dictionary<string, List<string>> { ["test_attr"] = ["value"] },
            Domains = [new KcOrganizationDomain { Name = "example.com", Verified = false }]
        };

        var serialized = useSystemTextJson
            ? System.Text.Json.JsonSerializer.Serialize(organization)
            : JsonConvert.SerializeObject(organization);
        var json = JObject.Parse(serialized);

        CollectionAssert.AreEquivalent(
            new[] { "id", "name", "alias", "enabled", "description", "redirectUrl", "attributes", "domains" },
            json.Properties().Select(property => property.Name).ToArray());
        Assert.AreEqual("Example", json["name"]?.Value<string>());
        var domain = json["domains"]?.First;
        Assert.IsNotNull(domain);
        Assert.AreEqual("example.com", domain["name"]?.Value<string>());
        Assert.AreEqual(false, domain["verified"]?.Value<bool>());
        Assert.IsNull(domain["Name"]);
        Assert.IsNull(domain["Verified"]);
    }
}
