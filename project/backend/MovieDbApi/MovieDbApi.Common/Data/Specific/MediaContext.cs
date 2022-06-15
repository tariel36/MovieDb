using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Maintenance;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Data.Specific
{
    public class MediaContext
        : DbContext
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILoggerService _logger; 

        public MediaContext()
        {

        }

        public MediaContext(IConfiguration configuration, ILoggerService logger)
        {
            _configuration = configuration;
            _logger = logger;
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
            string connectionString = _configuration.GetConnectionString(ConfigurationKeys.DefaultConnectionString);
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .LogTo(_logger.Log, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                ;
        }
    }
}
