using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NETCore.Keycloak.Client.Serialization;

/// <summary>
/// Keycloak enum serializer
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public abstract class KcJsonEnumConverter<TEnum> : JsonConverter
    where TEnum : struct, IConvertible
{
    /// <summary>
    /// Convert enum to string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected abstract string ConvertToString(TEnum? value);

    /// <summary>
    /// Get enum value from string
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    protected abstract TEnum? ConvertFromString(string s);

    /// <inheritdoc/>>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // Ensure the writer is not null
        ArgumentNullException.ThrowIfNull(writer);

        if ( value == null )
        {
            return;
        }

        var actualValue = ( TEnum ) value;
        writer.WriteValue(ConvertToString(actualValue));
    }

    /// <inheritdoc/>>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        // Ensure the reader is not null
        ArgumentNullException.ThrowIfNull(reader);

        if ( reader.TokenType == JsonToken.StartArray )
        {
            var items = new List<TEnum?>();
            var array = JArray.Load(reader);
            items.AddRange(array.Select(x => ConvertFromString(x.ToString())));

            return items;
        }

        var s = ( string ) (reader.Value ?? string.Empty);
        return ConvertFromString(s);
    }

    /// <inheritdoc/>>
    public override bool CanConvert(Type objectType) => objectType == typeof(string);
}
