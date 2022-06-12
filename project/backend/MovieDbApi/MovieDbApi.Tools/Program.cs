using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Apis.Specific.Anilist;
using MovieDbApi.Common.Domain.Compression.Abstract;
using MovieDbApi.Common.Domain.Compression.Specific;
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Crawling.Services;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Services.Abstract;
using MovieDbApi.Common.Domain.Media.Services.Specific;

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

MediaContext ctx = new MediaContext(configuration);

//List<TranslationCache> translations = ctx.TranslationCache.ToList();
//foreach (var translation in translations)
//{
//    if (string.IsNullOrWhiteSpace(translation.SourceHash))
//    {
//        translation.SourceHash = hashProvider.Get(translation.Source);
//        ctx.Update(translation);
//        ctx.SaveChanges();
//    }
//}

//MediaCrawlerService crawler = new MediaCrawlerService();
//crawler.Crawl(new MediaCrawlContext()
//{
//    Path = @"\\Nuta-NAS\nutka\s\media\cartoons_pl"
//});

IMediaService mediaService = new MediaService(ctx, hashProvider);

MediaMonitor monitor = new MediaMonitor(configuration, mediaService);
monitor.Work();


//var api = new AniListDataProvider(string.Empty);
//var res = api.GetByUrl("https://anilist.co/anime/113425/Kaifuku-Jutsushi-no-Yarinaoshi/");

Console.WriteLine("Done");
