using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Serialization;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Represents a policy configuration for a Keycloak client resource.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_policyrepresentation"/>
/// </summary>
public class KcClientResourcePolicy
{
    /// <summary>
    /// Gets or sets the configuration options for the resource policy.
    /// </summary>
    /// <value>
    /// An object representing the resource policy configuration.
    /// </value>
    [JsonProperty("config")]
    public object Config { get; set; }

    /// <summary>
    /// Gets or sets the decision strategy for this policy.
    /// The strategy determines how associated policies are evaluated and how the final decision is obtained.
    /// </summary>
    /// <value>
    /// A <see cref="KcDecisionStrategy"/> indicating the decision strategy.
    /// </value>
    [JsonProperty("decisionStrategy")]
    [JsonConverter(typeof(KcDecisionStrategyConverter))]
    public KcDecisionStrategy DecisionStrategy { get; set; }

    /// <summary>
    /// Gets or sets a description for this policy.
    /// </summary>
    /// <value>
    /// A string describing the policy.
    /// </value>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the resource policy.
    /// </summary>
    /// <value>
    /// A string representing the policy ID.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the logic for this policy.
    /// </summary>
    /// <value>
    /// A <see cref="KcPolicyLogic"/> representing the policy logic.
    /// </value>
    [JsonProperty("logic")]
    [JsonConverter(typeof(KcPolicyLogicConverter))]
    public KcPolicyLogic Logic { get; set; }

    /// <summary>
    /// Gets or sets a unique name for this policy.
    /// The name can be used to identify the policy, especially when querying.
    /// </summary>
    /// <value>
    /// A string representing the policy name.
    /// </value>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the owner of this policy.
    /// </summary>
    /// <value>
    /// A string representing the client ID that owns this policy.
    /// </value>
    [JsonProperty("owner")]
    public string Owner { get; set; }

    /// <summary>
    /// Gets or sets the associated policy names.
    /// </summary>
    /// <value>
    /// A collection of strings representing the associated policies.
    /// </value>
    [JsonProperty("policies")]
    public IEnumerable<string> Policies { get; set; }

    /// <summary>
    /// Gets or sets the names of associated resources.
    /// </summary>
    /// <value>
    /// A collection of strings representing the associated resource names.
    /// </value>
    [JsonProperty("resources")]
    public IEnumerable<string> Resources { get; set; }

    /// <summary>
    /// Gets or sets the associated resource data.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcClientResource"/> representing the resource data.
    /// </value>
    [JsonProperty("resourcesData")]
    public IEnumerable<KcClientResource> ResourcesData { get; set; }

    /// <summary>
    /// Gets or sets the names of associated scopes.
    /// </summary>
    /// <value>
    /// A collection of strings representing the associated scope names.
    /// </value>
    [JsonProperty("scopes")]
    public IEnumerable<string> Scopes { get; set; }

    /// <summary>
    /// Gets or sets the associated scope data.
    /// </summary>
    /// <value>
    /// A collection of <see cref="KcClientResourceScope"/> representing the scope data.
    /// </value>
    [JsonProperty("scopesData")]
    public IEnumerable<KcClientResourceScope> ScopesData { get; set; }

    /// <summary>
    /// Gets or sets the type of this policy.
    /// </summary>
    /// <value>
    /// A string representing the policy type.
    /// </value>
    [JsonProperty("type")]
    public string Type { get; set; }
}
