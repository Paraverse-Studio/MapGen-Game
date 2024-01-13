using System;
using UnityEngine;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class LeaderboardsModel
  {
    public string Username;
    public int HighestRoundNumberReached;
    public int CumulativeGamesPlayed;
    public int CumulativeSessionLength;
    public int CumulativeTotalScore;
    public int CumulativeParaverseScore;
    public int CumulativeMobsDefeatedCount;
    public int CumulativeBossesDefeatedCount;
    public int CumulativeMysticDungeonsEnteredCount;
    public BloodlineOccurancesModel BloodLine;
    public SkillsUsedOccurancesModel SkillUsed;
    public EffectsObtainedOccurancesModel EffectsObtained;

    public LeaderboardsModel() { }

    public LeaderboardsModel(
        string username,
        int roundNumberReached,
        int gamesPlayed,
        int sessionLength,
        int totalScore,
        int mobsDefeatedCount,
        int bossesDefeatedCount,
        int mysticDungeonsEnteredCount,
        BloodlineOccurancesModel bloodLine,
        SkillsUsedOccurancesModel skillUsed,
        EffectsObtainedOccurancesModel effectsObtained
        )
    {
      Username = username;
      HighestRoundNumberReached = roundNumberReached;
      CumulativeGamesPlayed = gamesPlayed;
      CumulativeSessionLength = sessionLength;
      CumulativeTotalScore = totalScore;
      CumulativeParaverseScore = totalScore;
      CumulativeMobsDefeatedCount = mobsDefeatedCount;
      CumulativeBossesDefeatedCount = bossesDefeatedCount;
      CumulativeMysticDungeonsEnteredCount = mysticDungeonsEnteredCount;
      BloodLine = bloodLine;
      SkillUsed = skillUsed;
      EffectsObtained = effectsObtained;

      CumulativeGamesPlayed++;
    }

    public LeaderboardsModel(LeaderboardsModel oldLeaderboards, SessionDataModel sessionData)
    {
      Username = sessionData.Username;
      HighestRoundNumberReached = Mathf.Max(oldLeaderboards.HighestRoundNumberReached, sessionData.RoundNumberReached);
      CumulativeGamesPlayed = oldLeaderboards.CumulativeGamesPlayed + 1;
      CumulativeSessionLength = oldLeaderboards.CumulativeSessionLength + sessionData.SessionLength;
      CumulativeTotalScore = oldLeaderboards.CumulativeTotalScore + sessionData.TotalScore;
      CumulativeParaverseScore = oldLeaderboards.CumulativeTotalScore + sessionData.TotalScore; 
      CumulativeMobsDefeatedCount = oldLeaderboards.CumulativeMobsDefeatedCount + sessionData.MobsDefeatedCount;
      CumulativeBossesDefeatedCount = oldLeaderboards.CumulativeBossesDefeatedCount + sessionData.BossesDefeatedCount;
      CumulativeMysticDungeonsEnteredCount = oldLeaderboards.CumulativeMysticDungeonsEnteredCount + sessionData.MysticDungeonsEnteredCount;
      BloodLine = new BloodlineOccurancesModel(sessionData.BloodLineEnum);
      SkillUsed = new SkillsUsedOccurancesModel(sessionData.SkillUsedEnum);
      EffectsObtained = new EffectsObtainedOccurancesModel(sessionData.EffectsObtainedEnums);
    }
  }

}