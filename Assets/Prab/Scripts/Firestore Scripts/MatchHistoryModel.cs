using System;
using System.Collections.Generic;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class MatchHistoryModel
  {
    public string Username;
    public int RoundNumberReached;
    public int SessionLength;
    public int DamageTaken;
    public int TotalScore;
    public int GoldEarned;
    public string Health;
    public int Attack;
    public int Ability;
    public int MobsDefeatedCount;
    public int BossesDefeatedCount;
    public int MysticDungeonsEnteredCount;
    public string BloodLine;
    public string SkillUsed;
    public EffectsObtainedOccurancesModel EffectsObtained = new EffectsObtainedOccurancesModel();
    public string Device;
    public string Timestamp;


    public MatchHistoryModel() { }

    public MatchHistoryModel(
        string username,
        int roundNumberReached,
        int sessionLength,
        int damageTaken,
        int averageScore,
        int goldEarned,
        int mobsDefeatedCount,
        int bossesDefeatedCount,
        int mysticDungeonsEnteredCount,
        string health,
        int attack,
        int ability,
        string bloodLine,
        string skillUsed,
        List<EffectName> effectsObtained
        )
    {
      Username = username;
      RoundNumberReached = roundNumberReached;
      SessionLength = sessionLength;
      DamageTaken = damageTaken;
      TotalScore = averageScore;
      GoldEarned = goldEarned;
      MobsDefeatedCount = mobsDefeatedCount;
      BossesDefeatedCount = bossesDefeatedCount;
      MysticDungeonsEnteredCount = mysticDungeonsEnteredCount;
      Health = health;
      Attack = attack;
      Ability = ability;
      BloodLine = bloodLine;
      SkillUsed = skillUsed;
      EffectsObtained = new EffectsObtainedOccurancesModel(effectsObtained);
      Timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");

#if UNITY_EDITOR
      Device = DeviceType.Test.ToString();
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
      Device = DeviceType.WebGL.ToString();
#endif
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR || UNITY_STANDALONE_WIN && !UNITY_EDITOR || UNITY_STANDALONE_LINUX && !UNITY_EDITOR
    Device = DeviceType.Desktop.ToString();
#endif
#if UNITY_IOS && !UNITY_EDITOR || UNITY_ANDROID && !UNITY_EDITOR
      Device = DeviceType.Mobile.ToString();
#endif
    }
  }
}
public enum DeviceType
{
  Test,
  Mobile,
  WebGL,
  Desktop
}