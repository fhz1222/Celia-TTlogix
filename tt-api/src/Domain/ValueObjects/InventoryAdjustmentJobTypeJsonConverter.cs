using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class InventoryAdjustmentJobTypeJsonConverter : JsonConverter<InventoryAdjustmentJobType>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public InventoryAdjustmentJobTypeJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override InventoryAdjustmentJobType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return InventoryAdjustmentJobType.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, InventoryAdjustmentJobType value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}