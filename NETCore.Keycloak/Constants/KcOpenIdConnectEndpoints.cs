namespace NETCore.Keycloak.Constants;

/// <summary>
/// Provides constants for Keycloak OpenID Connect endpoints.
/// </summary>
public struct KcOpenIdConnectEndpoints
{
    /// <summary>
    /// The endpoint for getting tokens.
    /// </summary>
    public const string TokenEndpoint = "protocol/openid-connect/token";

    /// <summary>
    /// The endpoint for revoking tokens.
    /// </summary>
    public const string TokenRevocationEndpoint = "protocol/openid-connect/revoke";
}
