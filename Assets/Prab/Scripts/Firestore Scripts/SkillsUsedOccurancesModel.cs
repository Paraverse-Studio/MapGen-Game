using ParaverseWebsite.Models;
using System;

[Serializable]
public class SkillsUsedOccurancesModel
{
  public int AzuriteInfusion = 0;
  public int BladeWhirl = 0;
  public int DescendingThrust = 0;
  public int AvatarState = 0;
  public int LightningBolt = 0;
  public int MoonlightSlash = 0;
  public int RegalCrescent = 0;
  public int StealthStep = 0;

  public SkillsUsedOccurancesModel() { }

  /// <summary>
  /// For updating Match History 
  /// </summary>
  /// <param name="skill"></param>
  public SkillsUsedOccurancesModel(SkillName skill)
  {
    if (skill.Equals(SkillName.AzuriteInfusion))
      AzuriteInfusion++;
    else if (skill.Equals(SkillName.BladeWhirl))
      BladeWhirl++;
    else if (skill.Equals(SkillName.DescendingThrust))
      DescendingThrust++;
    else if (skill.Equals(SkillName.AvatarState))
      AvatarState++;
    else if (skill.Equals(SkillName.LightningBolt))
      LightningBolt++;
    else if (skill.Equals(SkillName.MoonlightSlash))
      MoonlightSlash++;
    else if (skill.Equals(SkillName.RegalCrescent))
      RegalCrescent++;
    else if (skill.Equals(SkillName.StealthStep))
      StealthStep++;
  }

  /// <summary>
  /// For updating Leaderboards
  /// </summary>
  /// <param name="oldLeaderboards"></param>
  /// <param name="matchHistoryModel"></param>
  public SkillsUsedOccurancesModel(LeaderboardsModel oldLeaderboards, MatchHistoryModel matchHistoryModel)
  {
    if (matchHistoryModel.SkillUsed.Equals(SkillName.AzuriteInfusion))
      AzuriteInfusion = ++oldLeaderboards.SkillUsed.AzuriteInfusion;
    else if (matchHistoryModel.SkillUsed.Equals(SkillName.BladeWhirl))
      BladeWhirl = ++oldLeaderboards.SkillUsed.BladeWhirl;
    else if (matchHistoryModel.SkillUsed.Equals(SkillName.DescendingThrust))
      DescendingThrust = ++oldLeaderboards.SkillUsed.DescendingThrust;
    else if (matchHistoryModel.SkillUsed.Equals(SkillName.AvatarState))
      AvatarState = ++oldLeaderboards.SkillUsed.AvatarState;
    else if (matchHistoryModel.SkillUsed.Equals(SkillName.LightningBolt))
      LightningBolt = ++oldLeaderboards.SkillUsed.LightningBolt;
    else if (matchHistoryModel.SkillUsed.Equals(SkillName.MoonlightSlash))
      MoonlightSlash = ++oldLeaderboards.SkillUsed.MoonlightSlash;
    else if (matchHistoryModel.SkillUsed.Equals(SkillName.RegalCrescent))
      RegalCrescent = ++oldLeaderboards.SkillUsed.RegalCrescent;
    else if (matchHistoryModel.SkillUsed.Equals(SkillName.StealthStep))
      StealthStep = ++oldLeaderboards.SkillUsed.StealthStep;
  }
}

public enum SkillName
{
  RegalCrescent,      // ID 100
  MoonlightSlash,     // ID 101
  AzuriteInfusion,    // ID 102
  DescendingThrust,   // ID 103
  BladeWhirl,         // ID 104
  StealthStep,        // ID 105
  LightningBolt,      // ID 107
  AvatarState,        // ID 106
}
