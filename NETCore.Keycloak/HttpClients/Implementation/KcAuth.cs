using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Constants;
using NETCore.Keycloak.HttpClients.Abstraction;
using NETCore.Keycloak.Models;
using NETCore.Keycloak.Models.Auth;
using NETCore.Keycloak.Models.Tokens;
using NETCore.Keycloak.Requests;
using NETCore.Keycloak.Utils;

namespace NETCore.Keycloak.HttpClients.Implementation;

/// <inheritdoc cref="IKcAuth"/>
public class KcAuth : KcClientValidator, IKcAuth
{
    /// <summary>
    /// Logger <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Keycloak base URL
    /// </summary>
    private readonly string _baseUrl;

    /// <summary>
    /// Auth client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcAuth(string baseUrl, ILogger logger = null)
    {
        if ( string.IsNullOrWhiteSpace(baseUrl) )
        {
            throw new KcException($"{nameof(baseUrl)} is required");
        }

        // Remove last "/" from base url
        _baseUrl = baseUrl.EndsWith("/", StringComparison.Ordinal)
            ? baseUrl.Remove(baseUrl.Length - 1, 1)
            : baseUrl;

        _logger = logger;
    }

    /// <inheritdoc cref="IKcAuth.GetClientCredentialsTokenAsync"/>
    public async Task<KcResponse<KcIdentityProviderToken>> GetClientCredentialsTokenAsync(
        string realm,
        KcClientCredentials clientCredentials, CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new KcException($"{nameof(realm)} is required");
        }

        if ( clientCredentials == null )
        {
            throw new KcException($"{nameof(clientCredentials)} is required");
        }

        clientCredentials.Validate();

        using var client = new HttpClient();

        var tokenEndpoint = $"{_baseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenEndpoint}";

