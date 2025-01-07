using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.Constants;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Auth;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Tokens;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcAuth"/>
internal sealed class KcAuth(string baseUrl,
    ILogger logger) : KcHttpClientBase(logger, baseUrl), IKcAuth
{
    /// <inheritdoc cref="IKcAuth.GetClientCredentialsTokenAsync"/>
    public async Task<KcResponse<KcIdentityProviderToken>> GetClientCredentialsTokenAsync(
        string realm,
        KcClientCredentials clientCredentials,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm is not null or empty.
        ValidateRequiredString(nameof(realm), realm);

        // Validate that the client credentials are provided.
        ValidateNotNull(nameof(clientCredentials), clientCredentials);

        // Perform validation on the client credentials object.
        clientCredentials.Validate();

        // Construct the token endpoint URL for the realm.
        var tokenEndpoint = $"{BaseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenEndpoint}";

        try
        {
            // Execute the HTTP POST request to get the client credentials token.
            using var tokenRequest = await ExecuteRequest(async () =>
            {
                // Initialize the HTTP client for the request.
                using var client = new HttpClient();

                // Create the form content with the client credentials grant type.
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

                // Send the POST request to the token endpoint with the form content.
                return await client.PostAsync(new Uri(tokenEndpoint), form, cancellationToken)
                    .ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = tokenEndpoint, // Log the token endpoint URL for monitoring.
                HttpMethod = HttpMethod.Post // Log the HTTP method for monitoring.
            }).ConfigureAwait(false);

            // Handle the response and deserialize it into a KcIdentityProviderToken object.
            return await HandleAsync<KcIdentityProviderToken>(tokenRequest, cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            // Log the error if a logger is available.
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, "Unable to get client credentials token", e);
            }

            // Return a response indicating an error occurred.
            return new KcResponse<KcIdentityProviderToken>
            {
                IsError = true,
                Exception = e,
                ErrorMessage = e.Message,
                MonitoringMetrics = new KcHttpApiMonitoringMetrics
                {
                    HttpMethod = HttpMethod.Post,
                    Error = e.Message,
                    Url = new Uri(tokenEndpoint),
                    RequestException = e
                }
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.GetResourceOwnerPasswordTokenAsync"/>
    public async Task<KcResponse<KcIdentityProviderToken>> GetResourceOwnerPasswordTokenAsync(
        string realm,
        KcClientCredentials clientCredentials,
        KcUserLogin userLogin,
        string scope = null,
        string resource = null,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm is not null or empty.
        ValidateRequiredString(nameof(realm), realm);

        // Validate that the client credentials are provided.
        ValidateNotNull(nameof(clientCredentials), clientCredentials);

        // Perform validation on the client credentials object.
        clientCredentials.Validate();

        // Validate that the user login information is provided.
        ValidateNotNull(nameof(userLogin), userLogin);

        // Perform validation on the user login object.
        userLogin.Validate();

        // Construct the token endpoint URL for the realm.
        var tokenEndpoint = $"{BaseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenEndpoint}";

        try
        {
            // Execute the HTTP POST request to get the resource owner password token.
            using var tokenRequest = await ExecuteRequest(async () =>
            {
                // Initialize the HTTP client for the request.
                using var client = new HttpClient();

                // Prepare the form data for the request, including client credentials and user login details.
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

                // Add the scope to the form data if it is specified.
                if ( !string.IsNullOrWhiteSpace(scope) )
                {
                    formData.Add("scope", scope);
                }

                // Add the resource to the form data if it is specified.
                if ( !string.IsNullOrWhiteSpace(resource) )
                {
                    formData.Add("resource", resource);
                }

                // Create the form content with the prepared data.
                using var form = new FormUrlEncodedContent(formData);

                // Send the POST request to the token endpoint with the form content.
                return await client.PostAsync(new Uri(tokenEndpoint), form, cancellationToken)
                    .ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = tokenEndpoint, // Log the token endpoint URL for monitoring.
                HttpMethod = HttpMethod.Post // Log the HTTP method for monitoring.
            }).ConfigureAwait(false);

            // Handle the response and deserialize it into a KcIdentityProviderToken object.
            return await HandleAsync<KcIdentityProviderToken>(tokenRequest, cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            // Log the error if a logger is available.
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, "Unable to get resource owner password token", e);
            }

            // Return a response indicating an error occurred.
            return new KcResponse<KcIdentityProviderToken>
            {
                IsError = true,
                Exception = e,
                ErrorMessage = e.Message,
                MonitoringMetrics = new KcHttpApiMonitoringMetrics
                {
                    HttpMethod = HttpMethod.Post,
                    Error = e.Message,
                    Url = new Uri(tokenEndpoint),
                    RequestException = e
                }
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.ValidatePasswordAsync"/>
    public async Task<KcOperationResponse<bool>> ValidatePasswordAsync(
        string realm,
        KcClientCredentials clientCredentials,
        KcUserLogin userLogin,
        CancellationToken cancellationToken = default)
    {
        // Attempt to retrieve an access token using the provided credentials.
        if ( await GetResourceOwnerPasswordTokenAsync(realm, clientCredentials, userLogin,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false) is not { } tokenResponse )
        {
            return new KcOperationResponse<bool>();
        }

        // Initialize the operation response and add token monitoring metrics.
        var response = new KcOperationResponse<bool>();
        response.MonitoringMetrics.Add(tokenResponse.MonitoringMetrics);

        // Check if the token response indicates an error.
        if ( tokenResponse.IsError )
        {
            response.ErrorMessage = tokenResponse.ErrorMessage; // Capture the error message.
            response.Response = false; // Indicate a failed validation.
            response.Exception = tokenResponse.Exception; // Capture the exception, if any.
            response.IsError =
                string.IsNullOrWhiteSpace(tokenResponse
                    .ErrorMessage); // Mark response as error if no message is provided.

            return response;
        }

        // If an access token is available, revoke it.
        if ( !string.IsNullOrWhiteSpace(tokenResponse.Response?.AccessToken) )
        {
            var revokeTokenResponse = await RevokeAccessTokenAsync(realm, clientCredentials,
                tokenResponse.Response.AccessToken, cancellationToken).ConfigureAwait(false);

            // Add monitoring metrics for the token revocation operation.
            response.MonitoringMetrics.Add(revokeTokenResponse.MonitoringMetrics);
        }

        // If a refresh token is available, revoke it.
        if ( !string.IsNullOrWhiteSpace(tokenResponse.Response?.RefreshToken) )
        {
            var revokeRefreshTokenResponse = await RevokeRefreshTokenAsync(realm, clientCredentials,
                tokenResponse.Response.RefreshToken, cancellationToken).ConfigureAwait(false);

            // Add monitoring metrics for the refresh token revocation operation.
            response.MonitoringMetrics.Add(revokeRefreshTokenResponse.MonitoringMetrics);
        }

        // Mark the password validation as successful.
        response.Response = true;

        return response;
    }

    /// <inheritdoc cref="IKcAuth.RefreshAccessTokenAsync"/>
    public async Task<KcResponse<KcIdentityProviderToken>> RefreshAccessTokenAsync(
        string realm,
        KcClientCredentials clientCredentials,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm is not null or empty.
        ValidateRequiredString(nameof(realm), realm);

        // Validate that the refresh token is not null or empty.
        ValidateRequiredString(nameof(refreshToken), refreshToken);

        // Validate that the client credentials are provided.
        ValidateNotNull(nameof(clientCredentials), clientCredentials);

        // Perform validation on the client credentials object.
        clientCredentials.Validate();

        // Construct the token endpoint URL for the realm.
        var tokenEndpoint = $"{BaseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenEndpoint}";

        try
        {
            // Execute the HTTP POST request to refresh the access token.
            using var refreshTokenResponse = await ExecuteRequest(async () =>
            {
                // Initialize the HTTP client for the request.
                using var client = new HttpClient();

                // Prepare the form data for the refresh token request.
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

                // Send the POST request to the token endpoint with the form content.
                return await client.PostAsync(new Uri(tokenEndpoint), form, cancellationToken)
                    .ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = tokenEndpoint, // Log the token endpoint URL for monitoring.
                HttpMethod = HttpMethod.Post // Log the HTTP method for monitoring.
            }).ConfigureAwait(false);

            // Handle the response and deserialize it into a KcIdentityProviderToken object.
            return await HandleAsync<KcIdentityProviderToken>(refreshTokenResponse, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            // Log the error if a logger is available.
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, "Unable to refresh token", e);
            }

            // Return a response indicating an error occurred.
            return new KcResponse<KcIdentityProviderToken>
            {
                IsError = true,
                Exception = e,
                ErrorMessage = e.Message,
                MonitoringMetrics = new KcHttpApiMonitoringMetrics
                {
                    HttpMethod = HttpMethod.Post,
                    Error = e.Message,
                    Url = new Uri(tokenEndpoint),
                    RequestException = e
                }
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.RevokeAccessTokenAsync"/>
    public async Task<KcResponse<bool>> RevokeAccessTokenAsync(
        string realm,
        KcClientCredentials clientCredentials,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm is not null or empty.
        ValidateRequiredString(nameof(realm), realm);

        // Return an empty response if the access token is null or empty.
        ValidateRequiredString(nameof(accessToken), accessToken);

        // Validate that the client credentials are provided.
        ValidateNotNull(nameof(clientCredentials), clientCredentials);

        // Perform validation on the client credentials object.
        clientCredentials.Validate();

        // Construct the token revocation endpoint URL for the realm.
        var tokenRevocationEndpoint = $"{BaseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenRevocationEndpoint}";

        try
        {
            // Execute the HTTP POST request to revoke the access token.
            using var tokenRevocationResponse = await ExecuteRequest(async () =>
            {
                // Initialize the HTTP client for the request.
                using var client = new HttpClient();

                // Prepare the form data for the token revocation request.
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

                // Send the POST request to the token revocation endpoint with the form content.
                return await client.PostAsync(new Uri(tokenRevocationEndpoint), form, cancellationToken)
                    .ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = tokenRevocationEndpoint, // Log the token revocation endpoint URL for monitoring.
                HttpMethod = HttpMethod.Post // Log the HTTP method for monitoring.
            }).ConfigureAwait(false);

            // Construct and return the response object.
            return new KcResponse<bool>
            {
                Response = tokenRevocationResponse?.ResponseMessage?.IsSuccessStatusCode == true,
                IsError = tokenRevocationResponse?.ResponseMessage?.IsSuccessStatusCode != true,
                ErrorMessage = tokenRevocationResponse?.ResponseMessage != null
                    ? await tokenRevocationResponse.ResponseMessage.Content.ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false)
                    : null,
                MonitoringMetrics = await KcHttpApiMonitoringMetrics
                    .MapFromHttpRequestExecutionResult(tokenRevocationResponse, cancellationToken)
                    .ConfigureAwait(false)
            };
        }
        catch ( Exception e )
        {
            // Log the error if a logger is available.
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, "Unable to revoke access token", e);
            }

            // Return a response indicating an error occurred.
            return new KcResponse<bool>
            {
                IsError = true,
                Exception = e,
                ErrorMessage = e.Message,
                MonitoringMetrics = new KcHttpApiMonitoringMetrics
                {
                    HttpMethod = HttpMethod.Post,
                    Error = e.Message,
                    Url = new Uri(tokenRevocationEndpoint),
                    RequestException = e
                }
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.RevokeRefreshTokenAsync"/>
    public async Task<KcResponse<bool>> RevokeRefreshTokenAsync(
        string realm,
        KcClientCredentials clientCredentials,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // Validate that the realm is not null or empty.
        ValidateRequiredString(nameof(realm), realm);

        // Return an empty response if the refresh token is null or empty.
        ValidateRequiredString(nameof(refreshToken), refreshToken);

        // Validate that the client credentials are provided.
        ValidateNotNull(nameof(clientCredentials), clientCredentials);

        // Perform validation on the client credentials object.
        clientCredentials.Validate();

        // Construct the token revocation endpoint URL for the realm.
        var tokenRevocationEndpoint = $"{BaseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenRevocationEndpoint}";

        try
        {
            // Execute the HTTP POST request to revoke the refresh token.
            using var tokenRevocationResponse = await ExecuteRequest(async () =>
            {
                // Initialize the HTTP client for the request.
                using var client = new HttpClient();

                // Prepare the form data for the token revocation request.
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

                // Send the POST request to the token revocation endpoint with the form content.
                return await client.PostAsync(new Uri(tokenRevocationEndpoint), form, cancellationToken)
                    .ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = tokenRevocationEndpoint, // Log the token revocation endpoint URL for monitoring.
                HttpMethod = HttpMethod.Post // Log the HTTP method for monitoring.
            }).ConfigureAwait(false);

            // Construct and return the response object.
            return new KcResponse<bool>
            {
                Response = tokenRevocationResponse?.ResponseMessage?.IsSuccessStatusCode == true,
                IsError = tokenRevocationResponse?.ResponseMessage?.IsSuccessStatusCode != true,
                ErrorMessage = tokenRevocationResponse?.ResponseMessage != null
                    ? await tokenRevocationResponse.ResponseMessage.Content.ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false)
                    : null,
                MonitoringMetrics = await KcHttpApiMonitoringMetrics
                    .MapFromHttpRequestExecutionResult(tokenRevocationResponse, cancellationToken)
                    .ConfigureAwait(false)
            };
        }
        catch ( Exception e )
        {
            // Log the error if a logger is available.
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, "Unable to revoke refresh token", e);
            }

            // Return a response indicating an error occurred.
            return new KcResponse<bool>
            {
                IsError = true,
                Exception = e,
                ErrorMessage = e.Message,
                MonitoringMetrics = new KcHttpApiMonitoringMetrics
                {
                    HttpMethod = HttpMethod.Post,
                    Error = e.Message,
                    Url = new Uri(tokenRevocationEndpoint),
                    RequestException = e
                }
            };
        }
    }

    /// <inheritdoc cref="IKcAuth.GetRequestPartyTokenAsync"/>
    public async Task<KcResponse<KcIdentityProviderToken>> GetRequestPartyTokenAsync(
        string realm,
        string accessToken,
        string audience,
        IEnumerable<string> permissions = null,
        CancellationToken cancellationToken = default)
    {
        // Validate the realm and access token inputs.
        ValidateAccess(realm, accessToken);

        // Validate that the audience is not null or empty.
        ValidateRequiredString(nameof(audience), audience);

        // Construct the token endpoint URL for the realm.
        var tokenEndpoint = $"{BaseUrl}/{realm}/{KcOpenIdConnectEndpoints.TokenEndpoint}";

        try
        {
            // Execute the HTTP POST request to retrieve the Request Party Token.
            using var tokenRequest = await ExecuteRequest(async () =>
            {
                // Initialize the HTTP client for the request.
                using var client = new HttpClient();

                // Add the Authorization header with the provided access token.
                _ = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                // Prepare the form data for the RPT request.
                var formData = new Dictionary<string, string>
                {
                    {
                        "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket"
                    }, // Specify the UMA grant type.
                    {
                        "audience", audience
                    } // Specify the audience.
                };

                // Convert the permissions collection to a list, if provided.
                var permissionList = permissions?.ToList();

                // Add each permission to the form data if permissions are specified.
                if ( permissionList != null && permissionList.Count != 0 )
                {
                    foreach ( var permission in permissionList )
                    {
                        formData.Add("permission", permission);
                    }
                }

                // Create the form content with the prepared data.
                using var form = new FormUrlEncodedContent(formData);

                // Send the POST request to the token endpoint with the form content.
                return await client.PostAsync(new Uri(tokenEndpoint), form, cancellationToken).ConfigureAwait(false);
            }, new KcHttpMonitoringFallbackModel
            {
                Url = tokenEndpoint, // Log the token endpoint URL for monitoring.
                HttpMethod = HttpMethod.Post // Log the HTTP method for monitoring.
            }).ConfigureAwait(false);

            // Handle the response and deserialize it into a KcIdentityProviderToken object.
            return await HandleAsync<KcIdentityProviderToken>(tokenRequest, cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            // Log the error if a logger is available.
            if ( Logger != null )
            {
                KcLoggerMessages.Error(Logger, "Unable to get resource owner password token", e);
            }

            // Return a response indicating an error occurred.
            return new KcResponse<KcIdentityProviderToken>
            {
                IsError = true,
                Exception = e,
                ErrorMessage = e.Message,
                MonitoringMetrics = new KcHttpApiMonitoringMetrics
                {
                    HttpMethod = HttpMethod.Post,
                    Error = e.Message,
                    Url = new Uri(tokenEndpoint),
                    RequestException = e
                }
            };
        }
    }
}
