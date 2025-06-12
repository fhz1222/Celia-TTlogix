using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

public class CustomerClientStatusJsonConverter : JsonConverter<CustomerClientStatus>
{
    private readonly JsonSerializerOptions ConverterOptions;

    public CustomerClientStatusJsonConverter(JsonSerializerOptions converterOptions)
    {
        ConverterOptions = converterOptions;
    }

    public override CustomerClientStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        return CustomerClientStatus.From(JsonSerializer.Deserialize<int>(ref reader, ConverterOptions));
    }

    public override void Write(Utf8JsonWriter writer, CustomerClientStatus value, JsonSerializerOptions options)
    {
        //Very important: Pass in ConverterOptions here, not the 'options' method parameter.
        JsonSerializer.Serialize(writer, (int)value, ConverterOptions);
    }
}