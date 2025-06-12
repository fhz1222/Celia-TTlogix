using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Enums;
using TT.Core.Interfaces;
using TT.Core.QueryFilters;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services.Utilities;

namespace TT.Services.Services
{
    public class UserManagementService : ServiceBase<UserManagementService>, IUserManagementService
    {
        public UserManagementService(ITTLogixRepository repository,
            IMapper mapper,
            IReportService reportService,
            ILocker locker,
            ILogger<UserManagementService> logger) : base(locker, logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.reportService = reportService;
        }

        #region User

        public async Task<UserDto> GetUser(string code)
        {
            var user = await repository.GetUserAsync(code);
            if (user != null)
                return mapper.Map<UserDto>(user);
            return null;
        }

        public async Task<UserListDto> GetUsersList(UserListQueryFilter filter)
        {
            var query = repository.GetUsersList<UserListItemDto>(filter);
            var pagedQuery = query.Skip(filter.PageSize * (filter.PageNo - 1)).Take(filter.PageSize);
            var total = await query.CountAsync();
            var data = await pagedQuery.ToListAsync();

            return new UserListDto
            {
                Data = data,
                PageSize = filter.PageSize,
                PageNo = filter.PageNo,
                Total = total
            };
        }

        public async Task<Result<UserDto>> CreateUser(UserAddDto user, string userCode)
        {
            return await WithTransactionScope<UserDto>(async () =>
            {
                if (user.Password != user.ConfirmPassword)
                {
                    return new InvalidResult<UserDto>(new JsonResultError("PasswordsDoNotMatch").ToJson());
                }
                var entity = mapper.Map<User>(user);
                entity.CreatedBy = userCode;
                await repository.AddUserAsync(entity);
                return new SuccessResult<UserDto>(mapper.Map<UserDto>(entity));
            });
        }

        public async Task<Result<UserDto>> UpdateUser(UserUpdateDto user, string userCode)
        {
            return await WithTransactionScope<UserDto>(async () =>
            {
                var entity = await repository.GetUserAsync(user.Code);
                if (entity == null)
                {
                    return new NotFoundResult<UserDto>(new JsonResultError("RecordNotFound").ToJson());
                }
                if (!string.IsNullOrEmpty(user.Password) && user.Password != user.ConfirmPassword)
                {
                    return new InvalidResult<UserDto>(new JsonResultError("PasswordsDoNotMatch").ToJson());
                }
                mapper.Map(user, entity);
                entity.RevisedBy = userCode;
                entity.RevisedDate = DateTime.Now;
                await repository.SaveChangesAsync();
                return new SuccessResult<UserDto>(mapper.Map<UserDto>(entity));
            });
        }

        public async Task<Result<bool>> ToggleUserStatus(string code, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var entity = await repository.GetUserAsync(code);
                if (entity == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                entity.Status = entity.Status == ValueStatus.Active ? ValueStatus.InActive : ValueStatus.Active;
                entity.RevisedBy = userCode;
                entity.RevisedDate = DateTime.Now;
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(entity.Status == ValueStatus.Active);
            });
        }

        public async Task<Stream> UserReport(string whsCode)
        {
            var fileName = "UserReport.rpt";
            var title = "User Report";
            var stream = await Task.Run(() => reportService.GenerateReport(fileName, whsCode, title));
            return stream;
        }

        #endregion

        #region AccessGroup
        public async Task<IEnumerable<AccessGroupSimpleDto>> GetAccessGroups(AccessGroupFilter filter)
        {
            return await repository.GetAccessGroups<AccessGroupSimpleDto>(filter);
        }

        public async Task<AccessGroupDto> GetAccessGroup(string code)
        {
            var entity = await repository.GetAccessGroupAsync(code);
            if (entity != null)
                return mapper.Map<AccessGroupDto>(entity);
            return null;
        }

