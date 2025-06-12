using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class StatusJsonConverter : JsonConverter<Status>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public StatusJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return Status.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, Status value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}