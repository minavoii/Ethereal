using System;
using Generators;

namespace Ethereal.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal class BasicAPI : Attribute;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    Inherited = false,
    AllowMultiple = false
)]
internal class Deferrable : Attribute;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
internal class TryGet : Attribute;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal class PrivatePrimaryConstructor : Attribute;

[AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
internal class LazyValueConstructor : Attribute
{
    public (Type Type, string Name)[] NamedTypes => [(typeof(int), "id"), (typeof(string), "name")];
}

/// <summary>
/// Generate a getter and setter that forward their value to the given field or property.
/// </summary>
/// <param name="fieldOrProperty"></param>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
internal class Forward(
    string fieldOrProperty,
    ForwardConversion conversion = ForwardConversion.None
) : Attribute
{
    public string Link = fieldOrProperty;

    public ForwardConversion Conversion = conversion;
}

/// <summary>
/// Generate a getter and setter that forward their value to the field or property of the same name
/// in the given member.
/// </summary>
/// <param name="member"></param>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
internal class ForwardTo(string member, ForwardConversion conversion = ForwardConversion.None)
    : Attribute
{
    public string Link = member;

    public ForwardConversion Conversion = conversion;
}
