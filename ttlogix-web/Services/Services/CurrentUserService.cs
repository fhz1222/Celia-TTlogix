using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Interfaces;
using TT.Services.Interfaces;
using TT.Services.Models;

namespace TT.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IUserManagementService userService,
            ITTLogixRepository repository,
            IOptions<AppSettings> appSettings,
            IMapper mapper,
            IHttpContextAccessor contextAccessor)
        {
            this.userService = userService;
            this.repository = repository;
            this.mapper = mapper;
            this.appSettings = appSettings.Value;
            this.contextAccessor = contextAccessor;
        }

        public async Task<CurrentUserDto> Authenticate(string username, string password, string warehouse)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await repository.GetUserAsync(username);

            // check if username exists
            if (user == null || user.Status == 0)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.Password))
                return null;

            if (warehouse == null)
            {
                warehouse = user.WHSCode;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            var roles = await userService.GetPrivilegeNamesForGroup(user.GroupCode);

            //filter the roles to the ones that are currently used in the system
            var usedRoles = GetUsedRoles().Intersect(roles);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Code.ToString()),
                    //new Claim(ClaimTypes.Role, String.Join(",", roles.ToArray())),
                    new Claim(ClaimTypes.UserData, warehouse)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            foreach (var role in usedRoles)
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return mapper.Map<CurrentUserDto>(user, opts => opts.AfterMap((src, dest) =>
            {
                dest.Token = tokenHandler.WriteToken(token);
                dest.Roles = roles;
                dest.WHSCode = warehouse;
            }));
        }

        public async Task<CurrentUserDto> CurrentUser()
        {
            if (userCache == null && CurrentUserId() != null)
            {
                var user = await repository.GetUserAsync(CurrentUserId());
                if (user == null)
                    return null;

                var roles = await userService.GetPrivilegeNamesForGroup(user.GroupCode);
                userCache = mapper.Map<CurrentUserDto>(user, opts => opts.AfterMap((src, dest) =>
                {
                    dest.Roles = roles;
                    dest.WHSCode = CurrentUserWarehouse();
                }));

            }
            return userCache;
        }

        private string CurrentUserId()
        {
            ClaimsPrincipal claimsIdentity = contextAccessor.HttpContext.User;
            var name = claimsIdentity.FindFirst(ClaimTypes.Name);
            return name?.Value;
        }

        private string CurrentUserWarehouse()
        {
            ClaimsPrincipal claimsIdentity = contextAccessor.HttpContext.User;
            var name = claimsIdentity.FindFirst(ClaimTypes.UserData);
            return name?.Value;
        }

        private IEnumerable<string> GetUsedRoles()
        {
            FieldInfo[] fieldInfos = typeof(SystemModuleNames).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList().Select(f => f.Name);
        }

        private bool VerifyPasswordHash(string password, string storedPassword)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            // var passwordHasher = new PasswordHasher<TT.DB.User>();

            return storedPassword == password;
            //return passwordHasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Success;
        }

        private readonly IUserManagementService userService;
        private readonly ITTLogixRepository repository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly AppSettings appSettings;
        private CurrentUserDto userCache;
    }
}