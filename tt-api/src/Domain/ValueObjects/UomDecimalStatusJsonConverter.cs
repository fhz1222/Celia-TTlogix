using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class UomDecimalStatusJsonConverter : JsonConverter<UomDecimalStatus>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public UomDecimalStatusJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override UomDecimalStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return UomDecimalStatus.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, UomDecimalStatus value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}