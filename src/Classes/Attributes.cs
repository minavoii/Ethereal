using System;

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
