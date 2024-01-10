using System;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class LeaderboardsModel
  {
    public string Username;
    public int HighestRoundNumberReached;
    public int CumulativeSessionLength;
    public int CumulativeTotalScore;
    public int CumulativeParaverseScore;
    public int CumulativeMobsDefeatedCount;
    public int CumulativeBossesDefeatedCount;
    public int CumulativeMysticDungeonsEnteredCount;
    public string BloodLine;
    public string SkillUsed;
    public string EffectsObtained;

    public LeaderboardsModel() { }

    public LeaderboardsModel(
        string username,
        int roundNumberReached,
        int sessionLength,
        int totalScore,
        int paraverseScore,
        int mobsDefeatedCount,
        int bossesDefeatedCount,
        int mysticDungeonsEnteredCount,
        string bloodLine,
        string skillUsed,
        string effectsObtained
        )
    {
      Username = username;
      HighestRoundNumberReached = roundNumberReached;
      CumulativeSessionLength = sessionLength;
      CumulativeTotalScore = totalScore;
      CumulativeParaverseScore = totalScore;
      CumulativeMobsDefeatedCount = mobsDefeatedCount;
      CumulativeBossesDefeatedCount = bossesDefeatedCount;
      CumulativeMysticDungeonsEnteredCount = mysticDungeonsEnteredCount;
      BloodLine = bloodLine;
      SkillUsed = skillUsed;
      EffectsObtained = effectsObtained;
    }
  }

}