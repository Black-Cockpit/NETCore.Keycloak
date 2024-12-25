using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Serialization;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Keycloak resource server representation.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_resourceserverrepresentation"/>
/// </summary>
public class KcClientResourceServer
{
    /// <summary>
    /// Should resources be managed remotely by the resource server? If false,
    /// resources can be managed only from this admin console.
    /// </summary>
    [JsonProperty("allowRemoteResourceManagement")]
    public bool? AllowRemoteResourceManagement { get; set; }

    /// <summary>
    /// Related resource client id (resource owner)
    /// </summary>
    [JsonProperty("clientId")]
    public string ClientId { get; set; }

    /// <summary>
    /// The decision strategy dictates how permissions are evaluated and how a final decision is obtained.
    /// 'Affirmative' means that at least one permission must evaluate to a positive decision in order to grant access to a resource and its scopes.
    /// 'Unanimous' means that all permissions must evaluate to a positive decision in order for the final decision to be also positive.
    /// </summary>
    [JsonProperty("decisionStrategy")]
    [JsonConverter(typeof(KcDecisionStrategyConverter))]
    public KcDecisionStrategy DecisionStrategy { get; set; } = KcDecisionStrategy.Affirmative;

    /// <summary>
    /// Resource server id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// A unique name for this resource. The name can be used to uniquely identify a resource, useful when querying for a specific resource.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Resource server policies <see cref="KcClientResourcePolicy"/>
    /// </summary>
    [JsonProperty("policies")]
    public IEnumerable<KcClientResourcePolicy> Policies { get; set; }

    /// <summary>
    /// The policy enforcement mode dictates how policies are enforced when evaluating authorization requests.
    /// <see cref="KcPolicyEnforcementMode"/>
    /// </summary>
    [JsonProperty("policyEnforcementMode")]
    [JsonConverter(typeof(KcPolicyEnforcementModeConverter))]
    public KcPolicyEnforcementMode PolicyEnforcementMode { get; set; } =
        KcPolicyEnforcementMode.Enforcing;

    /// <summary>
    /// List of resources
    /// <see cref="KcClientResource"/>
    /// </summary>
    [JsonProperty("resources")]
    public IEnumerable<KcClientResource> Resources { get; set; }

    /// <summary>
    /// List of scopes
    /// <see cref="KcClientResourceScope"/>
    /// </summary>
    [JsonProperty("scopes")]
    public IEnumerable<KcClientResourceScope> Scopes { get; set; }
}
