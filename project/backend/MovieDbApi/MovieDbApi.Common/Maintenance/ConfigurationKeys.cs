namespace MovieDbApi.Common.Maintenance
{
    public static class ConfigurationKeys
    {
        public const string ConnectionString = nameof(ConnectionString);
        public const string DefaultConnectionString = nameof(DefaultConnectionString);
        public const string MyAnimeList = nameof(MyAnimeList);
        public const string Anilist = nameof(Anilist);
        public const string OpenMovieDb = nameof(OpenMovieDb);
        public const string ApiKeys = nameof(ApiKeys);
        public const string IsDeveloper = nameof(IsDeveloper);
        public const string ApiKeysOpenMovieDb = $"{ApiKeys}:{OpenMovieDb}";
        public const string ApiKeysMyAnimeList = $"{ApiKeys}:{MyAnimeList}";
        public const string ApiKeysAnilist = $"{ApiKeys}:{Anilist}";
        public const string Email = nameof(Email);
        public const string From = nameof(From);
        public const string User = nameof(User);
        public const string Password = nameof(Password);
        public const string Host = nameof(Host);
    }
}
