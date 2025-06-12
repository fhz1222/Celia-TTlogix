using ServiceResult;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TT.Core.QueryFilters;
using TT.Services.Models;

namespace TT.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<UserDto> GetUser(string code);
        Task<UserListDto> GetUsersList(UserListQueryFilter filter);
        Task<Result<UserDto>> CreateUser(UserAddDto user, string userCode);
        Task<Result<UserDto>> UpdateUser(UserUpdateDto user, string userCode);
        Task<Result<bool>> ToggleUserStatus(string code, string userCode);
        Task<Stream> UserReport(string whsCode);
        Task<IEnumerable<AccessGroupSimpleDto>> GetAccessGroups(AccessGroupFilter filter);
        Task<AccessGroupDto> GetAccessGroup(string code);
        Task<Result<AccessGroupDto>> UpdateAccessGroup(AccessGroupDto accessGroup, string name);
        Task<Result<AccessGroupDto>> CreateAccessGroup(AccessGroupAddDto accessGroup, string name);
        Task<Result<bool>> ToggleAccessGroupStatus(string code, string name);
        Task<IEnumerable<string>> GetPrivilegeNamesForGroup(string groupCode);
        Task<SystemModuleTreeDto> GetPrivilegesTree(string groupCode);
        Task<Result<bool>> UpdatePrivilegesTree(string groupCode, SystemModuleTreeDto tree);
    }
}
