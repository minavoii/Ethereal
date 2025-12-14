using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;

namespace Ethereal.API;

[BasicAPI]
public static partial class MonsterTypes
{
    /// <summary>
    /// Get a monster type.
    /// </summary>
    /// <param name="monsterType"></param>
    [GetObject]
    public static async Task<MonsterType?> Get(EMonsterType monsterType)
    {
        await WhenReady();
        return GameController.Instance.MonsterTypes.Find(x => x?.Type == monsterType);
    }

    /// <summary>
    /// Get all monster types.
    /// </summary>
    /// <returns></returns>
    public static async Task<List<MonsterType>> GetAll()
    {
        await WhenReady();
        return
        [
            .. GameController.Instance.MonsterTypes.Where(x =>
                x.Type != EMonsterType.EnemySkills && x.Type != EMonsterType.UiSkills
            ),
        ];
    }
}
