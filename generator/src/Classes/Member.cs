namespace Generators;

internal sealed record Member(string Type, string Name, string? DefaultValue = null)
{
    public string ParameterName => char.ToLowerInvariant(Name[0]) + Name.Substring(1);

    public string AsArgument() =>
        $"{Type} {ParameterName}{(DefaultValue is not null ? $" = {DefaultValue}" : "")}";

    public string AsSetter() => $"{Name} = {ParameterName}";
}
