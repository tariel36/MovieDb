// See https://aka.ms/new-console-template for more information
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Crawling.Services;

Console.WriteLine("Hello, World!");


var crawler = new MediaCrawlerService();

var ctx = new MediaCrawlContext
{
};

crawler.Crawl(ctx);


Console.WriteLine("done");
