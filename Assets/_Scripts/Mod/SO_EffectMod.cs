using Paraverse.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/Effect Mod")]
public class SO_EffectMod : SO_Mod
{
    [Header("Obtained Effects")]
    public GameObject Effect;

    private PlayerCombat _player;
    private MobEffect _effect;

    public override string GetTitle(int modLevel = -1)
    {
        Debug.Log("DOES THIS GET CALLED? " + modLevel + " so title: " + (Title + " " + UtilityFunctions.ToRoman(modLevel)));
        if (modLevel == -1) modLevel = ModLevel;
        return Title + (evolve.canStack ? " " + UtilityFunctions.ToRoman(modLevel) : string.Empty);
    }

    public override string GetDescription(int modLevel)
    {
        if (modLevel == -1) modLevel = ModLevel;
        if (!_effect) _effect = Effect.GetComponent<MobEffect>();
        return Description.Replace("[DMG]", GetScalingText(modLevel));
    }

    // Activates upon chest looting
    public override void Activate(GameObject go, int modLevel = -1)
    {
        Debug.Log("WHAT CALELD THis... actiate? ");
        if (modLevel == -1)
        {
            modLevel = ModLevel;
        }
        else
        {
            ModLevel = modLevel;
        }

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

        if (!_effect) _effect = Effect.GetComponent<MobEffect>();
        _effect.ID = ID;
        _effect.effectLevel = modLevel;
        //Skill.Description = Description;
        //Skill.Image = Image;


        // Add this skill to the player's list of skills, and also activate this one
        _player.ActivateEffect(Effect);

        Debug.Log($"Effect Mod: Mod \"{Title}\" (ID {ID}) LVL {modLevel} activated for {_player.gameObject.name}!");
    }

    public override SO_Mod Consume()
    {
        if (evolve.canStack)
        {
            //_modLevel++;
            AutofillDescription();
            return this;
        }
        else
        {
            return base.Consume();
        }
    }


    private string GetScalingText(int modLevel = -1)
    {
        if (modLevel == -1)
        {
            modLevel = ModLevel;
        }

        string msg = "";
        if (_effect.GetScalingStatData(modLevel).flatPower != 0)
        {
            msg += $"{_effect.GetScalingStatData(modLevel).flatPower}";
        }
        if (_effect.GetScalingStatData(modLevel).attackScaling != 0)
        {
            if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
            msg += $"<color=#FF977B>({_effect.GetScalingStatData(modLevel).attackScaling * 100f}% of Attack)</color>";
        }
        if (_effect.GetScalingStatData(modLevel).abilityScaling != 0)
        {
            if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
            msg += $"<color=#83C5FF>({_effect.GetScalingStatData(modLevel).abilityScaling * 100f}% of Ability)</color>";
        }
        if (_effect.GetScalingStatData(modLevel).healthScaling != 0)
        {
            if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
            msg += $"<color=#86F383>({_effect.GetScalingStatData(modLevel).healthScaling * 100f}% of Health)</color>";
        }
        msg = "<b>" + msg + "</b>";
        return msg;
    }

    public override void Reset()
    {
        base.Reset();        
        AutofillDescription();
    }

}
