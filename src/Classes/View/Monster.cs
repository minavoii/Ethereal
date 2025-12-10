using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using Ethereal.Attributes;
using Generators.Enums;
using UnityEngine;

namespace Ethereal.Classes.View;

/// <summary>
/// A helper class that allows easy viewing and editing of a Monster.
/// </summary>
/// <param name="gameObject"></param>
public sealed partial class MonsterView(GameObject gameObject)
{
    /// <summary>
    /// A helper class that allows easy viewing and editing of a Monster.
    /// </summary>
    /// <param name="monster"></param>
    public MonsterView(Monster monster)
        : this(monster.gameObject) { }

    public GameObject GameObject => gameObject;

    public readonly Monster Monster = gameObject.GetComponent<Monster>();

    public readonly SkillManager SkillManager = gameObject.GetComponent<SkillManager>();

    public readonly MonsterStats Stats = gameObject.GetComponent<MonsterStats>();

    public readonly MonsterAI AI = gameObject.GetComponent<MonsterAI>();

    public readonly MonsterAnimator Animator = gameObject.GetComponent<MonsterAnimator>();

    public readonly MonsterShift Shift = gameObject.GetComponent<MonsterShift>();

    [ForwardTo("Monster")]
    public partial int ID { get; set; }

    [ForwardTo("Monster")]
    public partial string Name { get; set; }

    [ForwardTo("Monster")]
    public partial string Description { get; set; }

    [ForwardTo("SkillManager")]
    public partial EMonsterMainType MainType { get; set; }

    public List<EMonsterType> Types
    {
        get => [.. SkillManager.MonsterTypes.Select(x => x.GetComponent<MonsterType>().Type)];
        set =>
            SkillManager.MonsterTypes = [
                .. value.Select(x =>
                    MonsterTypes.TryGet(x, out MonsterType type) ? type.gameObject : null
                ),
            ];
    }

    [ForwardTo("SkillManager")]
    public partial List<EElement> Elements { get; set; }

    [Forward("SkillManager.StaggerDefines")]
    public partial List<StaggerDefine> Stagger { get; set; }

    [ForwardTo("SkillManager")]
    public partial List<StaggerDefine> BossStagger { get; set; }

    [ForwardTo("SkillManager", ForwardConversion.GameObject)]
    public partial Trait SignatureTrait { get; set; }

    [ForwardTo("SkillManager", ForwardConversion.GameObjectList)]
    public partial List<BaseAction> StartActions { get; set; }

    [ForwardTo("SkillManager", ForwardConversion.GameObject)]
    public partial Trait EliteTrait { get; set; }

    [Forward("Stats.PerkInfosList")]
    public partial List<PerkInfos> BasePerks { get; set; }

    [ForwardTo("Stats")]
    public partial int BaseMaxHealth { get; set; }

    [ForwardTo("AI")]
    public partial List<MonsterAIAction> Scripting { get; set; }

    [Forward("AI.Traits")]
    public partial List<MonsterAI.MonsterAITrait> WildTraits { get; set; }

    public Texture2D Texture
    {
        get => Animator.Texture;
        set
        {
            Texture.SetPixels32(value.GetPixels32());
            Texture.Apply();
        }
    }

    public Sprite[] ShiftedSprites
    {
        set => Sprites.ShiftedSprites[Monster.ID] = value;
    }
}
