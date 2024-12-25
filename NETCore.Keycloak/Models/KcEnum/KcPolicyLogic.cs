using System.Runtime.Serialization;

namespace NETCore.Keycloak.Models.KcEnum;

/// <summary>
/// Represents the logic used in Keycloak policies to determine the effect of the policy's evaluation.
/// </summary>
public enum KcPolicyLogic
{
    /// <summary>
    /// 'Positive' logic uses the resulting effect (permit or deny) obtained during the policy evaluation directly for the decision.
    /// </summary>
    [EnumMember(Value = "POSITIVE")]
    Positive,

    /// <summary>
    /// 'Negative' logic negates the resulting effect of the policy evaluation. For example, a permit becomes a deny and vice versa.
    /// </summary>
    [EnumMember(Value = "NEGATIVE")]
    Negative
}
