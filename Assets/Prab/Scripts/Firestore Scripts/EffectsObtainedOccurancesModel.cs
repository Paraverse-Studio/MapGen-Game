using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public EffectsObtainedOccurancesModel(Dictionary<EffectName, int> effects)
    {
      foreach (KeyValuePair<EffectName, int> eff in effects)
      {
        switch (eff.Key)
        {
          case EffectName.EmpoweredAttack:
            EmpoweredAttack = eff.Value;
            break;
          case EffectName.Sunfire:
            Sunfire = eff.Value;
            break;
          case EffectName.CooldownRefund:
            CooldownRefund = eff.Value;
            break;
          case EffectName.Lichbane:
            Lichbane = eff.Value;
            break;
          case EffectName.RepearKill:
            RepearKill = eff.Value;
            break;
          case EffectName.SweepingDash:
            SweepingDash = eff.Value;
            break;
          default:
            Debug.Log($"{eff} does not exist in EffectName enum!");
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

      foreach (KeyValuePair<EffectName, int> eff in sessionDataModel.EffectsObtained)
      {
        switch (eff.Key)
        {
          case EffectName.EmpoweredAttack:
            EmpoweredAttack = eff.Value;
            break;
          case EffectName.Sunfire:
            Sunfire = eff.Value;
            break;
          case EffectName.CooldownRefund:
            CooldownRefund = eff.Value;
            break;
          case EffectName.Lichbane:
            Lichbane = eff.Value;
            break;
          case EffectName.RepearKill:
            RepearKill = eff.Value;
            break;
          case EffectName.SweepingDash:
            SweepingDash = eff.Value;
            break;
          default:
            Debug.Log($"{eff} does not exist in EffectName enum!");
            break;
        }
      }
    }

    public List<EffectName> GetMostUsedEffects()
    {
      int idx = 0;
      List<EffectName> mostUsedEffect = new List<EffectName>();
      Dictionary<EffectName, int> result = new Dictionary<EffectName, int>
      {
        { EffectName.None, 0 },
        { EffectName.EmpoweredAttack, EmpoweredAttack },
        { EffectName.Sunfire, Sunfire },
        { EffectName.CooldownRefund, CooldownRefund},
        { EffectName.Lichbane, Lichbane },
        { EffectName.RepearKill, RepearKill },
        { EffectName.SweepingDash, SweepingDash },
      };

      foreach (KeyValuePair<EffectName, int> effect in result.OrderByDescending(key => key.Value))
      {
        if (idx >= 3) break;
        if (idx == 0 && effect.Value <= 0)
        {
          mostUsedEffect.Add(effect.Key);
          Debug.Log($"Most Used Effect is {effect.Key} with {effect.Value} occurances!");
          break;
        }
        mostUsedEffect.Add(effect.Key);
        idx++;
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