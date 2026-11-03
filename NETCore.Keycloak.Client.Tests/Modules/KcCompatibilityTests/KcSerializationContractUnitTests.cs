using NETCore.Keycloak.Client.Models.Clients;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Keys;
using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Models.Roles;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NETCore.Keycloak.Client.Tests.Modules.KcCompatibilityTests;

/// <summary>
/// Verifies wire contracts for optional token claims, policies, keys and nested administrative representations.
/// </summary>
[TestClass]
[TestCategory("CoverageUnit")]
public sealed class KcSerializationContractUnitTests
{
    /// <summary>
    /// Preserves optional OIDC, UMA and certificate-binding claims through deserialization and serialization.
    /// </summary>
    [TestMethod]
    public void ShouldPreserveExtendedTokenClaims()
    {
        const string json = """
            {"category":"ACCESS","at_hash":"access-hash","c_hash":"code-hash","s_hash":"state-hash",
             "auth_time":1700000000,"nbf":1700000001,"nonce":"nonce","session_state":"session",
             "address":{"country":"CN","formatted":"Test address","locality":"Test city","postal_code":"100000",
                        "region":"Test region","street_address":"1 Test Street"},
             "authorization":{"permissions":[{"rsid":"resource-id","rsname":"orders","scopes":["read"],
                 "claims":{"tenant":["test"]}}]},"cnf":{"x5t#S256":"certificate-thumbprint"},
             "realm_access":{"roles":["reader"],"verify_caller":true},"birthdate":"2000-01-01","gender":"unspecified",
             "claims_locales":"zh-CN","locale":"zh-CN","middle_name":"Test","nickname":"tester",
             "otherClaims":{"tenant":"test"},"phone_number":"+860000000000","phone_number_verified":true,
             "picture":"https://example.test/picture","profile":"https://example.test/profile",
             "trusted-certs":["test-certificate"],"updated_at":1700000002,"website":"https://example.test",
             "zoneinfo":"Asia/Shanghai"}
            """;
        var token = JsonConvert.DeserializeObject<KcAccessToken>(json);
        Assert.AreEqual(KcAccessTokenCategory.Access, token.Category);
        Assert.AreEqual("certificate-thumbprint", token.Cnf.X5Ts256);
        var permission = token.Authorization.Permissions.Single();
        Assert.AreEqual("resource-id", permission.RsId);
        Assert.AreEqual("orders", permission.RsName);
        Assert.AreEqual("read", permission.Scopes.Single());
        Assert.IsTrue(permission.Claims.ContainsKey("tenant"));
        Assert.IsTrue(token.RealmAccess.VerifyCaller);
        Assert.AreEqual("1 Test Street", token.Address.StreetAddress);
        Assert.AreEqual("access-hash", token.AtHash);
        Assert.AreEqual("code-hash", token.CHash);
        Assert.AreEqual("state-hash", token.Hash);
        Assert.AreEqual(1700000000, token.AuthTime);
        Assert.AreEqual(1700000001L, token.Nbf);
        Assert.AreEqual("test", token.OtherClaims["tenant"]);
        var serialized = JObject.Parse(JsonConvert.SerializeObject(token));
        // Comparing the original subset catches wrong JSON names, dropped claims and incorrect nested shapes.
        foreach ( var property in JObject.Parse(json).Properties() )
        {
            Assert.IsTrue(JToken.DeepEquals(property.Value, serialized[property.Name]), property.Name);
        }
    }

    /// <summary>
    /// Preserves both positive and negative policy effects and their associated resource/scope references.
    /// </summary>
    /// <param name="wireLogic">Keycloak's policy logic value.</param>
    /// <param name="expected">The matching library value.</param>
    [TestMethod]
    [DataRow("POSITIVE", KcPolicyLogic.Positive)]
    [DataRow("NEGATIVE", KcPolicyLogic.Negative)]
    public void ShouldPreserveResourcePolicyEffects(string wireLogic, KcPolicyLogic expected)
    {
        var json = $$"""
            {"id":"policy-id","name":"owner-policy","type":"scope","logic":"{{wireLogic}}",
             "decisionStrategy":"UNANIMOUS","description":"Owner permission","owner":"owner-id",
             "config":{"applyPolicies":"[\"owner\"]"},"policies":["owner"],"resources":["resource-id"],
             "resourcesData":[{"_id":"resource-id","name":"orders"}],"scopes":["read"],
             "scopesData":[{"id":"scope-id","name":"read"}]}
            """;
        var policy = JsonConvert.DeserializeObject<KcClientResourcePolicy>(json);
        Assert.AreEqual(expected, policy.Logic);
        Assert.AreEqual(KcDecisionStrategy.Unanimous, policy.DecisionStrategy);
        Assert.AreEqual("policy-id", policy.Id);
        Assert.AreEqual("owner-id", policy.Owner);
        Assert.AreEqual("scope", policy.Type);
        Assert.AreEqual("owner-policy", policy.Name);
        Assert.AreEqual("Owner permission", policy.Description);
        Assert.AreEqual("owner", policy.Policies.Single());
        Assert.AreEqual("resource-id", policy.Resources.Single());
        Assert.AreEqual("orders", policy.ResourcesData.Single().Name);
        Assert.AreEqual("read", policy.Scopes.Single());
        Assert.AreEqual("scope-id", policy.ScopesData.Single().Id);
        Assert.IsNotNull(policy.Config);
        var wire = JObject.Parse(JsonConvert.SerializeObject(policy));
        Assert.AreEqual(wireLogic, wire["logic"]?.Value<string>());
        Assert.AreEqual("UNANIMOUS", wire["decisionStrategy"]?.Value<string>());
        AssertJsonSubset(JObject.Parse(json), wire);
    }

