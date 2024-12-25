using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Serialization;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Clients;

/// <summary>
/// Keycloak client representation.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_clientrepresentation"/>
/// </summary>
public class KcClient
{
    /// <summary>
    /// The registration access token provides access for clients to the client registration service.
    /// <see cref="KcClientAccess"/>
    /// </summary>
    [JsonProperty("access")]
    public KcClientAccess Access { get; set; }

    /// <summary>
    /// URL to the admin interface of the client. Set this if the client supports the adapter REST API.
    /// This REST API allows the auth server to push revocation policies and other administrative tasks.
    /// Usually this is set to the base URL of the client.
    /// </summary>
    [JsonProperty("adminUrl")]
    public string AdminUrl { get; set; }

    /// <summary>
    /// Always list this client in the Account Console, even if the user does not have an active session.
    /// </summary>
    [JsonProperty("alwaysDisplayInConsole")]
    public bool? AlwaysDisplayInConsole { get; set; }

    /// <summary>
    /// A list of attributes that can be given to the client.
    /// </summary>
    [JsonProperty("attributes")]
    public IDictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Override realm authentication flow bindings.
    /// </summary>
    [JsonProperty("authenticationFlowBindingOverrides")]
    public IDictionary<string, object> AuthenticationFlowBindingOverrides { get; set; }

    /// <summary>
    /// Enable/Disable fine-grained authorization support for a client.
    /// </summary>
    [JsonProperty("authorizationServicesEnabled")]
    public bool? AuthorizationServicesEnabled { get; set; }

    /// <summary>
    /// Authorization setting <see cref="KcClientResourceServer"/>
    /// </summary>
    [JsonProperty("authorizationSettings")]
    public KcClientResourceServer AuthorizationSettings { get; set; }

    /// <summary>
    /// Default URL to use when the auth server needs to redirect or link back to the client.
    /// </summary>
    [JsonProperty("baseUrl")]
    public string BaseUrl { get; set; }

    /// <summary>
    /// Indicates that the access type of this client is bearer-only.
    /// </summary>
    [JsonProperty("bearerOnly")]
    public bool? BearerOnly { get; set; }

    /// <summary>
    /// Indicates how do clients authenticate with the auth server.
    /// Either client-secret or client-jwt can be chosen.
    /// When using client-secret, the module parameter secret can set it,while for client-jwt, you can use the keys use.jwks.url, jwks.url,
    /// and jwt.credential.certificate in the attributes module parameter to configure its behavior.
    /// Allowed values are:
    /// - "client-secret"
    /// - "client-jwt"
    /// </summary>
    [JsonProperty("clientAuthenticatorType")]
    [JsonConverter(typeof(KcClientAuthenticatorTypeConverter))]
    public KcClientAuthenticatorType ClientAuthenticatorType { get; set; } =
        KcClientAuthenticatorType.Secret;

