namespace MovieDbApi.Common.Domain.Apis.Converters.Abstract
{
    public interface ITranslator
    {
        string Translate(string from, string to, string value);
    }
}