    /// <summary>
    /// Maps every category using the exact uppercase protocol representation.
    /// </summary>
    /// <param name="category">The library category.</param>
    /// <param name="wire">The protocol string.</param>
    [TestMethod]
    [DataRow(KcAccessTokenCategory.Internal, "INTERNAL")]
    [DataRow(KcAccessTokenCategory.Access, "ACCESS")]
    [DataRow(KcAccessTokenCategory.Id, "ID")]
    [DataRow(KcAccessTokenCategory.Admin, "ADMIN")]
    [DataRow(KcAccessTokenCategory.AuthorizationResponse, "AUTHORIZATION_RESPONSE")]
    [DataRow(KcAccessTokenCategory.Logout, "LOGOUT")]
    [DataRow(KcAccessTokenCategory.Userinfo, "USERINFO")]
    public void ShouldRoundTripTokenCategories(KcAccessTokenCategory category, string wire)
    {
        var token = JsonConvert.DeserializeObject<KcAccessToken>($"{{\"category\":\"{wire}\"}}");
        Assert.AreEqual(category, token.Category);
        Assert.AreEqual(wire, JObject.FromObject(token)["category"]?.Value<string>());
    }

    /// <summary>
    /// Global converter registration must convert enums without intercepting ordinary strings.
    /// </summary>
    [TestMethod]
    public void ShouldUseEnumConverterWithoutInterceptingStrings()
    {
        var converter = new KcPolicyLogicConverter();
        Assert.IsTrue(converter.CanConvert(typeof(KcPolicyLogic)));
        Assert.IsTrue(converter.CanConvert(typeof(KcPolicyLogic?)));
        Assert.IsFalse(converter.CanConvert(typeof(string)));
        var settings = new JsonSerializerSettings { Converters = { converter } };
        var json = JsonConvert.SerializeObject(new { logic = KcPolicyLogic.Negative, label = "literal" }, settings);
        Assert.AreEqual("NEGATIVE", JObject.Parse(json)["logic"]?.Value<string>());
        Assert.AreEqual("literal", JObject.Parse(json)["label"]?.Value<string>());
    }

    /// <summary>
    /// Nullable enum input preserves null/unknown values and valid values in array representations.
    /// </summary>
    [TestMethod]
    public void ShouldReadNullableEnumArraysAndWriteExplicitNull()
    {
        var converter = new KcPolicyLogicConverter();
        using var reader = new JsonTextReader(new StringReader("[\"POSITIVE\",\"NEGATIVE\",null,\"FUTURE\"]"));
        Assert.IsTrue(reader.Read());
        var result = ( List<KcPolicyLogic?> ) converter.ReadJson(reader, typeof(List<KcPolicyLogic?>), null,
            JsonSerializer.CreateDefault());
        Assert.HasCount(4, result);
        Assert.AreEqual(KcPolicyLogic.Positive, result[0]);
        Assert.AreEqual(KcPolicyLogic.Negative, result[1]);
        Assert.IsNull(result[2]);
        Assert.IsNull(result[3]);
        using var text = new StringWriter();
        using var writer = new JsonTextWriter(text);
        writer.WriteStartArray();
        converter.WriteJson(writer, null, JsonSerializer.CreateDefault());
        writer.WriteEndArray();
        Assert.AreEqual("[null]", text.ToString());
    }

