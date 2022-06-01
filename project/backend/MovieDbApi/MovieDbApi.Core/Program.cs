using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Apis.Converters.Abstract;
using MovieDbApi.Common.Domain.Apis.Converters.Specific;
using MovieDbApi.Common.Domain.Media.Services.Abstract;
using MovieDbApi.Common.Domain.Media.Services.Specific;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MediaContext>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IBackendToFrontendConverter, BackendToFrontendConverter>();
builder.Services.AddScoped<ITranslator, GoogleTranslate>();

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

app.Run();
