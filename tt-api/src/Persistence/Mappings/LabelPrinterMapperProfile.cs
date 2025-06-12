using Application.UseCases.Registration;
using Application.UseCases.Registration.Commands.AddLabelPrinter;
using Application.UseCases.Registration.Commands.UpdateLabelPrinter;
using AutoMapper;
using Domain.Entities;
using Domain.Metadata;
using Persistence.Entities;

namespace Persistence.Mappings;

internal class LabelPrinterMapperProfile : Profile
{
    public LabelPrinterMapperProfile()
    {
        CreateMap<TtLabelPrinter, LabelPrinterListItemDto>();

        CreateMap<TtLabelPrinter, Metadata>();

        CreateMap<Metadata, TtLabelPrinter>();

        CreateMap<TtLabelPrinter, LabelPrinter>();

        CreateMap<UpdateLabelPrinterDto, TtLabelPrinter>();

        CreateMap<AddLabelPrinterDto, TtLabelPrinter>();
    }
}
