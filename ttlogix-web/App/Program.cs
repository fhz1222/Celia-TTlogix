using AutoMapper.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NLog.Web;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using TT.Common;
using TT.Controllers.Authorization;
using TT.Controllers.Utilities;
using TT.Core.Interfaces;
using TT.DB;
using TT.Extensions;
using TT.Services.Interfaces;
using TT.Services.Middleware;
using TT.Services.Utilities;
using VueCliMiddleware;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.server.json", optional: true, reloadOnChange: true).Build();

// set up logging
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();

var configuration = builder.Configuration;
var appSettingsSection = configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

Action<SqlServerDbContextOptionsBuilder> contextOptions = options => options.CommandTimeout(90);
builder.Services
    .AddDbContext<Context>(builder => builder
        .UseSqlServer(configuration.GetConnectionString("Database"), contextOptions)
        .EnableSensitiveDataLogging())
    .AddDbContext<MRPContext>(builder => builder
        .UseSqlServer(configuration.GetConnectionString("MRPDatabase"), contextOptions));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
    cfg.AddDataReaderMapping();
});

builder.Services.AddScoped<IRawSqlExecutor, RawSqlExecutor>();
builder.Services.AddTransient<IValidatorInterceptor, CustomValidatorInterceptor>();
builder.Services.AddScoped<CheckIfLockedFilter>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAppServices(appSettingsSection.Get<AppSettings>());

// In production, the Vue files will be served from this directory
builder.Services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");

builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TTLogix API",
        Version = "1.0"
    });

    var xmlCommentsFile = $"Controllers.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setup.IncludeXmlComments(xmlCommentsFullPath);
});

// add authorization policies
builder.Services.AddSingleton<IAuthorizationPolicyProvider, ModuleAccessPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, ModuleAccessAuthorizationHandler>();

// add application insights
var aiConnectionString = configuration.GetSection("ApplicationInsights")["ConnectionString"];
var options = new ApplicationInsightsServiceOptions() { ConnectionString = aiConnectionString, EnableAdaptiveSampling = false };
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });
builder.Services.AddApplicationInsightsTelemetry(options);

// add reverse proxy
var proxyBuilder = builder.Services.AddReverseProxy();
// todo: move configuration to a separate class
proxyBuilder.LoadFromConfig(configuration.GetSection("ReverseProxy"))
    .AddTransforms(builder =>
    {
        var apiClientId = configuration.GetSection("Api").GetValue<string>("ClientId");
        var apiSecret = configuration.GetSection("Api").GetValue<string>("Secret");
        var authToken = Encoding.ASCII.GetBytes($"{apiClientId}:{apiSecret}");
        builder.AddRequestTransform(async context =>
        {
            var userService = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
            var currentUser = await userService.CurrentUser();
            var userCode = currentUser.Code;
            var whsCode = currentUser.WHSCode;
            context.ProxyRequest.RequestUri = RequestUtilities.MakeDestinationAddress(
                context.DestinationPrefix,
                context.Path,
                context.Query.QueryString
                    .ReplaceOrAdd("userCode", userCode)
                    .ReplaceOrAdd("whsCode", whsCode));
            context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        });
        // todo: implement API errors mapping
        //.AddResponseTransform(async context =>
        //{
        //    if (!context.ProxyResponse.IsSuccessStatusCode)
        //    {
        //        var stream = await context.ProxyResponse.Content.ReadAsStreamAsync();
        //        using var reader = new StreamReader(stream);
        //        var body = await reader.ReadToEndAsync();

        //        if (!string.IsNullOrEmpty(body))
        //        {
        //            context.SuppressResponseBody = true;

        //            body = body.Replace("Bravo", "Charlie");

        //            var bytes = Encoding.UTF8.GetBytes(body);

        //            // Change Content-Length to match the modified body, or remove it.
        //            context.HttpContext.Response.ContentLength = bytes.Length;

        //            // Response headers are copied before transforms are invoked, update any needed headers on the HttpContext.Response.
        //            await context.HttpContext.Response.Body.WriteAsync(bytes);
        //        }
        //    }
        //});
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "V1");
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<VersionCheckMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseEndpoints(endpoints =>
{
    // Register the reverse proxy routes
    endpoints.MapReverseProxy();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    endpoints.MapToVueCliProxy(
        "{*path}",
        new SpaOptions { SourcePath = "ClientApp" },
        npmScript: System.Diagnostics.Debugger.IsAttached ? "serve" : null,
        regex: "running at",
        forceKill: true);
});

app.Run();