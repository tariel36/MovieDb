using MovieDbApi.Common.Data.Caches.Abstract;
using MovieDbApi.Common.Data.Caches.Specific;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Apis.Converters.Abstract;
using MovieDbApi.Common.Domain.Apis.Converters.Specific;
using MovieDbApi.Common.Domain.Compression.Abstract;
using MovieDbApi.Common.Domain.Compression.Specific;
using MovieDbApi.Common.Domain.Media.Services.Abstract;
using MovieDbApi.Common.Domain.Media.Services.Specific;
using MovieDbApi.Common.Domain.Notifications.Abstract;
using MovieDbApi.Common.Domain.Notifications.Specific;
using MovieDbApi.Common.Domain.Tasks;
using MovieDbApi.Common.Maintenance.Logging.Abstract;
using MovieDbApi.Common.Maintenance.Logging.Specific;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MediaContext>();

builder.Services.AddScoped<IMediaMonitor, MediaMonitor>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IBackendToFrontendConverter, BackendToFrontendConverter>();
builder.Services.AddScoped<ITranslator, GoogleTranslate>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<IPathsService, PathsService>();

builder.Services.AddTransient<ILoggerSink, ConsoleLoggerSink>();

builder.Services.AddSingleton<ServicesContainer, ServicesContainer>();
builder.Services.AddSingleton<IHashProvider, Md5HashProvider>();
builder.Services.AddSingleton<ITranslationItemCache, TranslationItemCache>();
builder.Services.AddSingleton<ILoggerService, LoggerService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.UseHttpsRedirection();

app.MapControllers();

app.Services.GetRequiredService<ServicesContainer>().Start();

app.Run();
