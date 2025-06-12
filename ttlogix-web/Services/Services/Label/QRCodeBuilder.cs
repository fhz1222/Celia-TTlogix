using System.Linq;
using TT.Services.Utilities;

namespace TT.Services.Label
{
    public class QRCodeBuilder
    {
        private readonly string delimeter;
        private string code = "";

        public QRCodeBuilder(string header, string delimeter)
        {
            this.delimeter = delimeter;
            code = header;
        }

        public QRCodeBuilder Append(string v, int? length = null)
        {
            code = code.Concat(length.HasValue ? v.ForceLength(length.Value) : v, delimeter);
            return this;
        }

        public override string ToString() => code;
    }
}
