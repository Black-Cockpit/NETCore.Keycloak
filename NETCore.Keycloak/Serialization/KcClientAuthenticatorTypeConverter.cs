using NETCore.Keycloak.Models.KcEnum;

namespace NETCore.Keycloak.Serialization;

/// <inheritdoc/>>
public class KcClientAuthenticatorTypeConverter : KcJsonEnumConverter<KcClientAuthenticatorType>
{
    /// <summary>
    /// Keycloak authenticator type names
    /// </summary>
    private static readonly Dictionary<KcClientAuthenticatorType?, string> Names =
        new()
        {
            [KcClientAuthenticatorType.Jwt] = "client-secret",
            [KcClientAuthenticatorType.Secret] = "client-jwt"
        };

    /// <inheritdoc/>>
    protected override string ConvertToString(KcClientAuthenticatorType? value) =>
        value != null ? Names[value] : null;

    /// <inheritdoc/>>
    protected override KcClientAuthenticatorType? ConvertFromString(string s) =>
        string.IsNullOrWhiteSpace(s) ? null :
        Names.ContainsValue(s) ? Names.First(pair => pair.Value == s).Key : null;
}
