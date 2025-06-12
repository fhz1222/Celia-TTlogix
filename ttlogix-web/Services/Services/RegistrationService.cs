using AutoMapper;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Core.QueryResults;
using TT.Services.Interfaces;
using TT.Services.Label;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class RegistrationService : ServiceBase<RegistrationService>, IRegistrationService
    {
        public RegistrationService(ITTLogixRepository repository,
            ILabelProvider labelProvider,
            IOptions<AppSettings> appSettings,
            ILocker locker,
            IMapper mapper,
            ILogger<RegistrationService> logger) : base(locker, logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.labelProvider = labelProvider;
            this.appSettings = appSettings.Value;
        }

        public async Task<Result<bool>> PrintLocationLabels(CodesCombo[] Codes, string IP, int copies)
        {
            var labelPrinter = await repository.LabelPrinters().Where(x => x.IP == IP).FirstAsync();

            var factory = labelProvider.CreateFactory(labelPrinter);
            var locations = await GetLocations(Codes);
            if (locations.Count == 0)
            {
                return new InvalidResult<bool>(new JsonResultError("ListIsEmpty").ToJson());
            }
            foreach (var l in locations)
            {
                await factory.AddLabel(l);
            }
            try
            {
                await factory.Print(copies);
            }
            catch (PrinterUnavailableException)
            {
                return new InvalidResult<bool>(new JsonResultError("PrinterIsUnavailable").ToJson());
            }
            return new SuccessResult<bool>(true);
        }

        public async Task<Result<IEnumerable<QRCodeDto>>> GetLocationLabels(CodesCombo[] Codes)
        {
            List<Location> locations = await GetLocations(Codes);
            List<string> data = new List<string>();

            var factory = new QRCodeLabelFactory(repository, (string[] res) => data = new List<string>(res));
            foreach (var l in locations)
            {
                await factory.AddLabel(l);
            }
            await factory.Print(1);

            IEnumerable<QRCodeDto> qrs = data.Select((d, i) => new QRCodeDto { Code = d, Name = Codes[i].Code });

            return new SuccessResult<IEnumerable<QRCodeDto>>(qrs);
        }

        private async Task<List<Location>> GetLocations(CodesCombo[] Codes)
        {
            var codes = Codes.Select(c => c.Code).Distinct();
            var selectedByCode = await repository.Locations()
                .Where(x => codes.Contains(x.Code))
                .ToListAsync();
            return selectedByCode.Where(x=>Codes.Any(c=>c.Code == x.Code && c.WHSCode == x.WHSCode)).ToList();
        }

        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
        private readonly ILabelProvider labelProvider;
        private readonly AppSettings appSettings;
    }
}
