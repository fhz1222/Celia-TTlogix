using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class InboundReversalStatusJsonConverter : JsonConverter<InboundReversalStatus>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public InboundReversalStatusJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override InboundReversalStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return InboundReversalStatus.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, InboundReversalStatus value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}