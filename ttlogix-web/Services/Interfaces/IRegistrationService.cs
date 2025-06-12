using ServiceResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task<Result<bool>> PrintLocationLabels(CodesCombo[] Codes, string printer, int copies);
        Task<Result<IEnumerable<QRCodeDto>>> GetLocationLabels(CodesCombo[] Codes);
    }
}
