using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Apis.Converters.Abstract;
using MovieDbApi.Common.Domain.Apis.Converters.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Notifications.Abstract;
using MovieDbApi.Common.Domain.Utility;
using MovieDbApi.Common.Maintenance;
using Microsoft.EntityFrameworkCore;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Domain.Notifications.Specific
{
    public class EmailNotificationService
        : INotificationService
    {
        private readonly MediaContext _mediaContext;
        private readonly IBackendToFrontendConverter _converter;
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _logger;
        private readonly ITranslator _translator;

        public EmailNotificationService(
            MediaContext mediaContext,
            ILoggerService logger,
            IConfiguration configuration,
            ITranslator translator,
            IBackendToFrontendConverter converter)
        {
            _configuration = configuration;
            _translator = translator;
            _logger = logger;
            _mediaContext = mediaContext;
            _converter = converter;
        }

        public void Work()
        {
            DateTime today = DateTime.UtcNow.Date;

            List<Subscriber> subscribers = null;

            int tries = 10;
            while (tries > 0)
            {
                try
                {
                    subscribers = _mediaContext.Subscribers.Include(x => x.MediaItemTypes).ToList();
                    break;
                }
                catch (Exception ex)
                {
                    tries--;

                    if (tries < 0)
                    {
                        throw;
                    }

                    Task.Delay(1000).Wait();
                }
            }

            if (!(subscribers?.Count > 0))
            {
                return;
            }

            List<MediaItem> mediaItems = _mediaContext.MediaItems
                .Where(x => x.DateAdded.Date == today)
                .AsEnumerable()
                .GroupBy(x => x.Group)
                .SelectMany(x =>
                {
                    MediaItem firstItem = x.First();
                    int groupCount = x.Count();

                    if (groupCount == 1)
                    {
                        return x;
                    }

                    if (firstItem.Type == MediaItemType.Anime || firstItem.Type == MediaItemType.Series || firstItem.Type == MediaItemType.Cartoon)
                    {
                        return x.Where(y => y.IsGrouping || y.Group == null);
                    }

                    if (firstItem.Type == MediaItemType.Movie || firstItem.Type == MediaItemType.Concert)
                    {
                        return x.Where(y => !y.IsGrouping);
                    }

                    return x;
                })
                .ToList();

            if (!(mediaItems?.Count > 0))
            {
                return;
            }

            Dictionary<string, List<MediaItem>> translatedItems = new Dictionary<string, List<MediaItem>>();

            foreach (string language in subscribers.Select(x => x.Language).Distinct())
            {
                List<MediaItem> items = new List<MediaItem>();
                translatedItems[language] = items;

                foreach (MediaItem item in mediaItems)
                {
                    items.Add(_converter.Convert(new BackendToFrontendConverterContex(item, CommonConsts.BaseLanguage, language)));
                }
            }

            foreach (Subscriber subscriber in subscribers)
            {
                string title = $"{_translator.Translate(CommonConsts.BaseLanguage, subscriber.Language, "New movies")} - {DateTime.Today:yyyy-MM-dd}";

                StringBuilder sbMessage = new StringBuilder();

                sbMessage.AppendLine(title + ":");

                List<MediaItemType> mediaItemTypes = subscriber.MediaItemTypes?.Select(x => x.Type)?.ToList() ?? new List<MediaItemType>();
                List<MediaItem> itemsToNotifyAbout = translatedItems[subscriber.Language].Where(x => mediaItemTypes.Contains(x.Type)).ToList();

                if (itemsToNotifyAbout.Count > 0)
                {
                    foreach (MediaItem item in itemsToNotifyAbout)
                    {
                        sbMessage.AppendLine(item.Title);
                    }

                    TrySendEmail(subscriber.Email, title, sbMessage.ToString());
                }
            }
        }

        private void TrySendEmail(string email, string title, string content)
        {
            try
            {
                IConfigurationSection section = _configuration.GetSection(ConfigurationKeys.Email);

                string from = section[ConfigurationKeys.From];
                string user = section[ConfigurationKeys.User];
                string password = section[ConfigurationKeys.Password];
                string host = section[ConfigurationKeys.Host];

                using SmtpClient smtpClient = new SmtpClient(host)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(user, password),
                    EnableSsl = true,
                };

                smtpClient.Send(from, email, title, content);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }
    }
}
