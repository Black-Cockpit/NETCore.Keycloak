using System.Data;
using Microsoft.AspNetCore.Authorization;
using NETCore.Keycloak.Client.Authorization.Store;

namespace NETCore.Keycloak.Client.Authorization.Requirements;

/// <summary>
/// Represents a Keycloak authorization requirement that specifies the resource and scope needed for authorization.
/// </summary>
/// <remarks>
/// This requirement is used to verify that an incoming request has the necessary access to a specific protected resource and scope.
/// </remarks>
public class KcAuthorizationRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the store that holds the Keycloak protected resources.
    /// </summary>
    /// <value>
    /// An instance of <see cref="KcProtectedResourceStore"/> that contains the list of protected resources and their metadata.
    /// </value>
    public KcProtectedResourceStore ProtectedResourceStore { get; }

    /// <summary>
    /// Gets the name of the protected resource required for authorization.
    /// </summary>
    /// <value>
    /// A string representing the name of the resource being accessed, such as "orders" or "invoices".
    /// </value>
    public string Resource { get; }

    /// <summary>
    /// Gets the scope of access required for the resource.
    /// </summary>
    /// <value>
    /// A string representing the specific scope of the resource, such as "read", "write", or "delete".
    /// </value>
    private string Scope { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KcAuthorizationRequirement"/> class with the specified parameters.
    /// </summary>
    /// <param name="protectedResourceStore">The protected resource store that provides information about available resources and their scopes.</param>
    /// <param name="resource">The name of the resource that requires protection.</param>
    /// <param name="scope">The specific scope of the resource required for authorization.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="protectedResourceStore"/> is null.</exception>
    /// <exception cref="NoNullAllowedException">Thrown if <paramref name="resource"/> or <paramref name="scope"/> is null or whitespace.</exception>
    /// <remarks>
    /// The <paramref name="resource"/> parameter identifies the name of the protected resource, while <paramref name="scope"/>
    /// defines the action or permission level being requested.
    /// </remarks>
    public KcAuthorizationRequirement(KcProtectedResourceStore protectedResourceStore, string resource, string scope)
    {
        // Ensure protected resources is not null
        ArgumentNullException.ThrowIfNull(protectedResourceStore, $"{nameof(protectedResourceStore)} cannot be null.");

        // Ensure resource is not null or empty
        if ( string.IsNullOrWhiteSpace(resource) )
        {
            throw new NoNullAllowedException($"'{nameof(resource)}' cannot be null or whitespace.");
        }

        // Ensure scope is not null or empty
        if ( string.IsNullOrWhiteSpace(scope) )
        {
            throw new NoNullAllowedException($"'{nameof(scope)}' cannot be null or whitespace.");
        }

        ProtectedResourceStore = protectedResourceStore;
        Resource = resource;
        Scope = scope;
    }

    /// <summary>
    /// Returns a string representation of the requirement in the format "Resource#Scope".
    /// </summary>
    /// <returns>
    /// A string that concatenates the <see cref="Resource"/> and <see cref="Scope"/> properties in the format "Resource#Scope".
    /// </returns>
    public override string ToString() => $"{Resource}#{Scope}";
}
