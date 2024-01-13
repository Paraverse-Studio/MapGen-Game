using System;
using System.Collections.Generic;
using UnityEngine;


namespace ParaverseWebsite.Models
{
  public class SessionDataModel
  {
    public string Username;
    public int RoundNumberReached;
    public int GamesPlayed;
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
    public List<EffectName> EffectsObtained = new List<EffectName>();
    public string Device;
    public string Timestamp;

    public BloodlineType BloodLineEnum;
    public SkillName SkillUsedEnum;

    public SessionDataModel() { }

    public SessionDataModel(
      string username,
      int roundNumberReached,
      int gamesPlayed,
      int sessionLength,
      int damageTaken,
      int totalScore,
      int goldEarned,
      int mobsDefeatedCount,
      int bossesDefeatedCount,
      int mysticDungeonsEnteredCount,
      string health,
      int attack,
      int ability,
      string bloodLine,
      string skillUsed,
      List<EffectName> effectsObtained,
      BloodlineType bloodLineEnum,
      SkillName skillUsedEnum
      )
    {
      Username = username;
      RoundNumberReached = roundNumberReached;
      GamesPlayed = gamesPlayed;
      SessionLength = sessionLength;
      DamageTaken = damageTaken;
      TotalScore = totalScore;
      GoldEarned = goldEarned;
      Health = health;
      Attack = attack;
      Ability = ability;
      MobsDefeatedCount = mobsDefeatedCount;
      BossesDefeatedCount = bossesDefeatedCount;
      MysticDungeonsEnteredCount = mysticDungeonsEnteredCount;
      BloodLine = bloodLine;
      SkillUsed = skillUsed;
      EffectsObtained = effectsObtained;
      BloodLineEnum = bloodLineEnum;
      SkillUsedEnum = skillUsedEnum;
      Timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");

#if UNITY_EDITOR
      Device = DeviceType.Test.ToString();
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
      Device = DeviceType.WebGL.ToString();
#endif
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR|| UNITY_STANDALONE_WIN && !UNITY_EDITOR|| UNITY_STANDALONE_LINUX && !UNITY_EDITOR
    Device = DeviceType.Desktop.ToString();
#endif
#if UNITY_IOS && !UNITY_EDITOR || UNITY_ANDROID && !UNITY_EDITOR
    Device = DeviceType.Mobile.ToString();
#endif
    }
  }
}