    /// <summary>
    /// Specifies ID referenced in URI and tokens. For example 'my-client'.
    /// For SAML this is also the expected issuer value from authn requests
    /// </summary>
    [JsonProperty("clientId")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// If enabled, users have to consent to client access.
    /// </summary>
    [JsonProperty("consentRequired")]
    public bool? ConsentRequired { get; set; }

    /// <summary>
    /// List of default client scopes.
    /// </summary>
    [JsonProperty("defaultClientScopes")]
    public IEnumerable<string> DefaultClientScopes { get; set; }

    /// <summary>
    /// Specifies description of the client. For example 'My Client for TimeSheets'.
    /// Supports keys for localized values as well. For example: ${my_client_description}
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// This enables support for Direct Access Grants, which means that client has access to username/password of user and exchange it directly with Keycloak server for access token.
    /// In terms of OAuth2 specification, this enables support of 'Resource Owner Password Credentials Grant' for this client.
    /// <see href="https://oauth.net/2/grant-types/password/"/>
    /// </summary>
    [JsonProperty("directAccessGrantsEnabled")]
    public bool? DirectAccessGrantsEnabled { get; set; }

    /// <summary>
    /// Indicates where the client is enabled or not.
    /// Disabled clients cannot initiate a login or have obtained access tokens.
    /// </summary>
    [JsonProperty("enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// When true, logout requires a browser redirect to client.
    /// When false, server performs a background invocation for logout.
    /// </summary>
    [JsonProperty("frontchannelLogout")]
    public bool? FrontChannelLogout { get; set; }

    /// <summary>
    /// Indicates if the full scope is allowed
    /// </summary>
    [JsonProperty("fullScopeAllowed")]
    public bool? FullScopeAllowed { get; set; }

    /// <summary>
    /// Id of client to be worked on. This is usually an UUID.
    /// Either this or client_id is required. If both are specified, this takes precedence.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// This enables support for OpenID Connect redirect based authentication without authorization code.
    /// In terms of OpenID Connect or OAuth2 specifications, this enables support of 'Implicit Flow' for this client.
    /// <see href="https://oauth.net/2/grant-types/implicit/"/>
    /// </summary>
    [JsonProperty("implicitFlowEnabled")]
    public bool? ImplicitFlowEnabled { get; set; }

    /// <summary>
    /// Specifies display name of the client. For example 'My Client'.
    /// Supports keys for localized values as well. For example: ${my_client}
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Cluster node re-registration timeout for this client.
    /// </summary>
    [JsonProperty("nodeReRegistrationTimeout")]
    public int? NodeReregistrationTimeout { get; set; }

    /// <summary>
    /// Revoke any tokens issued before this time for this client.
    /// To push the policy, you should set an effective admin URL in the Settings tab first.
    /// </summary>
    [JsonProperty("notBefore")]
    public int? NotBefore { get; set; }

    /// <summary>
    /// This enables support for OAuth 2.0 Device Authorization Grant, which means that client is
    /// an application on device that has limited input capabilities or lack a suitable browser.
    /// </summary>
    [JsonProperty("oauth2DeviceAuthorizationGrantEnabled")]
    public bool? Oauth2DeviceAuthorizationGrantEnabled { get; set; }

    /// <summary>
    /// List of optional client scopes.
    /// </summary>
    [JsonProperty("optionalClientScopes")]
    public IEnumerable<string> OptionalClientScopes { get; set; }

    /// <summary>
    /// Allowed CORS origins. To permit all origins of Valid Redirect URIs, add '+'.
    /// This does not include the '*' wildcard though. To permit all origins, explicitly add '*'.
    /// </summary>
    [JsonProperty("webOrigins")]
    public IEnumerable<string> WebOrigins { get; set; }

    /// <summary>
    /// The protocol indicates the client type
    /// <see cref="KcProtocol"/>
    /// </summary>
    [JsonProperty("protocol")]
    public KcProtocol Protocol { get; set; } = KcProtocol.OpenidConnect;

    /// <summary>
    /// A list of mappers for the client protocols <see cref="KcClientProtocolMapper"/>
    /// </summary>
    [JsonProperty("protocolMappers")]
    public IEnumerable<KcClientProtocolMapper> ProtocolMappers { get; set; }

    /// <summary>
    /// Indicates if the client is public or confidential
    /// </summary>
    [JsonProperty("publicClient")]
    public bool? PublicClient { get; set; }

    /// <summary>
    /// Valid URI pattern a browser can redirect to after a successful login.
    /// Simple wildcards are allowed such as 'http://example.com/*'. Relative path can be specified too such as /my/relative/path/*.
    /// Relative paths are relative to the client root URL, or if none is specified the auth server root URL is used.
    /// For SAML, you must set valid URI patterns if you are relying on the consumer service URL embedded with the login request.
    /// </summary>
    [JsonProperty("redirectUris")]
    public IEnumerable<string> RedirectUris { get; set; }

    /// <summary>
    /// Register cluster nodes
    /// </summary>
    [JsonProperty("registeredNodes")]
    public object RegisteredNodes { get; set; }

    /// <summary>
    /// Registration access token
    /// </summary>
    [JsonProperty("registrationAccessToken")]
    public string RegistrationAccessToken { get; set; }

    /// <summary>
    /// Root URL appended to relative URLs
    /// </summary>
    [JsonProperty("rootUrl")]
    public string RootUrl { get; set; }

    /// <summary>
    /// Client Secret
    /// </summary>
    [JsonProperty("secret")]
    public string Secret { get; set; }

    /// <summary>
    /// Allows you to authenticate this client to Keycloak and retrieve access token dedicated to this client.
    /// In terms of OAuth2 specification, this enables support of 'Client Credentials Grant' for this client.
    /// </summary>
    [JsonProperty("serviceAccountsEnabled")]
    public bool? ServiceAccountsEnabled { get; set; }

    /// <summary>
    /// This enables standard OpenID Connect redirect based authentication with authorization code.
    /// In terms of OpenID Connect or OAuth2 specifications, this enables support of 'Authorization Code Flow' for this client.
    /// <see href="https://oauth.net/2/grant-types/authorization-code/"/>
    /// </summary>
    [JsonProperty("standardFlowEnabled")]
    public bool? StandardFlowEnabled { get; set; }

    /// <summary>
    /// Indicates whether or not surrogate auth is required.
    /// </summary>
    [JsonProperty("surrogateAuthRequired")]
    public bool? SurrogateAuthRequired { get; set; }
}
