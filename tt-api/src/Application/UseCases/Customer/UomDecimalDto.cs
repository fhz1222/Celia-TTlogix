namespace Application.UseCases.Customer;

public class UomDecimalDto
{
    public string CustomerCode { get; set; } = null!;
    public string Uom { get; set; } = null!;
    public int DecimalNum { get; set; }
}