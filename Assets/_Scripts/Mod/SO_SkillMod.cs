using Paraverse.Combat;
using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/Skill Mod")]
public class SO_SkillMod : SO_Mod
{
    [Header("Obtained Skills")]
    public MobSkill Skill;

    private PlayerCombat _player;


    public override string GetDescription()
    {
        return Description.Replace("[DMG]", GetScalingText());
    }

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
        // ---> stat, info, logistics and lore of the skill is provided from mod card to skill
        // ---> skill CD, range, damage and these things are to be put right on skill prefab
        Skill.Name = Title;
        Skill.ID = ID;
        Skill.Description = Description;
        Skill.Image = Image;
        Debug.Log($"Put... {Image} into skill's image... {Skill.Image}");

        // Add this skill to the player's list of skills, and also activate this one
        _player.ActivateSkill(Skill);
        
        Debug.Log($"Skill Mod: Mod \"{Title}\" (ID {ID}) activated for {_player.gameObject.name}!");
    }

    private string GetScalingText()
    {
        string msg = "";
        if (Skill.scalingStatData.flatPower != 0)
        {
            msg += $"<b>{Skill.scalingStatData.flatPower}</b>";
        }
        if (Skill.scalingStatData.attackScaling != 0)
        {
            if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
            msg += $"<color=#FF977B>({Skill.scalingStatData.attackScaling * 100f}% Attack)</color>";
        }
        if (Skill.scalingStatData.abilityScaling != 0)
        {
            if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
            msg += $"<color=#83C5FF>({Skill.scalingStatData.abilityScaling * 100f}% Ability)</color>";
        }
        return msg;
    }

}
