using Application.UseCases.StorageDetails;

namespace Persistence.Extensions
{
    internal static class StorageDetailItemDtoExtensions
    {
        internal static StorageDetailItemDto SetExternalValues(this StorageDetailItemDto dto, string location, string? externalPid, string? refNo, string? commInvNo, string iLogLocationCategory)
        {
            dto.Location = location;
            dto.ExternalPID = externalPid;
            dto.RefNo = refNo;
            dto.CommInvNo = commInvNo;
            dto.ILogLocationCategory = iLogLocationCategory;
            return dto;
        }
    }
}
