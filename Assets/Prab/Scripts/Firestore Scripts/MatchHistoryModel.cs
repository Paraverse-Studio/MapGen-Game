#if !UNITY_WEBGL || UNITY_EDITOR
using Firebase.Firestore;
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
[FirestoreData]
#endif
public class MatchHistoryModel
{
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
    public string Username { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int RoundNumberReached { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public string SessionLength { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int DamageTaken { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int TotalScore { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int GoldEarned { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int MobsDefeatedCount { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int BossesDefeatedCount { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int MysticDungeonsEnteredCount { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public string BloodLine { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public string SkillUsed { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int Attack { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int Ability { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public string Health { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public string EffectsObtained { get; set; }

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
    }
}
