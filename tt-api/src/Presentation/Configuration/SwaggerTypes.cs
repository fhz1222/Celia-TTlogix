using Domain.ValueObjects;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Presentation.Configuration;

public static class SwaggerTypes
{
    public static SwaggerGenOptions AddSwaggerTypes(this SwaggerGenOptions c)
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "TT-Api",
            Description = "Web API for TTLogix",
            
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        c.MapType<Ownership>(() => new OpenApiSchema { Type = "integer" });
        c.MapType<InventoryAdjustmentJobType>(() => new OpenApiSchema { Type = "integer" });
        c.MapType<InventoryAdjustmentStatus>(() => new OpenApiSchema { Type = "integer" });

        // schema id replacements due to conflict
        var defaultSchemaIdSelector = c.SchemaGeneratorOptions.SchemaIdSelector;
        c.CustomSchemaIds(type =>
        {
            var defaultId = defaultSchemaIdSelector(type);
            return type == new Common.InvoiceDto().GetType() ? "InvoiceDto2" : defaultId;
        });

        return c;
    }
}