        try
        {
            using var form = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {
                        "client_id", clientCredentials.ClientId
                    },
                    {
                        "client_secret", clientCredentials.Secret
                    },
                    {
                        "grant_type", "client_credentials"
                    }
                });

            var tokenRequest =
                await client.PostAsync(new Uri(tokenEndpoint), form, cancellationToken)
                    .ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcIdentityProviderToken>(tokenRequest,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get client credentials token", e);
            }

            return new KcResponse<KcIdentityProviderToken>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.GetResourceOwnerPasswordTokenAsync"/>
    public async Task<KcResponse<KcIdentityProviderToken>> GetResourceOwnerPasswordTokenAsync(
        string realm,
        KcClientCredentials clientCredentials, KcUserLogin userLogin, string scope = null,
        string resource = null, CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new KcException($"{nameof(realm)} is required");
        }

        if ( clientCredentials == null )
        {
            throw new KcException($"{nameof(clientCredentials)} is required");
        }

        clientCredentials.Validate();

        if ( userLogin == null )
        {
            throw new KcException($"{nameof(userLogin)} is required");
        }

        userLogin.Validate();

        var tokenEndpoint = $"{_baseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenEndpoint}";

        using var client = new HttpClient();
        try
        {
            var formData = new Dictionary<string, string>
            {
                {
                    "client_id", clientCredentials.ClientId
                },
                {
                    "client_secret", clientCredentials.Secret
                },
                {
                    "grant_type", "password"
                },
                {
                    "username", userLogin.Username
                },
                {
                    "password", userLogin.Password
                }
            };

            if ( !string.IsNullOrWhiteSpace(scope) )
            {
                formData.Add("scope", scope);
            }

            if ( !string.IsNullOrWhiteSpace(resource) )
            {
                formData.Add("resource", resource);
            }

            using var form = new FormUrlEncodedContent(formData);

            var tokenRequest =
                await client.PostAsync(new Uri(tokenEndpoint), form, cancellationToken)
                    .ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcIdentityProviderToken>(tokenRequest,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get resource owner password token", e);
            }

            return new KcResponse<KcIdentityProviderToken>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.ValidatePasswordAsync"/>
    public async Task<KcResponse<bool>> ValidatePasswordAsync(string realm,
        KcClientCredentials clientCredentials,
        KcUserLogin userLogin, CancellationToken cancellationToken = default)
    {
        if ( await GetResourceOwnerPasswordTokenAsync(realm, clientCredentials, userLogin,
                cancellationToken: cancellationToken).ConfigureAwait(false)
            is not { } tokenResponse )
        {
            return new KcResponse<bool>();
        }

        if ( tokenResponse.IsError )
        {
            return new KcResponse<bool>
            {
                Exception = tokenResponse.Exception,
                IsError = string.IsNullOrWhiteSpace(tokenResponse.ErrorMessage),
                Response = false,
                ErrorMessage = tokenResponse.ErrorMessage
            };
        }

        if ( !string.IsNullOrWhiteSpace(tokenResponse.Response?.AccessToken) )
        {
            _ = await RevokeAccessTokenAsync(realm, clientCredentials,
                tokenResponse.Response.AccessToken,
                cancellationToken).ConfigureAwait(false);
        }

        if ( !string.IsNullOrWhiteSpace(tokenResponse.Response?.RefreshToken) )
        {
            _ = await RevokeRefreshTokenAsync(realm, clientCredentials,
                tokenResponse.Response.RefreshToken,
                cancellationToken).ConfigureAwait(false);
        }

        return new KcResponse<bool>
        {
            Response = true
        };
    }

    /// <inheritdoc cref="IKcAuth.RefreshAccessTokenAsync"/>
    public async Task<KcResponse<KcIdentityProviderToken>> RefreshAccessTokenAsync(string realm,
        KcClientCredentials clientCredentials, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new KcException($"{nameof(realm)} is required");
        }

        if ( string.IsNullOrWhiteSpace(refreshToken) )
        {
            throw new KcException($"{nameof(refreshToken)} is required");
        }

        if ( clientCredentials == null )
        {
            throw new KcException($"{nameof(clientCredentials)} is required");
        }

        clientCredentials.Validate();

        var tokenEndpoint = $"{_baseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenEndpoint}";

        using var client = new HttpClient();
        try
        {
            using var form = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {
                        "client_id", clientCredentials.ClientId
                    },
                    {
                        "client_secret", clientCredentials.Secret
                    },
                    {
                        "grant_type", "refresh_token"
                    },
                    {
                        "refresh_token", refreshToken
                    }
                });

            var tokenRequest =
                await client.PostAsync(new Uri(tokenEndpoint), form, cancellationToken)
                    .ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcIdentityProviderToken>(tokenRequest,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to refresh token", e);
            }

            return new KcResponse<KcIdentityProviderToken>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.RevokeAccessTokenAsync"/>
    public async Task<KcResponse<bool>> RevokeAccessTokenAsync(string realm,
        KcClientCredentials clientCredentials, string accessToken,
        CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new KcException($"{nameof(realm)} is required");
        }

        if ( string.IsNullOrWhiteSpace(accessToken) )
        {
            return new KcResponse<bool>();
        }

        if ( clientCredentials == null )
        {
            throw new KcException($"{nameof(clientCredentials)} is required");
        }

        clientCredentials.Validate();

        var tokenRevocationEndpoint =
            $"{_baseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenRevocationEndpoint}";

        using var client = new HttpClient();
        try
        {
            using var form = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {
                        "client_id", clientCredentials.ClientId
                    },
                    {
                        "client_secret", clientCredentials.Secret
                    },
                    {
                        "token", accessToken
                    },
                    {
                        "token_type_hint", "access_token"
                    }
                });

            var tokenRevocationRequest = await client.PostAsync(new Uri(tokenRevocationEndpoint),
                form, cancellationToken).ConfigureAwait(false);

            return new KcResponse<bool>
            {
                Response = tokenRevocationRequest.IsSuccessStatusCode,
                IsError = !tokenRevocationRequest.IsSuccessStatusCode,
                ErrorMessage =
                    await tokenRevocationRequest.Content.ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false)
            };
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to revoke access token", e);
            }

            return new KcResponse<bool>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.RevokeRefreshTokenAsync"/>
    public async Task<KcResponse<bool>> RevokeRefreshTokenAsync(string realm,
        KcClientCredentials clientCredentials,
        string refreshToken, CancellationToken cancellationToken = default)
    {
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new KcException($"{nameof(realm)} is required");
        }

        if ( string.IsNullOrWhiteSpace(refreshToken) )
        {
            return new KcResponse<bool>();
        }

        if ( clientCredentials == null )
        {
            throw new KcException($"{nameof(clientCredentials)} is required");
        }

        clientCredentials.Validate();

        var tokenRevocationEndpoint =
            $"{_baseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenRevocationEndpoint}";

        using var client = new HttpClient();
        try
        {
            using var form = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {
                        "client_id", clientCredentials.ClientId
                    },
                    {
                        "client_secret", clientCredentials.Secret
                    },
                    {
                        "token", refreshToken
                    },
                    {
                        "token_type_hint", "refresh_token"
                    }
                });

            var tokenRevocationRequest = await client.PostAsync(new Uri(tokenRevocationEndpoint),
                form, cancellationToken).ConfigureAwait(false);

            return new KcResponse<bool>
            {
                Response = tokenRevocationRequest.IsSuccessStatusCode,
                IsError = !tokenRevocationRequest.IsSuccessStatusCode,
                ErrorMessage =
                    await tokenRevocationRequest.Content.ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false)
            };
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to revoke refresh token", e);
            }

            return new KcResponse<bool>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.GetRequestPartyTokenAsync"/>
    public async Task<KcResponse<KcIdentityProviderToken>> GetRequestPartyTokenAsync(string realm,
        string accessToken, string audience, IEnumerable<string> permissions = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(audience) )
        {
            throw new KcException($"{nameof(audience)} is required");
        }

        var tokenEndpoint = $"{_baseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenEndpoint}";

        using var client = new HttpClient();

        _ = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
            $"Bearer {accessToken}");

        try
        {
            var formData = new Dictionary<string, string>
            {
                {
                    "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket"
                },
                {
                    "audience", audience
                }
            };

            var permissionList = permissions?.ToList();

            if ( permissionList != null && permissionList.Count != 0 )
            {
                foreach ( var permission in permissionList )
                {
                    formData.Add("permission", permission);
                }
            }

            using var form = new FormUrlEncodedContent(formData);

            var tokenRequest = await client.PostAsync(new Uri(tokenEndpoint), form,
                    cancellationToken)
                .ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcIdentityProviderToken>(tokenRequest,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to get resource owner password token", e);
            }

            return new KcResponse<KcIdentityProviderToken>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
