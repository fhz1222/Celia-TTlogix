using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class ScannerTypeJsonConverter : JsonConverter<ScannerType>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public ScannerTypeJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override ScannerType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return ScannerType.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, ScannerType value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}