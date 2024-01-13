using System;
using System.Collections.Generic;


public class SessionDataModel
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
  public List<string> EffectsObtained = new List<string>();
  public string Device;
  public string Timestamp;

  public BloodlineType BloodLineEnum;
  public SkillName SkillUsedEnum;
  public List<EffectName> EffectsObtainedEnums;

  public SessionDataModel() { }

  public SessionDataModel(
    string username,
    int roundNumberReached,
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
    List<string> effectsObtained,
    BloodlineType bloodLineEnum,
    SkillName skillUsedEnum,
    List<EffectName> effectsObtainedEnums
    )
  {
    Username = username;
    RoundNumberReached = roundNumberReached;
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
    EffectsObtainedEnums = effectsObtainedEnums;
    Timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
#if UNITY_WEBGL
    Device = DeviceType.WebGL.ToString();
#endif
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
    Device = DeviceType.Desktop.ToString();
#endif
#if UNITY_IOS || UNITY_ANDROID
    Device = DeviceType.Mobile.ToString();
#endif
  }
}