namespace Application.UseCases.ILogIntegration.Queries.GetConfig;

public class IntegrationConfigDto
{
    public string[] WHSCodes { get; set; } = default!;
    public bool IsEnabled { get; set; } = default!;
}