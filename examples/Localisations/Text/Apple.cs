namespace ExampleLocalisations.Text;

internal static class Apple
{
    internal const int Id = 9901;

    internal const string Original = "Apple";

    internal static readonly LocalisationData.LocalisationDataEntry entry = new()
    {
        ID = Id,
        StringContent = Original,
        StringContentEnglish = Original,
        StringContentFrench = "Pomme",
    };
}
