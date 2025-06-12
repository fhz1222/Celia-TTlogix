using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class DefinedTypeJsonConverter : JsonConverter<DefinedType>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public DefinedTypeJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override DefinedType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return DefinedType.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, DefinedType value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}