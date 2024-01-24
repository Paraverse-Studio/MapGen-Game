using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class EffectsObtainedOccurancesModel
  {
    public int EmpoweredAttack = 0;
    public int Sunfire = 0;
    public int CooldownRefund = 0;
    public int Lichbane = 0;
    public int RepearKill = 0;
    public int SweepingDash = 0;

    public EffectsObtainedOccurancesModel() { }

    /// <summary>
    /// For updating Match History 
    /// </summary>
    /// <param name="effects"></param>
    public EffectsObtainedOccurancesModel(List<EffectName> effects)
    {
      foreach (EffectName eff in effects)
      {
        switch (eff)
        {
          case EffectName.EmpoweredAttack:
            EmpoweredAttack++;
            break;
          case EffectName.Sunfire:
            Sunfire++;
            break;
          case EffectName.CooldownRefund:
            CooldownRefund++;
            break;
          case EffectName.Lichbane:
            Lichbane++;
            break;
          case EffectName.RepearKill:
            RepearKill++;
            break;
          case EffectName.SweepingDash:
            SweepingDash++;
            break;
          default:
            Debug.Log($"{eff} does not exists in EffectName enum!");
            break;
        }
      }
    }

    /// <summary>
    /// For updating Leaderboards
    /// </summary>
    /// <param name="oldLeaderboards"></param>
    /// <param name="sessionDataModel"></param>
    public EffectsObtainedOccurancesModel(LeaderboardsModel oldLeaderboards, SessionDataModel sessionDataModel)
    {
      EmpoweredAttack = oldLeaderboards.EffectsObtained.EmpoweredAttack;
      Sunfire = oldLeaderboards.EffectsObtained.Sunfire;
      CooldownRefund = oldLeaderboards.EffectsObtained.CooldownRefund;
      Lichbane = oldLeaderboards.EffectsObtained.Lichbane;
      RepearKill = oldLeaderboards.EffectsObtained.RepearKill;
      SweepingDash = oldLeaderboards.EffectsObtained.SweepingDash;

      foreach (EffectName eff in sessionDataModel.EffectsObtained)
      {
        switch (eff)
        {
          case EffectName.EmpoweredAttack:
            EmpoweredAttack++;
            break;
          case EffectName.Sunfire:
            Sunfire++;
            break;
          case EffectName.CooldownRefund:
            CooldownRefund++;
            break;
          case EffectName.Lichbane:
            Lichbane++;
            break;
          case EffectName.RepearKill:
            RepearKill++;
            break;
          case EffectName.SweepingDash:
            SweepingDash++;
            break;
          default:
            Debug.Log($"{eff} does not exists in EffectName enum!");
            break;
        }
      }
    }

    public EffectName GetMostUsedEffects()
    {
      int occurances = 0;
      EffectName mostUsedEffect = EffectName.None;
      Dictionary<EffectName, int> result = new Dictionary<EffectName, int>
      {
        { EffectName.EmpoweredAttack, EmpoweredAttack },
        { EffectName.Sunfire, Sunfire },
        { EffectName.CooldownRefund, CooldownRefund},
        { EffectName.Lichbane, Lichbane },
        { EffectName.RepearKill, RepearKill },
        { EffectName.SweepingDash, SweepingDash },
      };

      foreach (KeyValuePair<EffectName, int> effect in result)
      {
        if (effect.Value >= occurances)
        {
          Debug.Log($"Most Used Effect is {effect.Key} with {effect.Value} occurances!");
          occurances = effect.Value;
          mostUsedEffect = effect.Key;
        }
      }

      return mostUsedEffect;
    }
  }

  public enum EffectName
  {
    None,
    EmpoweredAttack,  // ID 200
    Sunfire,          // ID 201
    CooldownRefund,   // ID 202
    Lichbane,         // ID 203
    RepearKill,       // ID 204
    SweepingDash,     // ID 205
  }

}