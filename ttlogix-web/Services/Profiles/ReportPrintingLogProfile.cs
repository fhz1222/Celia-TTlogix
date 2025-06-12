using AutoMapper;
using TT.Core.Entities;
using TT.Services.Models;

namespace TT.Services.Profiles
{
    public class ReportPrintingLogProfile : Profile
    {
        public ReportPrintingLogProfile()
        {
            CreateMap<ReportPrintingLog, ReportPrintedDto>();
        }
    }
}

