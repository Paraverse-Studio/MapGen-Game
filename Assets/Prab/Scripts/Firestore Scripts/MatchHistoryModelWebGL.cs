using System;

[Serializable]
public class MatchHistoryModelWebGL
{
  public string Username;
  public int RoundNumberReached;
  public string SessionLength;
  public int DamageTaken;
  public int TotalScore;
  public int GoldEarned;
  public int MobsDefeatedCount;
  public int BossesDefeatedCount;
  public int MysticDungeonsEnteredCount;
  public string BloodLine;
  public string SkillUsed;
  public int Attack;
  public int Ability;
  public string Health;
  public string EffectsObtained;
  public string Device;
  public string Timestamp;


  public MatchHistoryModelWebGL() { }

  public MatchHistoryModelWebGL(
      string username,
      int roundNumberReached,
      string sessionLength,
      int damageTaken,
      int averageScore,
      int goldEarned,
      int mobsDefeatedCount,
      int bossesDefeatedCount,
      int mysticDungeonsEnteredCount,
      string bloodLine,
      string skillUsed,
      int attack,
      int ability,
      string health,
      string effectsObtained
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
    BloodLine = bloodLine;
    SkillUsed = skillUsed;
    Attack = attack;
    Ability = ability;
    Health = health;
    EffectsObtained = effectsObtained;
    Timestamp = DateTime.Now.ToString();
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
public enum DeviceType
{
  Mobile,
  WebGL,
  Desktop
}