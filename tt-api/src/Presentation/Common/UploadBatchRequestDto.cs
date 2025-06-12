using Application.UseCases.InvoiceRequest.Commands.UploadBatch;
using AutoMapper;

namespace Presentation.Common;

public class UploadBatchRequestDto
{
    public string SupplierId { get; set; } = default!;
    public string FactoryId { get; set; } = default!;
    public int? PriceValidationId { get; set; } = default!;
    public List<int> RequestIds { get; set; } = default!;
    public List<InvoiceDto> Invoices { get; set; } = default!;
    public string LoginId { get; set; } = default!;

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<InvoiceDto, UploadInvoiceDto>();
            CreateMap<UploadBatchRequestDto, UploadBatchCommand>();
        }
    }
}

public class InvoiceDto
{
    public string InvoiceNumber { get; set; } = default!;
    public decimal Value { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public int[] Content { get; set; } = default!;
}