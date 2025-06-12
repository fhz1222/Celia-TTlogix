using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class InventoryAdjustmentStatusJsonConverter : JsonConverter<InventoryAdjustmentStatus>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public InventoryAdjustmentStatusJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override InventoryAdjustmentStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return InventoryAdjustmentStatus.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, InventoryAdjustmentStatus value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}