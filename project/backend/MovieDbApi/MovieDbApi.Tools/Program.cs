using System.Configuration;
using Microsoft.Extensions.Configuration;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Compression.Abstract;
using MovieDbApi.Common.Domain.Compression.Specific;
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Crawling.Services;
using MovieDbApi.Common.Domain.Media.Models.Data;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Path.GetFullPath(@"../../../../MovieDbApi.Core"))
    .AddJsonFile("appsettings.json")
    .Build();

IHashProvider hashProvider = new Md5HashProvider();

MediaContext ctx = new MediaContext(configuration);


List<TranslationCache> translations = ctx.TranslationCache.ToList();
foreach (var translation in translations)
{
    if (string.IsNullOrWhiteSpace(translation.SourceHash))
    {
        translation.SourceHash = hashProvider.Get(translation.Source);
        ctx.Update(translation);
        ctx.SaveChanges();
    }
}

Console.WriteLine("Done");
