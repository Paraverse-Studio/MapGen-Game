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
      Debug.Log($"games played: {gamesPlayed}");
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
      Debug.Log($"Cumulative Games Played: {CumulativeGamesPlayed}");
    }

    public LeaderboardsModel(LeaderboardsModel oldLeaderboards, SessionDataModel sessionDataModel)
    {
      Debug.Log($"Previous Cumulative Games Played: {oldLeaderboards.CumulativeGamesPlayed}");
      Username = sessionDataModel.Username;
      HighestRoundNumberReached = Mathf.Max(oldLeaderboards.HighestRoundNumberReached, sessionDataModel.RoundNumberReached);
      CumulativeGamesPlayed = oldLeaderboards.CumulativeGamesPlayed + 1;
      CumulativeSessionLength = oldLeaderboards.CumulativeSessionLength + sessionDataModel.SessionLength;
      CumulativeTotalScore = oldLeaderboards.CumulativeTotalScore + sessionDataModel.TotalScore;
      CumulativeParaverseScore = oldLeaderboards.CumulativeTotalScore + sessionDataModel.TotalScore; 
      CumulativeMobsDefeatedCount = oldLeaderboards.CumulativeMobsDefeatedCount + sessionDataModel.MobsDefeatedCount;
      CumulativeBossesDefeatedCount = oldLeaderboards.CumulativeBossesDefeatedCount + sessionDataModel.BossesDefeatedCount;
      CumulativeMysticDungeonsEnteredCount = oldLeaderboards.CumulativeMysticDungeonsEnteredCount + sessionDataModel.MysticDungeonsEnteredCount;
      BloodLine = new BloodlineOccurancesModel(oldLeaderboards, sessionDataModel);
      SkillUsed = new SkillsUsedOccurancesModel(oldLeaderboards, sessionDataModel);
      EffectsObtained = new EffectsObtainedOccurancesModel(oldLeaderboards, sessionDataModel);
      Debug.Log($"Current Cumulative Games Played: {CumulativeGamesPlayed}");
    }
  }

}