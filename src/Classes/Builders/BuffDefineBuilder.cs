using Ethereal.Attributes;
using Ethereal.Classes.LazyValues;
using static ActionApplyBuff;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a BuffDefine at runtime.
/// </summary>
[PrivatePrimaryConstructor]
public sealed partial record BuffDefineBuilder
{
    public BuffDefineBuilder(int id, int stacks)
        : this(new LazyBuff(id), stacks) { }

    public BuffDefineBuilder(string name, int stacks)
        : this(new LazyBuff(name), stacks) { }

    public BuffDefineBuilder(BuffDefine buffDefine)
        : this(new LazyBuff(buffDefine.Buff.GetComponent<Buff>()), buffDefine.Stacks) { }

    public LazyBuff LazyBuff { get; init; }

    public int Stacks { get; init; }

    public BuffDefine? Build() => new() { Buff = LazyBuff.Get()?.gameObject, Stacks = Stacks };
}
