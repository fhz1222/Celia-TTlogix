using System.Text;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Services.Interfaces;

namespace TT.Services.Label
{
    public abstract class LabelFactory : ILabelFactory
    {
        protected StringBuilder sb;
        protected LabelPrinter printer;

        public void Init(LabelPrinter printer)
        {
            this.printer = printer;
            this.sb = new StringBuilder();
        }

        public abstract Task AddLabel(StorageDetail pid, ILabelFactory.LabelType type);

        public abstract Task AddLabel(StorageDetailGroup group, ILabelFactory.LabelType type);

        public abstract Task AddLabel(Location location);

        public abstract Task Print(int copies);
    }
}