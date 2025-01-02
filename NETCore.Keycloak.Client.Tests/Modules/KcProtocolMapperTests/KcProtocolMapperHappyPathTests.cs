using NETCore.Keycloak.Client.Models.ClientScope;
using NETCore.Keycloak.Client.Tests.Abstraction;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Modules.KcProtocolMapperTests;

/// <summary>
/// Contains tests for validating the Keycloak protocol mapper API functionalities under expected scenarios (Happy Path).
/// </summary>
[TestClass]
[TestCategory("Sequential")]
public class KcProtocolMapperHappyPathTests : KcTestingModule
{
    /// <summary>
    /// Represents the context of the current test.
    /// This constant is used for consistent naming conventions and environment variable management across tests in this class.
    /// </summary>
    private const string TestContext = "GlobalContext";

    /// <summary>
    /// Gets or sets the Keycloak protocol mapper used for testing in happy path scenarios.
    /// </summary>
    private static KcProtocolMapper TestProtocolMapper
    {
        get
        {
            try
            {
                // Retrieve and deserialize the protocol mapper object from the environment variable.
                return JsonConvert.DeserializeObject<KcProtocolMapper>(
                    Environment.GetEnvironmentVariable(
                        $"{nameof(KcProtocolMapperHappyPathTests)}_KC_PROTOCOL_MAPPER") ?? string.Empty);
            }
            catch ( Exception e )
            {
                // Fail the test if deserialization fails.
                Assert.Fail(e.Message);
                return null; // Return statement to satisfy the compiler, unreachable due to Assert.Fail.
            }
        }
        set => Environment.SetEnvironmentVariable($"{nameof(KcProtocolMapperHappyPathTests)}_KC_PROTOCOL_MAPPER",
            JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Sets up the test environment before each test execution.
    /// Ensures that the Keycloak protocol mapper module is correctly initialized and available for use.
    /// </summary>
    [TestInitialize]
    public void Init() => Assert.IsNotNull(KeycloakRestClient.ProtocolMappers);
}
