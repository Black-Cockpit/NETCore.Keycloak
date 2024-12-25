using Newtonsoft.Json;

namespace NETCore.Keycloak.Models.Tokens;

/// <summary>
/// Represents an address claim set in a Keycloak token.
/// <see href="https://www.keycloak.org/docs-api/20.0.3/rest-api/index.html#_addressclaimset"/>
/// </summary>
public class KcAddressClaimSet
{
    /// <summary>
    /// Gets or sets the country name in the address.
    /// </summary>
    /// <value>
    /// A string representing the country name.
    /// </value>
    [JsonProperty("country")]
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the formatted address.
    /// </summary>
    /// <value>
    /// A string representing the formatted address.
    /// </value>
    [JsonProperty("formatted")]
    public string Formatted { get; set; }

    /// <summary>
    /// Gets or sets the locality or city of the address.
    /// </summary>
    /// <value>
    /// A string representing the address locality.
    /// </value>
    [JsonProperty("locality")]
    public string Locality { get; set; }

    /// <summary>
    /// Gets or sets the postal code or ZIP code of the address.
    /// </summary>
    /// <value>
    /// A string representing the postal code.
    /// </value>
    [JsonProperty("postal_code")]
    public string PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the region or state of the address.
    /// </summary>
    /// <value>
    /// A string representing the address region.
    /// </value>
    [JsonProperty("region")]
    public string Region { get; set; }

    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    /// <value>
    /// A string representing the street address.
    /// </value>
    [JsonProperty("street_address")]
    public string StreetAddress { get; set; }
}
