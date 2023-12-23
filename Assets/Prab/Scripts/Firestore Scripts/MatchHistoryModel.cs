using Firebase.Firestore;

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