        public async Task<Result<AccessGroupDto>> UpdateAccessGroup(AccessGroupDto accessGroup, string userCode)
        {
            return await WithTransactionScope<AccessGroupDto>(async () =>
            {
                var entity = await repository.GetAccessGroupAsync(accessGroup.Code);
                if (entity == null)
                {
                    return new NotFoundResult<AccessGroupDto>(new JsonResultError("RecordNotFound").ToJson());
                }
                mapper.Map(accessGroup, entity);
                entity.RevisedBy = userCode;
                entity.RevisedDate = DateTime.Now;
                await repository.SaveChangesAsync();
                return new SuccessResult<AccessGroupDto>(mapper.Map<AccessGroupDto>(entity));
            });
        }

        public async Task<Result<AccessGroupDto>> CreateAccessGroup(AccessGroupAddDto accessGroup, string userCode)
        {
            return await WithTransactionScope<AccessGroupDto>(async () =>
            {
                var entity = mapper.Map<AccessGroup>(accessGroup);
                entity.CreatedBy = userCode;
                await repository.AddAccessGroupAsync(entity);
                return new SuccessResult<AccessGroupDto>(mapper.Map<AccessGroupDto>(entity));
            });
        }

        public async Task<Result<bool>> ToggleAccessGroupStatus(string code, string userCode)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                var entity = await repository.GetAccessGroupAsync(code);
                if (entity == null)
                {
                    return new NotFoundResult<bool>(new JsonResultError("RecordNotFound").ToJson());
                }
                entity.Status = entity.Status == ValueStatus.Active ? ValueStatus.InActive : ValueStatus.Active;
                entity.RevisedBy = userCode;
                entity.RevisedDate = DateTime.Now;
                await repository.SaveChangesAsync();
                return new SuccessResult<bool>(entity.Status == ValueStatus.Active);
            });
        }

        #endregion

        #region Privileges
        public async Task<IEnumerable<string>> GetPrivilegeNamesForGroup(string groupCode)
        {
            return await repository.GetSystemModuleNamesForGroup(groupCode);
        }
        public async Task<SystemModuleTreeDto> GetPrivilegesTree(string groupCode)
        {
            var modulesWithAccess = await repository.AccessRights().Where(a => a.GroupCode == groupCode && a.Status == 1).Select(a => a.ModuleCode).ToListAsync();
            var modules = await repository.SystemModules().ToListAsync();
            var topParent = modules.Where(m => m.ParentCode == string.Empty).Single();
            var tree = new SystemModuleTreeDto(topParent, null, modulesWithAccess.Contains(topParent.Code));
            tree.Children = GetChildren(tree, modules, modulesWithAccess).ToList();
            return tree;
        }
        public async Task<Result<bool>> UpdatePrivilegesTree(string groupCode, SystemModuleTreeDto tree)
        {
            return await WithTransactionScope<bool>(async () =>
            {
                // remove all nodes for this group
                await repository.DeleteAllAccessRightsAsync(groupCode);
                // recreate all nodes
                IList<AccessRight> ar = new List<AccessRight>();
                AssignPrivileges(groupCode, tree, ref ar);
                await repository.BatchAddAccessRightAsync(ar);
                return new SuccessResult<bool>(true);
            });
        }

        private void AssignPrivileges(string groupCode, SystemModuleTreeDto parent, ref IList<AccessRight> accessRights)
        {
            accessRights.Add(new AccessRight { GroupCode = groupCode, ModuleCode = parent.Code, Status = (byte?)(parent.IsChecked ? 1 : 0) });
            foreach (var child in parent.Children)
            {
                AssignPrivileges(groupCode, child, ref accessRights);
            }
        }


        private IEnumerable<SystemModuleTreeDto> GetChildren(SystemModuleTreeDto parent, IEnumerable<SystemModule> modules, IEnumerable<string> modulesWithAccess)
        {
            foreach (var module in modules.Where(m => m.ParentCode == parent.Code))
            {
                var subnode = new SystemModuleTreeDto(module, parent, modulesWithAccess.Contains(module.Code));
                subnode.Children = GetChildren(subnode, modules, modulesWithAccess);
                yield return subnode;
            }
        }

        #endregion

        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
        private readonly IReportService reportService;
    }
}
