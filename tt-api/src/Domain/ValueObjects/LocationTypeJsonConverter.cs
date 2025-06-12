using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class LocationTypeJsonConverter : JsonConverter<LocationType>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public LocationTypeJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override LocationType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return LocationType.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, LocationType value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}