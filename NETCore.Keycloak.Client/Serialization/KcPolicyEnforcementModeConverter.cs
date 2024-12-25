using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Serialization;

/// <inheritdoc/>>
public class KcPolicyEnforcementModeConverter : KcJsonEnumConverter<KcPolicyEnforcementMode>
{
    /// <summary>
    /// Keycloak policy enforcement mode names
    /// </summary>
    private static readonly Dictionary<KcPolicyEnforcementMode?, string> Names =
        new()
        {
            [KcPolicyEnforcementMode.Enforcing] = "ENFORCING",
            [KcPolicyEnforcementMode.Disabled] = "DISABLED",
            [KcPolicyEnforcementMode.Permissive] = "PERMISSIVE"
        };

    /// <inheritdoc/>>
    protected override string ConvertToString(KcPolicyEnforcementMode? value) =>
        value != null ? Names[value] : null;

    /// <inheritdoc/>>
    protected override KcPolicyEnforcementMode? ConvertFromString(string s) =>
        string.IsNullOrWhiteSpace(s) ? null :
        Names.ContainsValue(s) ? Names.First(pair => pair.Value == s).Key : null;
}
