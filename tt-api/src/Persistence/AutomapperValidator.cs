using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class AutomapperValidator
{
    public static void CheckMaps(this IServiceProvider services)
    {
        var mapper = services.GetRequiredService<IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
