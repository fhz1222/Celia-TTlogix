using Application;
using Application.Interfaces;
using Application.Services.LocksService;
using Infrastructure;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.FeatureManagement;
using Persistence;
using Presentation.Configuration;
using Presentation.Utilities;
using Presentation.Utilities.Interfaces;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//TODO add auth

builder.Configuration.AddJsonFile("appsettings.Production.json", true);
var config = builder.Configuration;
var appSettings = config.Get<AppSettings>();
builder.Services.Configure<AppSettings>(config);

var aiConnectionString = config.GetSection("ApplicationInsights")["ConnectionString"];
var options = new ApplicationInsightsServiceOptions() { EnableAdaptiveSampling = false, ConnectionString = aiConnectionString };
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });
builder.Services.AddApplicationInsightsTelemetry(options);
builder.Services.AddApplicationInsightsTelemetryProcessor<CustomTelemetryFilter>();

builder.Services.AddControllers().AddJsonConverters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.AddSwaggerTypes());


builder.Services.AddApplication();
builder.Services.AddPersistence(config);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddInfrastructure();

builder.Services.AddSingleton<IAppSettings>(appSettings);
builder.Services.AddScoped<CheckIfLockedFilter>();
builder.Services.AddScoped<ILocksService, LocksService>();

builder.Services.AddFeatureManagement(builder.Configuration.GetSection("FeatureFlags"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsProduction())
{
    app.UseMiddleware<BasicAuthMiddleware>();
}

// TODO is it needed?
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.UseExceptionHandler(err => err.UseCustomErrors(app.Environment));

app.Run();
