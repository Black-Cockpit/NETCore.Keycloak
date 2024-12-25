using System.Collections.ObjectModel;
using NETCore.Keycloak.Models.Common;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Users;

/// <summary>
/// Keycloak user representation.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_userrepresentation"/>
/// </summary>
public class KcUser
{
    /// <summary>
    /// User access
    /// </summary>
    [JsonProperty("access")]
    public KcUserAccess Access { get; set; }

    /// <summary>
    /// User attributes
    /// </summary>
    [JsonProperty("attributes")]
    public IDictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// User consents to clients <see cref="KcUserConsent"/>
    /// </summary>
    [JsonProperty("clientConsents")]
    public IEnumerable<KcUserConsent> ClientConsents { get; set; }

    /// <summary>
    /// User clients roles
    /// </summary>
    [JsonProperty("clientRoles")]
    public IDictionary<string, object> ClientRoles { get; set; }

    /// <summary>
    /// User account creation time stamp
    /// </summary>
    [JsonProperty("createdTimestamp")]
    public long CreatedTimestamp { get; set; }

    /// <summary>
    /// User credentials <see cref="KcCredentials"/>
    /// </summary>
    [JsonProperty("credentials")]
    public IEnumerable<KcCredentials> Credentials { get; set; }

    /// <summary>
    /// Credentials type
    /// </summary>
    [JsonProperty("disableableCredentialTypes")]
    public ReadOnlyCollection<string> DisableableCredentialTypes { get; set; }

    /// <summary>
    /// User email
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; }

    /// <summary>
    /// Indicates if the user email is verified
    /// </summary>
    [JsonProperty("emailVerified")]
    public bool? EmailVerified { get; set; }

    /// <summary>
    /// Indicates it the user account is enabled
    /// </summary>
    [JsonProperty("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>
    /// User federated identity <see cref="KcFederatedIdentity"/>
    /// </summary>
    [JsonProperty("federatedIdentities")]
    public IEnumerable<KcFederatedIdentity> FederatedIdentities { get; set; }

    /// <summary>
    /// User federation link
    /// </summary>
    [JsonProperty("federationLink")]
    public string FederationLink { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    /// <summary>
    /// Groups where the user has membership.
    /// </summary>
    [JsonProperty("groups")]
    public IEnumerable<string> Groups { get; set; }

    /// <summary>
    /// Keycloak user id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Last name
    /// </summary>
    [JsonProperty("lastName")]
    public string LastName { get; set; }

    /// <summary>
    /// Not before
    /// </summary>
    [JsonProperty("notBefore")]
    public int? NotBefore { get; set; }

    /// <summary>
    /// User origin
    /// </summary>
    [JsonProperty("origin")]
    public string Origin { get; set; }

    /// <summary>
    /// Assigned roles to user
    /// </summary>
    [JsonProperty("realmRoles")]
    public IEnumerable<string> RealmRoles { get; set; }

    /// <summary>
    /// Require an action when the user logs in. 'Verify email' sends an email to the user to verify their email address. 'Update profile' requires user to enter in new personal information.
    /// 'Update password' requires user to enter in a new password. 'Configure OTP' requires setup of a mobile password generator.
    /// </summary>
    [JsonProperty("requiredActions")]
    public ReadOnlyCollection<string> RequiredActions { get; set; }

    /// <summary>
    /// User self
    /// </summary>
    [JsonProperty("self")]
    public string Self { get; set; }

    /// <summary>
    /// Service account id
    /// </summary>
    [JsonProperty("serviceAccountClientId")]
    public string ServiceAccountClientId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    [JsonProperty("username")]
    public string UserName { get; set; }
}
