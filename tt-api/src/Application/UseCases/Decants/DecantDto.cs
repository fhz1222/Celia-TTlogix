using Domain.ValueObjects;

namespace Application.UseCases.Decants;

public class DecantDto
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string? ReferenceNo { get; set; }
    public DateTime CreatedDate { get; set; }
    public DecantStatus Status { get; set; } = null!;
    public string? Remark { get; set; }

    // TODO what is the purpose of additional setter? There is already a property CustomerCode
    // TODO dto should probably not be exposing any functionalities apart from plain properties
    public DecantDto SetCustomerName(string customerName)
    {
        CustomerName = customerName;
        return this;
    }
}
