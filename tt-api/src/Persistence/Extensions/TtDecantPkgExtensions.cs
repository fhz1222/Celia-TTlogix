using Domain.Entities;
using Persistence.Entities;

namespace Persistence.Extensions
{
    static internal class TtDecantPkgExtensions
    {
        public static void CopyValuesFromStorageDetail(this TtDecantPkg decantPkg, Pallet pallet)
        {
            decantPkg.ProductCode = pallet.Product.Code;
            decantPkg.SupplierId = pallet.Product.CustomerSupplier.SupplierId;
            decantPkg.Length = pallet.Length;
            decantPkg.Width = pallet.Width;
            decantPkg.Height = pallet.Height;
            decantPkg.NetWeight = pallet.NetWeight;
            decantPkg.GrossWeight = pallet.GrossWeight;
        }
    }
}
