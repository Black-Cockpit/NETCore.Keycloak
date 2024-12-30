using NETCore.Keycloak.Client.Tests.Models;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Tests.Abstraction;

/// <summary>
/// Base class for Keycloak testing modules, providing a shared testing environment configuration.
/// This class is intended to be inherited by specific Keycloak test modules.
/// </summary>
public abstract class KcTestingModule
{
    /// <summary>
    /// Represents the testing environment configuration for Keycloak integration tests.
    /// Loaded from the `Assets/testing_environment.json` file.
    /// </summary>
    protected KcTestEnvironment TestEnvironment = new();

    /// <summary>
    /// Loads the test environment configuration from the `Assets/testing_environment.json` file.
    /// The loaded configuration is deserialized into the <see cref="KcTestEnvironment"/> object.
    /// Ensures that the configuration is not null after loading.
    /// </summary>
    protected void LoadConfiguration()
    {
        using var sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(),
            "Assets/testing_environment.json"));
        TestEnvironment = JsonConvert.DeserializeObject<KcTestEnvironment>(sr.ReadToEnd());

        Assert.IsNotNull(TestEnvironment, "The test environment configuration must not be null.");
    }
}
