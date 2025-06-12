using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using TT.Common;
using TT.Core.Interfaces;
using TT.DB;
using TT.Services.Interfaces;
using TT.Services.Label;
using TT.Services.Services;
using TT.Services.Services.Utilities;

namespace TT.Services.Utilities
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, AppSettings settings)
        {
            services.AddScoped<IILogConnect, ILogConnect>();
            services.AddScoped<ITTLogixRepository, SqlTTLogixRepository>();
            services.AddScoped<ITTLogixRepositoryForOutboundImportEKanban, SqlTTLogixRepositoryForOutboundImportEKanban>();
            services.AddScoped<IMRPRepository, SqlMRPRepository>();

            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IOutboundService, OutboundService>();
            services.AddScoped<IEKanbanService, EKanbanService>();
            services.AddScoped<IPickingListService, PickingListService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ILoggerService, LoggerSerivce>();
            services.AddScoped<ILoadingService, LoadingService>();
            services.AddScoped<IXlsService, XlsService>();
            services.AddScoped<IInboundService, InboundService>();
            services.AddScoped<IBillingService, BillingService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IPriceMasterService, PriceMasterService>();
            services.AddScoped<ILocksService, LocksService>();
            services.AddScoped<IStockTransferService, StockTransferService>();
            services.AddScoped<IEStockTransferService, EStockTransferService>();
            services.AddScoped<ILabelProvider, LabelProvider>();
            services.AddScoped<IStorageGroupService, StorageGroupService>();
            services.AddScoped<IRegistrationService, RegistrationService>();

            services.AddTransient<SatoLabelFactory>();

            services.AddHostedService<LockRemovalService>();

            services.AddSingleton<ILocker, Locker>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/api/locks")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = async context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserManagementService>();
                        var user = await userService.GetUser(context.Principal.Identity.Name);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            return services;
        }
    }
}
