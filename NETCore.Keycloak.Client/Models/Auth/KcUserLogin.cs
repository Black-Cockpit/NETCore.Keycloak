using NETCore.Keycloak.Client.Exceptions;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Auth;

/// <summary>
/// Represents the login credentials for a Keycloak user.
/// </summary>
public class KcUserLogin
{
    /// <summary>
    /// Gets or sets the username of the Keycloak user.
    /// </summary>
    /// <value>
    /// A string representing the user's username.
    /// </value>
    [JsonProperty("username")]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password of the Keycloak user.
    /// </summary>
    /// <value>
    /// A string representing the user's password.
    /// </value>
    [JsonProperty("password")]
    public string Password { get; set; }

    /// <summary>
    /// Validates the Keycloak user login credentials.
    /// </summary>
    /// <exception cref="KcException">
    /// Thrown if either <see cref="Username"/> or <see cref="Password"/> is null, empty, or consists only of whitespace.
    /// </exception>
    public void Validate()
    {
        // Check if the username is null, empty, or consists only of whitespace.
        if ( string.IsNullOrWhiteSpace(Username) )
        {
            throw new KcException($"{nameof(Username)} is required");
        }

        // Check if the password is null, empty, or consists only of whitespace.
        if ( string.IsNullOrWhiteSpace(Password) )
        {
            throw new KcException($"{nameof(Password)} is required");
        }
    }
}
