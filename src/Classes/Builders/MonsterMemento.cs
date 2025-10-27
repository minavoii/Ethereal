using Ethereal.Classes.LazyValues;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MonsterMemento at runtime.
/// </summary>
/// <param name="ID"></param>
/// <param name="Name"></param>
/// <param name="Monster"></param>
/// <param name="Area"></param>
/// <param name="Shift"></param>
/// <param name="ActionIconBig"></param>
/// <param name="ActionIconSmall"></param>
/// <param name="Icon"></param>
public sealed record MonsterMementoBuilder(
    int ID,
    string Name,
    LazyMonster Monster,
    EArea Area,
    EMonsterShift Shift,
    Sprite ActionIconBig,
    Sprite ActionIconSmall,
    Sprite Icon
)
{
    public MonsterMemento Build() =>
        new()
        {
            ID = ID,
            Name = Name,
            Monster = Monster.Get(),
            Area = Area,
            Shift = Shift,
            ActionIconBig = ActionIconBig,
            ActionIconSmall = ActionIconSmall,
            Icon = Icon,
        };
}
