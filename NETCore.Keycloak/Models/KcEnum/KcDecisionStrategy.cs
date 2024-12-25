using System.Runtime.Serialization;

namespace NETCore.Keycloak.Models.KcEnum;

/// <summary>
/// Represents the decision strategy used in Keycloak to evaluate permissions and determine access to resources.
/// </summary>
public enum KcDecisionStrategy
{
    /// <summary>
    /// 'Affirmative' strategy requires at least one permission to evaluate to a positive decision to grant access to a resource and its scopes.
    /// </summary>
    [EnumMember(Value = "AFFIRMATIVE")]
    Affirmative,

    /// <summary>
    /// 'Unanimous' strategy requires all permissions to evaluate to a positive decision for the final decision to also be positive.
    /// </summary>
    [EnumMember(Value = "UNANIMOUS")]
    Unanimous,

    /// <summary>
    /// 'Consensus' strategy requires the number of positive decisions to exceed the number of negative decisions.
    /// If the count of positive and negative decisions is equal, the final decision will be negative.
    /// </summary>
    [EnumMember(Value = "CONSENSUS")]
    Consensus
}
