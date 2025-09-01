using QrVision.Domain;
using QrVision.Infra;
using QrVision.Worker.Consumers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Configuration.AddJsonByFileName("sharedsettings");
builder.Services.AddHostedService<ProcessVideoMessageConsumer>();
builder.Services.AddDomainServices();
builder.Services.AddRepositories(builder.Configuration);

FfmpegSettings.AddFfmpegGlobalSettings();

var host = builder.Build();
host.Run();
