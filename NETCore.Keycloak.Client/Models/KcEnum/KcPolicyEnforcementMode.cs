using System.Runtime.Serialization;

namespace NETCore.Keycloak.Client.Models.KcEnum;

/// <summary>
/// The policy enforcement mode dictates how policies are enforced when evaluating authorization requests.
/// </summary>
public enum KcPolicyEnforcementMode
{
    /// <summary>
    /// 'Enforcing' means requests are denied by default even when there is no policy associated with a given resource.
    /// </summary>
    [EnumMember(Value = "ENFORCING")] Enforcing,

    /// <summary>
    /// 'Permissive' means requests are allowed even when there is no policy associated with a given resource.
    /// </summary>
    [EnumMember(Value = "PERMISSIVE")] Permissive,

    /// <summary>
    /// 'Disabled' completely disables the evaluation of policies and allows access to any resource.
    /// </summary>
    [EnumMember(Value = "DISABLED")] Disabled
}
