using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Maintenance;

namespace MovieDbApi.Common.Data.Specific
{
    public class MediaContext
        : DbContext
    {
        protected readonly IConfiguration Configuration;

        public MediaContext()
        {

        }

        public MediaContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<MediaItemAttribute> MediaItemAttributes { get; set; }
        public DbSet<MediaItemImage> MediaItemImages { get; set; }
        public DbSet<MediaItemLanguage> MediaItemLanguages { get; set; }
        public DbSet<MediaItemLink> MediaItemLinks { get; set; }
        public DbSet<MediaItemRelation> MediaItemRelations { get; set; }
        public DbSet<MediaItemTitle> MediaItemTitle { get; set; }
        public DbSet<ScannedPath> ScannedPaths { get; set; }
        public DbSet<IgnoredPath> IgnoredPaths { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<SubscriberMediaItemType> SubscriberMediaItemTypes { get; set; }
        public DbSet<TranslationCache> TranslationCache { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string connectionString = Configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString);
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                ;
        }
    }
}
