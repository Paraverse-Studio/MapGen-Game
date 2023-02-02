using Paraverse.Mob.Stats;
using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/Effect Mod")]
public class SO_EffectMod : SO_Mod
{
    [Header("Obtained Effects")]
    public GameObject Effect;

    private PlayerCombat _player;

    public override void Activate(GameObject go)
    {
        base.Activate();

        // if the provided object can't be parsed into a player, something is wrong
        // activate() should be called from shop, and supplied with the player to act upon
        if (!go.TryGetComponent(out _player))
        {
            Debug.LogError("Effect Mod: Activate() called with a non-player parameter.");
            return;
        }

        // Set some info from mod card to skill 
        // ---> stat, info, logistics and lore of the skill is provided from mod card to skill
        // ---> skill CD, range, damage and these things are to be put right on skill prefab
        //Effect.Name = Title;
        Effect.GetComponent<MobEffect>().ID = ID;
        //Skill.Description = Description;
        //Skill.Image = Image;


        // Add this skill to the player's list of skills, and also activate this one
        _player.ActivateEffect(Effect); 

        Debug.Log($"Effect Mod: Mod \"{Title}\" (ID {ID}) activated for {_player.gameObject.name}!");
    }

}
