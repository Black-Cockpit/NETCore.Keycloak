using NETCore.Keycloak.Client.Models.KcEnum;
using NETCore.Keycloak.Client.Serialization;
using Newtonsoft.Json;

namespace NETCore.Keycloak.Client.Models.Tokens;

/// <summary>
/// Keycloak access token representation.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_accesstoken"/>
/// </summary>
public class KcAccessToken
{
    /// <summary>
    /// Authentication Context Class Reference
    /// </summary>
    [JsonProperty("acr")]
    public string Acr { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [JsonProperty("address")]
    public KcAddressClaimSet Address { get; set; }

    /// <summary>
    /// List of allowed origins
    /// </summary>
    [JsonProperty("allowed-origins")]
    public IEnumerable<string> AllowedOrigins { get; set; }

    /// <summary>
    /// Access Token hash value
    /// </summary>
    [JsonProperty("at_hash")]
    public string AtHash { get; set; }

    /// <summary>
    /// Authentication timestamp
    /// </summary>
    [JsonProperty("auth_time")]
    public int? AuthTime { get; set; }

    /// <summary>
    /// Token authorization. <see cref="KcAccessTokenAuthorization"/>
    /// </summary>
    [JsonProperty("authorization")]
    public KcAccessTokenAuthorization Authorization { get; set; }

    /// <summary>
    /// Authorized party
    /// </summary>
    [JsonProperty("azp")]
    public string Azp { get; set; }

    /// <summary>
    /// Birthday
    /// </summary>
    [JsonProperty("birthdate")]
    public string Birthdate { get; set; }

    /// <summary>
    /// Code hash value.
    /// </summary>
    [JsonProperty("c_hash")]
    public string CHash { get; set; }

    /// <summary>
    /// Token category <see cref="KcAccessTokenCategory"/>
    /// </summary>
    [JsonProperty("category")]
    [JsonConverter(typeof(KcAccessTokenCategoryConverter))]
    public KcAccessTokenCategory Category { get; set; }

    /// <summary>
    /// Local claims
    /// </summary>
    [JsonProperty("claims_locales")]
    public string ClaimsLocales { get; set; }

    /// <summary>
    /// Proof-of-Possession Key, RFC 7800
    /// <see href="https://webconcepts.info/specs/IETF/RFC/7800"/>
    /// </summary>
    [JsonProperty("cnf")]
    public KcAccessTokenCertConf Cnf { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; }

    /// <summary>
    /// Indicates if the email is verified
    /// </summary>
    [JsonProperty("email_verified")]
    public bool? EmailVerified { get; set; }

    /// <summary>
    /// Token expiration
    /// </summary>
    [JsonProperty("exp")]
    public int? Exp { get; set; }

    /// <summary>
    /// Family name
    /// </summary>
    [JsonProperty("family_name")]
    public string FamilyName { get; set; }

    /// <summary>
    /// Gender
    /// </summary>
    [JsonProperty("gender")]
    public string Gender { get; set; }

    /// <summary>
    /// Given name
    /// </summary>
    [JsonProperty("given_name")]
    public string GivenName { get; set; }

    /// <summary>
    /// Issued at
    /// </summary>
    [JsonProperty("iat")]
    public int? Iat { get; set; }

    /// <summary>
    /// Token issuer
    /// </summary>
    [JsonProperty("iss")]
    public string Iss { get; set; }

    /// <summary>
    /// Json web token id
    /// </summary>
    [JsonProperty("jti")]
    public string Jti { get; set; }

    /// <summary>
    /// Locale; e.g. en-US
    /// </summary>
    [JsonProperty("locale")]
    public string Locale { get; set; }

    /// <summary>
    /// Middle name
    /// </summary>
    [JsonProperty("middle_name")]
    public string MiddleName { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Not before
    /// </summary>
    [JsonProperty("nbf")]
    public long? Nbf { get; set; }

    /// <summary>
    /// Nick name
    /// </summary>
    [JsonProperty("nickname")]
    public string Nickname { get; set; }

    /// <summary>
    /// Nonce
    /// </summary>
    [JsonProperty("nonce")]
    public string Nonce { get; set; }

    /// <summary>
    /// Other claims
    /// </summary>
    [JsonProperty("otherClaims")]
    public IDictionary<string, object> OtherClaims { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [JsonProperty("phone_number")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Indicates if the phone number is verified
    /// </summary>
    [JsonProperty("phone_number_verified")]
    public bool? PhoneNumberVerified { get; set; }

    /// <summary>
    /// Picture
    /// </summary>
    [JsonProperty("picture")]
    public string Picture { get; set; }

    /// <summary>
    /// Preferred username
    /// </summary>
    [JsonProperty("preferred_username")]
    public string PreferredUsername { get; set; }

    /// <summary>
    /// Profile
    /// </summary>
    [JsonProperty("profile")]
    public string Profile { get; set; }

    /// <summary>
    /// Real access permissions
    /// </summary>
    [JsonProperty("realm_access")]
    public KcTokenRealmAccess RealmAccess { get; set; }

    /// <summary>
    /// State hash value. Its value is the base64url encoding of the left-most half of the hash of the octets
    /// of the ASCII representation of the state value, where the hash algorithm used is the hash algorithm used in
    /// the alg Header Parameter of the ID Token’s JOSE Header. For instance, if the alg is HS512,hash the code value with SHA-512,
    /// then take the left-mots 256 bits and base64url encode them. The s_hash value is a case sensitive string.
    /// </summary>
    [JsonProperty("s_hash")]
    public string Hash { get; set; }

    /// <summary>
    /// Scope
    /// </summary>
    [JsonProperty("scope")]
    public string Scope { get; set; }

    /// <summary>
    /// Session state
    /// </summary>
    [JsonProperty("session_state")]
    public string SessionState { get; set; }

    /// <summary>
    /// Session id
    /// </summary>
    [JsonProperty("sid")]
    public string Sid { get; set; }

    /// <summary>
    /// Subject
    /// </summary>
    [JsonProperty("sub")]
    public string Sub { get; set; }

    /// <summary>
    /// Trusted certificates
    /// </summary>
    [JsonProperty("trusted-certs")]
    public IEnumerable<string> TrustedCerts { get; set; }

    /// <summary>
    /// Token type
    /// </summary>
    [JsonProperty("typ")]
    public string Typ { get; set; }

    /// <summary>
    /// Last updated at
    /// </summary>
    [JsonProperty("updated_at")]
    public long UpdatedAt { get; set; }

    /// <summary>
    /// Web site
    /// </summary>
    [JsonProperty("website")]
    public string Website { get; set; }

    /// <summary>
    /// Zone info
    /// </summary>
    [JsonProperty("zoneinfo")]
    public string Zoneinfo { get; set; }
}
