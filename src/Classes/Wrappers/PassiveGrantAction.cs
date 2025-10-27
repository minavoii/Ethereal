using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Wrappers;

public sealed class PassiveGrantActionWrapper(LazyAction action, LazyBuff? triggerBuff = null)
    : PassiveGrantAction
{
    public LazyAction ActionWrapper { get; init; } = action;

    public LazyBuff? TriggerBuffWrapper { get; init; } = triggerBuff;

    public void Unwrap()
    {
        Action = ActionWrapper.Get();

        if (TriggerBuffWrapper is not null)
            TriggerBuff = TriggerBuffWrapper.Get()?.gameObject;
    }
}
