using NETCore.Keycloak.Models.KcEnum;

namespace NETCore.Keycloak.Serialization;

/// <summary>
/// A custom JSON converter for Keycloak policy logic.
/// </summary>
public class KcPolicyLogicConverter : KcJsonEnumConverter<KcPolicyLogic>
{
    /// <summary>
    /// A dictionary mapping <see cref="KcPolicyLogic"/> enum values to their corresponding Keycloak representation strings.
    /// </summary>
    private static readonly Dictionary<KcPolicyLogic?, string> Names =
        new()
        {
            [KcPolicyLogic.Negative] = "NEGATIVE",
            [KcPolicyLogic.Positive] = "POSITIVE"
        };

    /// <summary>
    /// Converts a <see cref="KcPolicyLogic"/> enum value to its string representation.
    /// </summary>
    /// <param name="value">The <see cref="KcPolicyLogic"/> value to convert.</param>
    /// <returns>
    /// A string representing the Keycloak policy logic, or <c>null</c> if the value is <c>null</c>.
    /// </returns>
    protected override string ConvertToString(KcPolicyLogic? value) =>
        value != null ? Names[value] : null;

    /// <summary>
    /// Converts a string representation of a policy logic to its corresponding <see cref="KcPolicyLogic"/> enum value.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>
    /// A <see cref="KcPolicyLogic"/> value corresponding to the string, or <c>null</c> if the string is <c>null</c> or empty.
    /// </returns>
    protected override KcPolicyLogic? ConvertFromString(string s) =>
        string.IsNullOrWhiteSpace(s)
            ? null
            : Names.ContainsValue(s)
                ? Names.First(pair => pair.Value == s).Key
                : null;
}
