using System.Threading.Tasks;
using TT.Core.Entities;

namespace TT.Services.Interfaces
{
    public interface ILabelFactory
    {
        public enum LabelType
        {
            SMALL,
            SMALLER,
            BIG
        }

        public void Init(LabelPrinter printer);
        public Task AddLabel(StorageDetail pid, LabelType type);
        public Task AddLabel(StorageDetailGroup goup, LabelType type);
        public Task AddLabel(Location location);
        public Task Print(int copies);
    }
}
