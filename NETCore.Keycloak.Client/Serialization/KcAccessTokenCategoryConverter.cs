using NETCore.Keycloak.Client.Models.KcEnum;

namespace NETCore.Keycloak.Client.Serialization;

/// <inheritdoc/>>
public class KcAccessTokenCategoryConverter : KcJsonEnumConverter<KcAccessTokenCategory>
{
    /// <summary>
    /// Keycloak access token category names
    /// </summary>
    private static readonly Dictionary<KcAccessTokenCategory?, string> Names =
        new()
        {
            [KcAccessTokenCategory.Internal] = "INTERNAL",
            [KcAccessTokenCategory.Access] = "ACCESS",
            [KcAccessTokenCategory.Id] = "ID",
            [KcAccessTokenCategory.Admin] = "ADMIN",
            [KcAccessTokenCategory.AuthorizationResponse] = "AUTHORIZATION_RESPONSE",
            [KcAccessTokenCategory.Logout] = "LOGOUT",
            [KcAccessTokenCategory.Userinfo] = "USERINFO"
        };

    /// <inheritdoc/>>
    protected override string ConvertToString(KcAccessTokenCategory? value) =>
        value != null ? Names[value] : null;

    /// <inheritdoc/>>
    protected override KcAccessTokenCategory? ConvertFromString(string s) =>
        string.IsNullOrWhiteSpace(s) ? null :
        Names.ContainsValue(s) ? Names.First(pair => pair.Value == s).Key : null;
}
