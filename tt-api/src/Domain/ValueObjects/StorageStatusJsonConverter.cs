using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class StorageStatusJsonConverter : JsonConverter<StorageStatus>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public StorageStatusJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override StorageStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return StorageStatus.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, StorageStatus value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}