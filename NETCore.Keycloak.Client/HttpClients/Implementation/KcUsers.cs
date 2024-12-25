using System.Globalization;
using Microsoft.Extensions.Logging;
using NETCore.Keycloak.Client.HttpClients.Abstraction;
using NETCore.Keycloak.Client.Models;
using NETCore.Keycloak.Client.Models.Common;
using NETCore.Keycloak.Client.Models.Groups;
using NETCore.Keycloak.Client.Models.Users;
using NETCore.Keycloak.Client.Requests;
using NETCore.Keycloak.Client.Utils;

namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <inheritdoc cref="IKcUsers"/>
public class KcUsers : KcClientValidator, IKcUsers
{
    /// <summary>
    /// Logger <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;

    private readonly string _baseUrl;

    /// <summary>
    /// Users client constructor
    /// </summary>
    /// <param name="baseUrl">Keycloak server base url.
    /// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_uri_scheme"/></param>
    /// <param name="logger">Logger <see cref="ILogger"/></param>
    public KcUsers(string baseUrl, ILogger logger = null)
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

    /// <inheritdoc cref="IKcUsers.CreateAsync"/>
    public async Task<KcResponse<object>> CreateAsync(string realm, string accessToken, KcUser user,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( user == null )
        {
            throw new KcException($"{nameof(user)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/users", accessToken,
                KcRequestHandler.GetBody(user));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to create user", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.CreateAsync"/>
    public async Task<KcResponse<bool>> IsUserExistsByEmailAsync(string realm, string accessToken,
        string email,
        CancellationToken cancellationToken = default)
    {
        var usersList = await ListUserAsync(realm, accessToken, new KcUserFilter
        {
            Email = email,
            Exact = true
        }, cancellationToken).ConfigureAwait(false);

        return usersList.IsError
            ? new KcResponse<bool>
            {
                IsError = usersList.IsError,
                Exception = usersList.Exception,
                ErrorMessage = usersList.ErrorMessage
            }
            : new KcResponse<bool>
            {
                Response = usersList.Response.Any()
            };
    }

    /// <inheritdoc cref="IKcUsers.ListUserAsync"/>
    public async Task<KcResponse<IEnumerable<KcUser>>> ListUserAsync(string realm,
        string accessToken, KcUserFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        filter ??= new KcUserFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcUser>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to list realm {realm} users", e);
            }

            return new KcResponse<IEnumerable<KcUser>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.CountAsync"/>
    public async Task<KcResponse<int>> CountAsync(string realm, string accessToken,
        KcUserFilter filter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        filter ??= new KcUserFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/count{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return response.IsSuccessStatusCode
                ? new KcResponse<int>
                {
                    Response = int.Parse(await response.Content.ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false), CultureInfo.CurrentCulture)
                }
                : new KcResponse<int>
                {
                    IsError = true,
                    ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false)
                };
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to count realm {realm} users", e);
            }

            return new KcResponse<int>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.GetAsync"/>
    public async Task<KcResponse<KcUser>> GetAsync(string realm, string accessToken, string userId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcUser>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to get user {userId}", e);
            }

            return new KcResponse<KcUser>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.UpdateAsync"/>
    public async Task<KcResponse<object>> UpdateAsync(string realm, string accessToken,
        string userId, KcUser user,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        if ( user == null )
        {
            throw new KcException($"{nameof(user)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/users/{userId}",
                accessToken, KcRequestHandler.GetBody(user));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to update user {userId}", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.DeleteAsync"/>
    public async Task<KcResponse<object>> DeleteAsync(string realm, string accessToken,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/users/{userId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to delete user {userId}", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.GetCredentialsAsync"/>
    public async Task<KcResponse<IEnumerable<KcCredentials>>> GetCredentialsAsync(string realm,
        string accessToken,
        string userId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/credentials", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcCredentials>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to get user {userId} credentials", e);
            }

            return new KcResponse<IEnumerable<KcCredentials>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.DeleteCredentialsAsync"/>
    public async Task<KcResponse<object>> DeleteCredentialsAsync(string realm, string accessToken,
        string userId,
        string credentialsId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(credentialsId) )
        {
            throw new KcException($"{nameof(credentialsId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/users/{userId}/credentials/{credentialsId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to delete user {userId} credentials", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.UpdateCredentialsLabelAsync"/>
    public async Task<KcResponse<object>> UpdateCredentialsLabelAsync(string realm,
        string accessToken, string userId,
        string credentialsId, string label, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(credentialsId) )
        {
            throw new KcException($"{nameof(credentialsId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(label) )
        {
            throw new KcException($"{nameof(label)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/users/{userId}/credentials/{credentialsId}/userLabel",
                accessToken, new StringContent(label), "text/plain");

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to update user {userId} credentials label", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.UserGroupsAsync"/>
    public async Task<KcResponse<IEnumerable<KcGroup>>> UserGroupsAsync(string realm,
        string accessToken, string userId,
        KcFilter filter = null, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/groups{filter.BuildQuery()}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcGroup>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to get user {userId} groups", e);
            }

            return new KcResponse<IEnumerable<KcGroup>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.CountGroupsAsync"/>
    public async Task<KcResponse<int>> CountGroupsAsync(string realm, string accessToken,
        string userId,
        KcFilter filter, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        filter ??= new KcFilter();

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/groups/count{filter.BuildQuery()}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return response.IsSuccessStatusCode
                ? new KcResponse<int>
                {
                    Response = int.Parse(await response.Content.ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false), CultureInfo.CurrentCulture)
                }
                : new KcResponse<int>
                {
                    IsError = true,
                    ErrorMessage = await response.Content.ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false)
                };
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to count user {userId} groups", e);
            }

            return new KcResponse<int>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.AddToGroupAsync"/>
    public async Task<KcResponse<object>> AddToGroupAsync(string realm, string accessToken,
        string userId,
        string groupId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/users/{userId}/groups/{groupId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to add user {userId} to group {groupId}", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.DeleteFromGroupAsync"/>
    public async Task<KcResponse<object>> DeleteFromGroupAsync(string realm, string accessToken,
        string userId,
        string groupId, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        if ( string.IsNullOrWhiteSpace(groupId) )
        {
            throw new KcException($"{nameof(groupId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/users/{userId}/groups/{groupId}", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to delete user {userId} from group {groupId}", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.ResetPasswordAsync"/>
    public async Task<KcResponse<KcCredentials>> ResetPasswordAsync(string realm,
        string accessToken, string userId,
        KcCredentials credentials, CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        if ( credentials == null )
        {
            throw new KcException($"{nameof(credentials)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Put,
                $"{_baseUrl}/{realm}/users/{userId}/reset-password",
                accessToken, KcRequestHandler.GetBody(credentials));

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<KcCredentials>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to update reset password for user {userId}", e);
            }

            return new KcResponse<KcCredentials>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.SessionsAsync"/>
    public async Task<KcResponse<IEnumerable<KcSession>>> SessionsAsync(string realm,
        string accessToken, string userId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Get,
                $"{_baseUrl}/{realm}/users/{userId}/sessions", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<IEnumerable<KcSession>>(response,
                cancellationToken).ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable to get user {userId} sessions", e);
            }

            return new KcResponse<IEnumerable<KcSession>>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.DeleteSessionAsync"/>
    public async Task<KcResponse<object>> DeleteSessionAsync(string realm, string accessToken,
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(sessionId) )
        {
            throw new KcException($"{nameof(sessionId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Delete,
                $"{_baseUrl}/{realm}/sessions/{sessionId}",
                accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, $"Unable delete session {sessionId}", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }

    /// <inheritdoc cref="IKcUsers.LogoutFromAllSessionsAsync"/>
    public async Task<KcResponse<object>> LogoutFromAllSessionsAsync(string realm,
        string accessToken, string userId,
        CancellationToken cancellationToken = default)
    {
        ValidateAccess(realm, accessToken);

        if ( string.IsNullOrWhiteSpace(userId) )
        {
            throw new KcException($"{nameof(userId)} is required");
        }

        try
        {
            using var request = KcRequestHandler.CreateRequest(HttpMethod.Post,
                $"{_baseUrl}/{realm}/users/{userId}/logout", accessToken);

            using var client = new HttpClient();

            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            return await KcRequestHandler.HandleAsync<object>(response, cancellationToken)
                .ConfigureAwait(false);
        }
        catch ( Exception e )
        {
            if ( _logger != null )
            {
                KcLoggerMessages.Error(_logger, "Unable to logout from all sessions", e);
            }

            return new KcResponse<object>
            {
                IsError = true,
                Exception = e
            };
        }
    }
}
