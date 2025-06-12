using Application.UseCases.InvoiceRequest.Commands.ValidatePrice;
using AutoMapper;

namespace Presentation.Common;

public class ValidatePriceRequestDto
{
    public decimal TotalPrice { get; set; } = default!;
    public List<int> RequestIds { get; set; } = default!;
    public string LoginId { get; set; } = default!;

    public class MapperProfile : Profile
    {
        public MapperProfile() => CreateMap<ValidatePriceRequestDto, ValidatePriceCommand>();
    }
}
