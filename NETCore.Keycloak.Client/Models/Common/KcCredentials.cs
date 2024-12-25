using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Common;

/// <summary>
/// Represents a credential configuration in Keycloak.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_credentialrepresentation"/>
/// </summary>
public class KcCredentials
{
    /// <summary>
    /// Gets or sets the creation date of the credentials, represented as a UNIX timestamp.
    /// </summary>
    /// <value>
    /// A nullable long representing the creation date of the credentials.
    /// </value>
    [JsonProperty("createdDate")]
    public long? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the credential data.
    /// </summary>
    /// <value>
    /// A string containing the credential data.
    /// </value>
    [JsonProperty("credentialData")]
    public string CredentialData { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the credentials.
    /// </summary>
    /// <value>
    /// A string representing the credentials' ID.
    /// </value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the priority of the credentials.
    /// </summary>
    /// <value>
    /// An integer representing the credentials' priority.
    /// </value>
    [JsonProperty("priority")]
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets the secret data associated with the credentials.
    /// </summary>
    /// <value>
    /// A string containing the secret data.
    /// </value>
    [JsonProperty("secretData")]
    public string SecretData { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the credentials are temporary.
    /// If temporary, the user must change the password on the next login.
    /// </summary>
    /// <value>
    /// <c>true</c> if the credentials are temporary; otherwise, <c>false</c>.
    /// </value>
    [JsonProperty("temporary")]
    public bool? Temporary { get; set; }

    /// <summary>
    /// Gets or sets the type of credentials, such as "password" or "otp".
    /// </summary>
    /// <value>
    /// A string representing the credentials' type.
    /// </value>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the user label associated with the credentials.
    /// </summary>
    /// <value>
    /// A string containing the user label.
    /// </value>
    [JsonProperty("userLabel")]
    public string UserLabel { get; set; }

    /// <summary>
    /// Gets or sets the value of the credentials.
    /// </summary>
    /// <value>
    /// A string containing the credentials' value.
    /// </value>
    [JsonProperty("value")]
    public string Value { get; set; }
}
