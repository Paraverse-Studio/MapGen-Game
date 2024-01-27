using ParaverseWebsite.Models;
using UnityEngine;

namespace Paraverse.Helper
{
  public static class ParaverseHelper
  {
    /// <summary>
    /// Gets the distance between two gameobjects.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float GetDistance(Vector3 from, Vector3 to)
    {
      return Vector3.Distance(from, to);
    }

    /// <summary>
    /// Returns given position disregarding the Y axis. 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector3 GetPositionXZ(Vector3 position)
    {
      return new Vector3(position.x, 0f, position.z);
    }

    /// <summary>
    /// Rotates the gameobject to target direction with given rotation speed.
    /// </summary>
    /// <param name="from">Rotate from</param>
    /// <param name="to">Rotate to</param>
    /// <param name="speed">Rotation Speed</param>
    /// <returns></returns>
    public static Quaternion FaceTarget(Transform from, Transform to, float speed)
    {
      Vector3 targetDir = (to.position - from.position).normalized;
      targetDir = new Vector3(targetDir.x, 0.0f, targetDir.z);
      Quaternion lookRot = Quaternion.LookRotation(targetDir);
      return Quaternion.Slerp(from.rotation, lookRot, speed * Time.deltaTime);
    }

    public static string GetSkillName(SkillName skill)
    {
      switch (skill)
      {
        case SkillName.None:
          return "None";
        case SkillName.MoonlightSlash:
          return "Moonlight Slash";
        case SkillName.BladeWhirl:
          return "Blade Whirl";
        case SkillName.AvatarState:
          return "Avatar State";
        case SkillName.AzuriteInfusion:
          return "Azurite Infusion";
        case SkillName.StealthStep:
          return "Stealth Step";
        case SkillName.RegalCrescent:
          return "Regal Crescent";
        case SkillName.DescendingThrust:
          return "Descending Thrust";
        case SkillName.LightningBolt:
          return "Lightning Bolt";
        default:
          return "Error with " + skill.ToString();
      }
    }

    public static string GetEffectName(EffectName effect)
    {
      switch (effect)
      {
        case EffectName.None:
          return "None";
        case EffectName.Sunfire:
          return "Sunfire";
        case EffectName.CooldownRefund:
          return "Cooldown Refund";
        case EffectName.Lichbane:
          return "Lichbane";
        case EffectName.RepearKill:
          return "Repear Kill";
        case EffectName.SweepingDash:
          return "Sweeping Dash";
        case EffectName.EmpoweredAttack:
          return "Empowered Attack";
        default:
          return "Error with " + effect.ToString();
      }
    }

    public static string GetBloodlineName(BloodlineType bloodline)
    {
      switch (bloodline)
      {
        case BloodlineType.Vagabond:
          return "Vagabond";
        case BloodlineType.Harrier:
          return "Harrier";
        case BloodlineType.Pioneer:
          return "Pioneer";
        case BloodlineType.Scholar:
          return "Scholar";
        default:
          return "Error with " + bloodline.ToString();
      }
    }
  }
}