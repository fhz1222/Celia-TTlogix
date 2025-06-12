
namespace Application.UseCases.ILogLocationIntegration;

public class ILogIntegrationLocationDto
{
    public string Code { get; set; } = null!;
    public string Whscode { get; set; } = null!;
    public string AreaCode { get; set; } = null!;
    public int ILogLocationCategoryId { get; set; }
    public bool IsActive { get; set; }
}
