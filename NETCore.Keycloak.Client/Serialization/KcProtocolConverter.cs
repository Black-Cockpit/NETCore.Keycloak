using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Serialization;

/// <summary>
/// A converter for serializing and deserializing Keycloak protocols to and from JSON.
/// </summary>
public class KcProtocolConverter : KcJsonEnumConverter<KcProtocol>
{
    /// <summary>
    /// A dictionary mapping Keycloak protocols to their respective string representations.
    /// </summary>
    private static readonly Dictionary<KcProtocol?, string> Names =
        new()
        {
            [KcProtocol.Saml] = "saml",
            [KcProtocol.OpenidConnect] = "openid-connect"
        };

    /// <summary>
    /// Converts a Keycloak protocol enum value to its string representation.
    /// </summary>
    /// <param name="value">The Keycloak protocol to convert.</param>
    /// <returns>
    /// A string representation of the protocol if it exists; otherwise, <c>null</c>.
    /// </returns>
    /// <inheritdoc/>
    protected override string ConvertToString(KcProtocol? value) =>
        value != null ? Names[value] : null;

    /// <summary>
    /// Converts a string representation of a protocol to its Keycloak protocol enum value.
    /// </summary>
    /// <param name="s">The string representation of the protocol.</param>
    /// <returns>
    /// The corresponding Keycloak protocol enum value if it exists; otherwise, <c>null</c>.
    /// </returns>
    /// <inheritdoc/>
    protected override KcProtocol? ConvertFromString(string s) => string.IsNullOrWhiteSpace(s)
        ? null
        : Names.ContainsValue(s)
            ? Names.First(pair => pair.Value == s).Key
            : null;
}
