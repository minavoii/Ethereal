using System.Collections.Generic;
using Ethereal.Classes.Builders;

namespace Ethereal.Classes.Wrappers;

public sealed class ActionApplyBuffWrapper(List<BuffDefineBuilder> buffDefines) : ActionApplyBuff
{
    public List<BuffDefineBuilder> BuffDefines { get; init; } = buffDefines;
}
