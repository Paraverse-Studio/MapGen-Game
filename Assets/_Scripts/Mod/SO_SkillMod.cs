using Paraverse.Mob.Combat;
using Paraverse.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/Skill Mod")]
public class SO_SkillMod : SO_Mod
{
  [Header("Obtained Skills")]
  public GameObject Skill;

  private PlayerCombat _player;
  private MobSkill _skill;

  public override string GetDescription()
  {
    if (!_skill) _skill = Skill.GetComponent<MobSkill>();
    return Description.Replace("[DMG]", GetScalingText()) + $" ({_skill.Cooldown}s cooldown)";
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
    if (!_skill) _skill = Skill.GetComponent<MobSkill>();
    _skill.Name = Title;
    _skill.ID = ID;
    _skill.Description = Description;
    _skill.Image = Image;

    // Add this skill to the player's list of skills, and also activate this one
    _player.ActivateSkill(Skill);

    Debug.Log($"Skill Mod: Mod \"{Title}\" (ID {ID}) activated for {_player.gameObject.name}!");
  }

  private string GetScalingText()
  {
    string msg = "";
    if (_skill.scalingStatData.flatPower != 0)
    {
      msg += $"{_skill.scalingStatData.flatPower}";
    }
    if (_skill.scalingStatData.attackScaling != 0)
    {
      if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
      msg += $"<color=#FF977B>({_skill.scalingStatData.attackScaling * 100f}% of Attack)</color>";
    }
    if (_skill.scalingStatData.abilityScaling != 0)
    {
      if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
      msg += $"<color=#83C5FF>({_skill.scalingStatData.abilityScaling * 100f}% of Ability)</color>";
    }
    if (_skill.scalingStatData.healthScaling != 0)
    {
      if (!string.IsNullOrWhiteSpace(msg)) msg += " + ";
      msg += $"<color=#86F383>({_skill.scalingStatData.healthScaling * 100f}% of Health)</color>";
    }
    msg = "<b>" + msg + "</b>";
    return msg;
  }

}
