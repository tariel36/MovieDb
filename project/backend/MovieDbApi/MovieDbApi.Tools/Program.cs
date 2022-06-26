using Microsoft.Extensions.Configuration;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Compression.Abstract;
using MovieDbApi.Common.Domain.Compression.Specific;
using MovieDbApi.Common.Domain.Media.Services.Abstract;
using MovieDbApi.Common.Domain.Media.Services.Specific;
using MovieDbApi.Common.Maintenance.Logging.Abstract;
using MovieDbApi.Common.Maintenance.Logging.Specific;

AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    Console.WriteLine(e.ExceptionObject);
}

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Path.GetFullPath(@"../../../../MovieDbApi.Core"))
    .AddJsonFile("appsettings.json")
    .Build();

IHashProvider hashProvider = new Md5HashProvider();

ILoggerService logger = new LoggerService(new ILoggerSink[] { new ConsoleLoggerSink() });

MediaContext ctx = new MediaContext(configuration, logger);

IMediaService mediaService = new MediaService(ctx, hashProvider);
IPathsService pathsService = new PathsService(ctx);

MediaMonitor monitor = new MediaMonitor(logger, configuration, pathsService, mediaService);
monitor.Work();

Console.WriteLine("Done");
