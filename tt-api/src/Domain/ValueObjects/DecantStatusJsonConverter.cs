using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

// TODO wrong namespace for a converter?
public class DecantStatusJsonConverter : JsonConverter<DecantStatus>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public DecantStatusJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override DecantStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return DecantStatus.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, DecantStatus value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}