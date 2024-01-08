/*
using Firebase.Firestore;
using System;

[FirestoreData]
public class MatchHistoryModel
{
  [FirestoreProperty]
  public string Username { get; set; }
  [FirestoreProperty]
  public int RoundNumberReached { get; set; }
  [FirestoreProperty]
  public string SessionLength { get; set; }
  [FirestoreProperty]
  public int DamageTaken { get; set; }
  [FirestoreProperty]
  public int TotalScore { get; set; }
  [FirestoreProperty]
  public int GoldEarned { get; set; }
  [FirestoreProperty]
  public int MobsDefeatedCount { get; set; }
  [FirestoreProperty]
  public int BossesDefeatedCount { get; set; }
  [FirestoreProperty]
  public int MysticDungeonsEnteredCount { get; set; }
  [FirestoreProperty]
  public string BloodLine { get; set; }
  [FirestoreProperty]
  public string SkillUsed { get; set; }
  [FirestoreProperty]
  public int Attack { get; set; }
  [FirestoreProperty]
  public int Ability { get; set; }
  [FirestoreProperty]
  public string Health { get; set; }
  [FirestoreProperty]
  public string EffectsObtained { get; set; }
  [FirestoreProperty]
  public string Device { get; set; }
  [FirestoreProperty]
  public string Timestamp { get; set; }



  public MatchHistoryModel() { }

  public MatchHistoryModel(
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


*/