using QrVision.Domain;
using QrVision.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonByFileName("sharedsettings");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDomainServices();
builder.Services.AddRepositories(builder.Configuration);

QrVision.Domain.FfmpegSettings.AddFfmpegGlobalSettings();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
