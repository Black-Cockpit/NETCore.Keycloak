using NETCore.Keycloak.Models.Auth;

namespace NETCore.Keycloak.Models.Common;

/// <summary>
/// Represents the configuration for Keycloak realm administration.
/// </summary>
public class KcRealmAdminConfiguration
{
    /// <summary>
    /// Gets or sets the base URL of the Keycloak server.
    /// </summary>
    /// <value>
    /// A string representing the Keycloak base URL.
    /// </value>
    public string KeycloakBaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the name of the Keycloak realm.
    /// </summary>
    /// <value>
    /// A string representing the Keycloak realm name.
    /// </value>
    public string Realm { get; set; }

    /// <summary>
    /// Gets or sets the Keycloak client ID used for authentication.
    /// </summary>
    /// <value>
    /// A string representing the client ID.
    /// </value>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the realm administrator credentials.
    /// </summary>
    /// <value>
    /// An instance of <see cref="KcUserLogin"/> containing the admin credentials.
    /// </value>
    public KcUserLogin RealmAdminCredentials { get; set; }

    /// <summary>
    /// Validates the Keycloak realm administration configuration.
    /// </summary>
    /// <exception cref="KcException">
    /// Thrown if <see cref="KeycloakBaseUrl"/> is null, empty, or consists only of whitespace,
    /// or if <see cref="RealmAdminCredentials"/> is null.
    /// </exception>
    public void Validate()
    {
        // Validate that the Keycloak base URL is not null, empty, or whitespace
        if ( string.IsNullOrWhiteSpace(KeycloakBaseUrl) )
        {
            throw new KcException($"{nameof(KeycloakBaseUrl)} is required");
        }

        // Validate that realm admin credentials are provided
        if ( RealmAdminCredentials == null )
        {
            throw new KcException($"{nameof(RealmAdminCredentials)} is required");
        }
    }
}
