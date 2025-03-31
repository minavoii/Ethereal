using UnityEngine;

namespace Ethereal.API;

public static class Trait
{
    public static global::Trait Get(string name)
    {
        // Find trait by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            global::Trait trait = type.Traits.Find(x => x.Name == name);

            if (trait != null)
                return trait;
        }

        // Find signature trait
        foreach (GameObject monster in GameController.Instance.CompleteMonsterList)
        {
            if (monster == null)
                continue;

            global::Trait trait = monster
                .GetComponent<SkillManager>()
                ?.SignatureTrait?.GetComponent<global::Trait>();

            if (trait.Name == name)
                return trait;
        }

        return null;
    }
}
