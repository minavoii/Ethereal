using System.Threading.Tasks;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Wrappers;

public sealed class PassiveGrantActionWrapper(LazyAction action, LazyBuff? triggerBuff = null)
    : PassiveGrantAction
{
    public LazyAction ActionWrapper { get; init; } = action;

    public LazyBuff? TriggerBuffWrapper { get; init; } = triggerBuff;

    public async Task Unwrap()
    {
        Action = await ActionWrapper.Get();

        if (TriggerBuffWrapper is not null)
            TriggerBuff = (await TriggerBuffWrapper.Get())?.gameObject;
    }
}
