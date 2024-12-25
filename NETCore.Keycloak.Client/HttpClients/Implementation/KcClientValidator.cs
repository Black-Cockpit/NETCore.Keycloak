namespace NETCore.Keycloak.Client.HttpClients.Implementation;

/// <summary>
/// Validator for realm name and access token
/// </summary>
public class KcClientValidator
{
    /// <summary>
    /// Validate realm name and access token
    /// </summary>
    /// <param name="realm">Keycloak realm name</param>
    /// <param name="accessToken">Realm admin access token</param>
    /// <exception cref="KcException"></exception>
    protected void ValidateAccess(string realm, string accessToken)
    {
        if ( string.IsNullOrWhiteSpace(realm) )
        {
            throw new KcException($"{nameof(realm)} is required");
        }

        if ( string.IsNullOrWhiteSpace(accessToken) )
        {
            throw new KcException($"{nameof(accessToken)} is required");
        }
    }
}
