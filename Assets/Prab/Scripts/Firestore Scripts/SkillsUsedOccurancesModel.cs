using UnityEngine;
using System;

namespace ParaverseWebsite.Models
{
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
    /// <param name="sessionDataModel"></param>
    public SkillsUsedOccurancesModel(LeaderboardsModel oldLeaderboards, SessionDataModel sessionDataModel)
    {
      AzuriteInfusion = oldLeaderboards.SkillUsed.AzuriteInfusion;
      BladeWhirl = oldLeaderboards.SkillUsed.BladeWhirl;
      DescendingThrust = oldLeaderboards.SkillUsed.DescendingThrust;
      AvatarState = oldLeaderboards.SkillUsed.AvatarState;
      LightningBolt = oldLeaderboards.SkillUsed.LightningBolt;
      MoonlightSlash = oldLeaderboards.SkillUsed.MoonlightSlash;
      RegalCrescent = oldLeaderboards.SkillUsed.RegalCrescent;
      StealthStep = oldLeaderboards.SkillUsed.StealthStep;

      Debug.Log(AzuriteInfusion);
      Debug.Log(BladeWhirl);
      Debug.Log(DescendingThrust);
      Debug.Log(AvatarState);
      Debug.Log(LightningBolt);
      Debug.Log(MoonlightSlash);
      Debug.Log(RegalCrescent);
      Debug.Log(StealthStep);

      if (sessionDataModel.SkillUsedEnum.Equals(SkillName.AzuriteInfusion))
        AzuriteInfusion++;
      else if (sessionDataModel.SkillUsedEnum.Equals(SkillName.BladeWhirl))
        BladeWhirl++;
      else if (sessionDataModel.SkillUsedEnum.Equals(SkillName.DescendingThrust))
        DescendingThrust++;
      else if (sessionDataModel.SkillUsedEnum.Equals(SkillName.AvatarState))
        AvatarState++;
      else if (sessionDataModel.SkillUsedEnum.Equals(SkillName.LightningBolt))
        LightningBolt++;
      else if (sessionDataModel.SkillUsedEnum.Equals(SkillName.MoonlightSlash))
        MoonlightSlash++;
      else if (sessionDataModel.SkillUsedEnum.Equals(SkillName.RegalCrescent))
        RegalCrescent++;
      else if (sessionDataModel.SkillUsedEnum.Equals(SkillName.StealthStep))
        StealthStep++;
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
}