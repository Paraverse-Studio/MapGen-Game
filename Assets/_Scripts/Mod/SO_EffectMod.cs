using Paraverse.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/Effect Mod")]
public class SO_EffectMod : SO_Mod
{
    [Header("Obtained Effects")]
    public GameObject Effect;

    private int _modLevel = 0;
    public int ModLevel => _modLevel;

    private PlayerCombat _player;
    private MobEffect _effect;

    public override string GetTitle()
    {
        return Title + (evolve.canStack? " " + UtilityFunctions.ToRoman(ModLevel) : string.Empty);
    }

    public override string GetDescription()
    {
        if (!_effect) _effect = Effect.GetComponent<MobEffect>();
        return Description.Replace("[DMG]", GetScalingText());
    }

    // Activates upon chest looting
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

        if (!_effect) _effect = Effect.GetComponent<MobEffect>();
        _effect.ID = ID;
        _effect.effectLevel = ModLevel;
        //Skill.Description = Description;
        //Skill.Image = Image;


        // Add this skill to the player's list of skills, and also activate this one
        _player.ActivateEffect(Effect);

        Debug.Log($"Effect Mod: Mod \"{Title}\" (ID {ID}) activated for {_player.gameObject.name}!");
    }

    public override SO_Mod Consume()
    {
        if (evolve.canStack)
        {
            _modLevel++;
            AutofillDescription();
            return this;
        }
        else
        {
            return base.Consume();
        }
    }


    private string GetScalingText()
    {
        string msg = "";
        if (_effect.GetScalingStatData().flatPower != 0)
        {
            msg += $"{_effect.GetScalingStatData().flatPower}";
        }
        if (_effect.GetScalingStatData().attackScaling != 0)
        {
            if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
            msg += $"<color=#FF977B>({_effect.GetScalingStatData().attackScaling * 100f}% of Attack)</color>";
        }
        if (_effect.GetScalingStatData().abilityScaling != 0)
        {
            if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
            msg += $"<color=#83C5FF>({_effect.GetScalingStatData().abilityScaling * 100f}% of Ability)</color>";
        }
        if (_effect.GetScalingStatData().healthScaling != 0)
        {
            if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
            msg += $"<color=#86F383>({_effect.GetScalingStatData().healthScaling * 100f}% of Health)</color>";
        }
        msg = "<b>" + msg + "</b>";
        return msg;
    }

    public override void Reset()
    {
        base.Reset();        
        _modLevel = 1;
        AutofillDescription();
    }

}
