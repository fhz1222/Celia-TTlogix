using Application.Interfaces;

namespace Presentation.Configuration;

public class AppSettings : IAppSettings
{
    public string OwnerCode { get; set; } = null!;
}
