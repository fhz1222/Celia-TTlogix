using TT.Core.Entities;

namespace TT.Services.Interfaces
{
    public interface ILabelProvider
    {
        public ILabelFactory CreateFactory(LabelPrinter printer);
    }
}
