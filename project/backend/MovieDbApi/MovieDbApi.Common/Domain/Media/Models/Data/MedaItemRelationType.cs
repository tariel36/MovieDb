namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public enum MedaItemRelationType
    {
        Unknown = 0,

        ParentStory = 1,
        Other = 2,
        Adaptation = 3,
        Prequel = 4,
        SideStory = 5,
        Sequel = 6,
        Summary = 7,
        SpinOff = 8,
        Character = 9,
        AlternativeSetting = 10,
        AlternativeVersion = 11,
        FullStory = 12,

        MaxValue
    }
}
