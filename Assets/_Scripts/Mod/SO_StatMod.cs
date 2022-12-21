using Paraverse.Mob.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/Stat Mod")]
public class SO_StatMod : SO_Mod
{
    [Header("Stats Change")]
    public MobStats addedStats;
    private MobStats _player;

    public override void Activate(GameObject go)
    {
        base.Activate();

        // if the provided object can't be parsed into a player, something is wrong
        // activate() should be called from shop, and supplied with the player to act upon
        if (!go.TryGetComponent(out _player))
        {
            Debug.LogError("Skill Mod: Activate() called with a non-player parameter.");
            return;
        }

        // Set some info from mod card to skill 
        // ---> logistics and lore of the skill is provided from mod card to skill
        // ---> skill CD, range, damage and these things are to be put right on skill prefab
        //Skill.Name = Title;
        //Skill.Description = Description;

        //// Add this skill to the player's list of skills, and also activate this one
        //_player.skills.Add(Skill);
        //_player.ActivateSkill(Skill);

        Debug.Log($"Skill Mod: Mod \"{Title}\" (ID {ID}) activated for {_player.gameObject.name}!");
    }
}
