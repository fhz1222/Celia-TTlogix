using Microsoft.Extensions.Logging;
using System;
using TT.Core.Entities;
using TT.Services.Interfaces;
using TT.Services.Label;

namespace TT.Services.Services
{
    public class LabelProvider : ServiceBase<LabelProvider>, ILabelProvider
    {
        public static readonly string LocalQRCodeIP = "LocalQRCodeIP";

        public LabelProvider(IServiceProvider serviceProvider, ILocker locker, ILogger<LabelProvider> logger) : base(locker, logger)
        {
            this.serviceProvider = serviceProvider;
        }

        public ILabelFactory CreateFactory(LabelPrinter printer)
        {
            ILabelFactory factory;
            if (printer.IP == LocalQRCodeIP) factory = (ILabelFactory)this.serviceProvider.GetService(typeof(QRCodeLabelFactory));
            else factory = (ILabelFactory)this.serviceProvider.GetService(typeof(SatoLabelFactory));
            factory.Init(printer);
            return factory;
        }

        private readonly IServiceProvider serviceProvider;
    }
}
