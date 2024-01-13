using ParaverseWebsite.Models;
using System;
using System.Collections.Generic;

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
      if (eff.Equals(EffectName.EmpoweredAttack))
        EmpoweredAttack++;
      else if (eff.Equals(EffectName.Sunfire))
        Sunfire++;
      else if (eff == EffectName.CooldownRefund)
        CooldownRefund++;
      else if (eff == EffectName.Lichbane)
        Lichbane++;
      else if (eff == EffectName.RepearKill)
        RepearKill++;
      else if (eff == EffectName.SweepingDash)
        SweepingDash++;
    }
  }

  /// <summary>
  /// For updating Leaderboards
  /// </summary>
  /// <param name="oldLeaderboards"></param>
  /// <param name="matchHistoryModel"></param>
  public EffectsObtainedOccurancesModel(LeaderboardsModel oldLeaderboards, MatchHistoryModel matchHistoryModel)
  {
    EmpoweredAttack = oldLeaderboards.EffectsObtained.EmpoweredAttack;
    Sunfire = oldLeaderboards.EffectsObtained.Sunfire;
    CooldownRefund = oldLeaderboards.EffectsObtained.CooldownRefund;
    Lichbane = oldLeaderboards.EffectsObtained.Lichbane;
    RepearKill = oldLeaderboards.EffectsObtained.RepearKill;
    SweepingDash = oldLeaderboards.EffectsObtained.SweepingDash;

    foreach (EffectName eff in matchHistoryModel.EffectsObtained)
    {
      if (eff.Equals(EffectName.EmpoweredAttack))
        EmpoweredAttack++;
      else if (eff.Equals(EffectName.Sunfire))
        Sunfire++;
      else if (eff.Equals(EffectName.CooldownRefund))
        CooldownRefund++;
      else if (eff.Equals(EffectName.Lichbane))
        Lichbane++;
      else if (eff.Equals(EffectName.RepearKill))
        RepearKill++;
      else if (eff.Equals(EffectName.SweepingDash))
        SweepingDash++; 
    }
  }
}

public enum EffectName
{
  EmpoweredAttack,  // ID 200
  Sunfire,          // ID 201
  CooldownRefund,   // ID 202
  Lichbane,         // ID 203
  RepearKill,       // ID 204
  SweepingDash,     // ID 205
}