    /// <summary>
    /// Reads active signing-key metadata without confusing key identifiers and key material.
    /// </summary>
    [TestMethod]
    public void ShouldPreserveKeyMetadata()
    {
        const string json = """
            {"active":{"HS256":"hmac-id","RS256":"rsa-id","AES":"aes-id"},"keys":[
             {"algorithm":"RS256","certificate":"test-certificate","kid":"rsa-id","providerId":"provider-id",
              "providerPriority":100,"publicKey":"test-public-key","status":"ACTIVE","type":"RSA","use":"SIG"}]}
            """;
        var metadata = JsonConvert.DeserializeObject<KcKeyMetadata>(json);
        Assert.AreEqual("hmac-id", metadata.Active.Hs256);
        Assert.AreEqual("rsa-id", metadata.Active.Rs256);
        Assert.AreEqual("aes-id", metadata.Active.Aes);
        var key = metadata.Keys.Single();
        Assert.AreEqual(metadata.Active.Rs256, key.Kid);
        Assert.AreEqual("RS256", key.Algorithm);
        Assert.AreEqual("RSA", key.Type);
        Assert.AreEqual("SIG", key.Use);
        Assert.AreEqual("ACTIVE", key.Status);
        Assert.AreEqual(100, key.ProviderPriority);
        Assert.AreEqual("provider-id", key.ProviderId);
        Assert.AreEqual("test-public-key", key.PublicKey);
        Assert.AreEqual("test-certificate", key.Certificate);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(json), JObject.FromObject(metadata)));
    }

    /// <summary>
    /// Accepts the previous id input while emitting the resource identifier used by Keycloak.
    /// </summary>
    /// <param name="property">The identifier key in the input representation.</param>
    [TestMethod]
    [DataRow("_id")]
    [DataRow("id")]
    public void ShouldPreserveResourceIdsAcrossLegacyAndServerRepresentations(string property)
    {
        var resource = JsonConvert.DeserializeObject<KcClientResource>($"{{\"{property}\":\"resource-id\"}}");
        Assert.AreEqual("resource-id", resource.Id);
        var json = JObject.FromObject(resource);
        Assert.AreEqual("resource-id", json["_id"]?.Value<string>());
        Assert.IsFalse(json.ContainsKey("id"));
    }

    /// <summary>
    /// Preserves nested client-role mappings, realm composites, consent history and federated identity links.
    /// </summary>
    [TestMethod]
    public void ShouldPreserveAdministrativeNestedRepresentations()
    {
        var mapping = JsonConvert.DeserializeObject<KcRoleMapping>("""
            {"clientMappings":{"api":{"id":"client-id","client":"api","mappings":[{"id":"role-id","name":"read"}]}}}
            """);
        var client = mapping.ClientMappings["api"];
        Assert.AreEqual("client-id", client.Id);
        Assert.AreEqual("api", client.Client);
        Assert.AreEqual("read", client.Mappings.Single().Name);
        var composite = JsonConvert.DeserializeObject<KcRoleComposite>("{\"realm\":[\"reader\"],\"client\":{}}");
        Assert.AreEqual("reader", composite.Realm.Single());
        Assert.IsEmpty(composite.Client);
        var user = JsonConvert.DeserializeObject<KcUser>("""
            {"clientConsents":[{"clientId":"api","grantedClientScopes":["profile"],
              "createdDate":1,"lastUpdatedDate":2}],
             "federatedIdentities":[{"identityProvider":"oidc","userId":"external-id","userName":"external-user"}]}
            """);
        var consent = user.ClientConsents.Single();
        Assert.AreEqual("api", consent.ClientId);
        Assert.AreEqual("profile", consent.GrantedClientScopes.Single());
        Assert.AreEqual(1L, consent.CreatedDate);
        Assert.AreEqual(2L, consent.LastUpdatedDate);
        var identity = user.FederatedIdentities.Single();
        Assert.AreEqual("oidc", identity.IdentityProvider);
        Assert.AreEqual("external-id", identity.UserId);
        Assert.AreEqual("external-user", identity.UserName);
        var result = JsonConvert.DeserializeObject<KcGlobalRequestResult>(
            "{\"successRequests\":[\"node-1\"],\"failedRequests\":[\"node-2\"]}");
        Assert.AreEqual("node-1", result.SuccessRequests.Single());
        Assert.AreEqual("node-2", result.FailedRequests.Single());
        var mapper = JsonConvert.DeserializeObject<KcClientProtocolMapper>("{\"consentText\":\"Share profile\"}");
        Assert.AreEqual("Share profile", mapper.ConsentText);
        Assert.AreEqual("orders", JsonConvert.DeserializeObject<KcProtectedResource>(
            "{\"Resource\":\"orders\"}").Resource);
    }

    private static void AssertJsonSubset(JToken expected, JToken actual)
    {
        Assert.IsNotNull(actual, expected.Path);
        if ( expected is JObject properties )
        {
            foreach ( var property in properties.Properties() )
            {
                AssertJsonSubset(property.Value, actual[property.Name]);
            }
        }
        else if ( expected is JArray array )
        {
            Assert.AreEqual(array.Count, actual.Count(), expected.Path);
            for ( var index = 0; index < array.Count; index++ )
            {
                AssertJsonSubset(array[index], actual[index]);
            }
        }
        else
        {
            Assert.IsTrue(JToken.DeepEquals(expected, actual), expected.Path);
        }
    }
}
