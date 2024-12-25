using NETCore.Keycloak.Models.KcEnum;

namespace NETCore.Keycloak.Serialization;

/// <summary>
/// A custom JSON converter for Keycloak decision strategies.
/// </summary>
public class KcDecisionStrategyConverter : KcJsonEnumConverter<KcDecisionStrategy>
{
    /// <summary>
    /// A dictionary mapping <see cref="KcDecisionStrategy"/> enum values to their corresponding Keycloak representation strings.
    /// </summary>
    private static readonly Dictionary<KcDecisionStrategy?, string> Names =
        new()
        {
            [KcDecisionStrategy.Affirmative] = "AFFIRMATIVE",
            [KcDecisionStrategy.Consensus] = "CONSENSUS",
            [KcDecisionStrategy.Unanimous] = "UNANIMOUS"
        };

    /// <summary>
    /// Converts a <see cref="KcDecisionStrategy"/> enum value to its string representation.
    /// </summary>
    /// <param name="value">The <see cref="KcDecisionStrategy"/> value to convert.</param>
    /// <returns>
    /// A string representing the Keycloak decision strategy, or <c>null</c> if the value is <c>null</c>.
    /// </returns>
    protected override string ConvertToString(KcDecisionStrategy? value) =>
        value != null ? Names[value] : null;

    /// <summary>
    /// Converts a string representation of a decision strategy to its corresponding <see cref="KcDecisionStrategy"/> enum value.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>
    /// A <see cref="KcDecisionStrategy"/> value corresponding to the string, or <c>null</c> if the string is <c>null</c> or empty.
    /// </returns>
    protected override KcDecisionStrategy? ConvertFromString(string s) =>
        string.IsNullOrWhiteSpace(s) ? null :
        Names.ContainsValue(s) ? Names.First(pair => pair.Value == s).Key : null;
}
