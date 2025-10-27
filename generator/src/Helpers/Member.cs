namespace Generators.Helpers;

internal static class MemberHelper
{
    internal static string ParseDefaultValue(object value) =>
        value switch
        {
            bool x => x.ToString().ToLowerInvariant(),
            string x => $"\"{x}\"",
            char x => $"'{x}'",
            _ => value.ToString(),
        };
}
