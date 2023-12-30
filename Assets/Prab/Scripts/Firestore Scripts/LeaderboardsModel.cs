#if !UNITY_WEBGL || UNITY_EDITOR
using Firebase.Firestore;
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
[FirestoreData]
#endif
public class LeaderboardsModel
{
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
    public string Username { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int HighestRoundNumberReached { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public string HighestSessionLength { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int HighestDamageTaken { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int HighestTotalScore { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int HighestGoldEarned { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int HighestMobsDefeatedCount { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int HighestBossesDefeatedCount { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int HighestMysticDungeonsEnteredCount { get; set; }
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
  public int HighestAttack { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public int HighestAbility { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public string HighestHealth { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
  [FirestoreProperty]
#endif
  public string EffectsObtained { get; set; }

    public LeaderboardsModel() { }

    public LeaderboardsModel(
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
        HighestRoundNumberReached = roundNumberReached;
        HighestSessionLength = sessionLength;
        HighestDamageTaken = damageTaken;
        HighestTotalScore = averageScore;
        HighestGoldEarned = goldEarned;
        HighestMobsDefeatedCount = mobsDefeatedCount;
        HighestBossesDefeatedCount = bossesDefeatedCount;
        HighestMysticDungeonsEnteredCount = mysticDungeonsEnteredCount;
        BloodLine = bloodLine;
        SkillUsed = skillUsed;
        HighestAttack = attack;
        HighestAbility = ability;
        HighestHealth = health;
        EffectsObtained = effectsObtained;
    }
}
