using Domain.ValueObjects;
using System.Text.Json;

namespace Presentation.Configuration;

public static class JsonConvertersConfiguration
{
    public static IMvcBuilder AddJsonConverters(this IMvcBuilder builder)
        => builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new OwnershipJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new StorageStatusJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new InventoryAdjustmentJobTypeJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new InventoryAdjustmentStatusJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new ScannerTypeJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new DecantStatusJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new InboundReversalStatusJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new UomDecimalStatusJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new CustomerClientStatusJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new StatusJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new JobStatusJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new DefinedTypeJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
            options.JsonSerializerOptions.Converters.Add(new LocationTypeJsonConverter(new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        });
}
