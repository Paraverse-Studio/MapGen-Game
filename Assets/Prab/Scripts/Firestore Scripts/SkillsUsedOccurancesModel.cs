using Paraverse.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class SkillsUsedOccurancesModel
  {
    public int RegalCrescent = 0;
    public int MoonlightSlash = 0;
    public int AzuriteInfusion = 0;
    public int BladeWhirl = 0;
    public int DescendingThrust = 0;
    public int StealthStep = 0;
    public int LightningBolt = 0;
    public int AvatarState = 0;

    public SkillsUsedOccurancesModel() { }

    /// <summary>
    /// For updating Match History 
    /// </summary>
    /// <param name="skill"></param>
    public SkillsUsedOccurancesModel(SkillName skill)
    {
      switch (skill)
      {
        case SkillName.AvatarState:
          AvatarState++;
          break;
        case SkillName.LightningBolt:
          LightningBolt++;
          break;
        case SkillName.MoonlightSlash:
          MoonlightSlash++;
          break;
        case SkillName.AzuriteInfusion:
          AzuriteInfusion++;
          break;
        case SkillName.BladeWhirl:
          BladeWhirl++;
          break;
        case SkillName.DescendingThrust:
          DescendingThrust++;
          break;
        case SkillName.StealthStep:
          StealthStep++;
          break;
        case SkillName.RegalCrescent:
          RegalCrescent++;
          break;
        default:
          Debug.Log(skill.ToString() + " doesn't exists in SkillName enum!");
          break;
      }
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

      switch (sessionDataModel.SkillUsedEnum)
      {
        case SkillName.AvatarState:
          AvatarState++;
          break;
        case SkillName.LightningBolt:
          LightningBolt++;
          break;
        case SkillName.MoonlightSlash:
          MoonlightSlash++;
          break;
        case SkillName.AzuriteInfusion:
          AzuriteInfusion++;
          break;
        case SkillName.BladeWhirl:
          BladeWhirl++;
          break;
        case SkillName.DescendingThrust:
          DescendingThrust++;
          break;
        case SkillName.StealthStep:
          StealthStep++;
          break;
        case SkillName.RegalCrescent:
          RegalCrescent++;
          break;
        default:
          Debug.Log(sessionDataModel.SkillUsedEnum.ToString() + " doesn't exists in SkillName enum!");
          break;
      }
    }

    public SkillName GetMostUsedSkill()
    {
      int occurances = 1;
      SkillName mostUsedSkill = SkillName.None;
      Dictionary<SkillName, int> result = new Dictionary<SkillName, int>
    {
        { SkillName.None, 0 },
        { SkillName.RegalCrescent, RegalCrescent },
        { SkillName.MoonlightSlash, MoonlightSlash },
        { SkillName.AzuriteInfusion, AzuriteInfusion },
        { SkillName.DescendingThrust, DescendingThrust},
        { SkillName.BladeWhirl, BladeWhirl },
        { SkillName.StealthStep, AzuriteInfusion },
        { SkillName.LightningBolt, LightningBolt },
        { SkillName.AvatarState, AvatarState }
      };

      foreach (KeyValuePair<SkillName, int> skill in result)
      {
        if (skill.Value >= occurances)
        {
          occurances = skill.Value;
          mostUsedSkill = skill.Key;
        }
      }

      return mostUsedSkill;
    }
  }

  public enum SkillName
  {
    None,
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