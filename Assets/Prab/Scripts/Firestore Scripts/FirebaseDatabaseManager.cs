#if !UNITY_WEBGL
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
  public static FirebaseDatabaseManager Instance;
  private FirebaseFirestore _db;
  private CollectionReference _MatchHistoryCollection;
  private CollectionReference _LeaderboardsCollection;
  private Query _MatchHistoryQuery;
  private Query _LeaderboardsQuery;
  private Task<QuerySnapshot> _MatchHistorySnapShot;
  private Task<QuerySnapshot> _LeaderboardsSnapShot;
  public MatchHistoryModel _MatchHistoryModel;
  public LeaderboardsModel _LeaderboardsModel;




  private void Awake()
  {
    // Singleton
    if (Instance == null)
      Instance = this;
    else
      Destroy(this);

    // Init
    _db = FirebaseFirestore.DefaultInstance;
    _MatchHistoryQuery = _db.Collection("MatchHistory");
    _LeaderboardsQuery = _db.Collection("Leaderboards");
    _MatchHistorySnapShot = _db.Collection("MatchHistory").GetSnapshotAsync();
    _LeaderboardsSnapShot = _db.Collection("Leaderboards").GetSnapshotAsync();
    _MatchHistoryCollection = _db.Collection("MatchHistory");
    _LeaderboardsCollection = _db.Collection("Leaderboards");

    //_MatchHistoryModel = new MatchHistoryModel
    //{
    //  Username = "Prab",
    //  RoundNumberReached = 4,
    //  SessionLength = "4:20",
    //  DamageTaken = 690,
    //  TotalScore = 696969,
    //  GoldEarned = 420,
    //  MobsDefeatedCount = 42,
    //  BossesDefeatedCount = 5,
    //  MysticDungeonsEnteredCount = 3,
    //  BloodLine = "Period Blood",
    //  SkillUsed = "Azurite Infusion",
    //  Attack = 20,
    //  Ability = 30,
    //  Health = "100/100",
    //  EffectsObtained = "Reapers Kill"
    //};

    //CreateMatchHistory(_MatchHistoryModel);
    //CreateLeaderboards(_MatchHistoryModel);
    //UpdateLeaderboards(_MatchHistoryModel);
  }

  private void Update()
  {
  }

  public Task CreateMatchHistory(MatchHistoryModel matchHistoryModel)
  {
    DocumentReference document = _MatchHistoryCollection.Document(matchHistoryModel.Username);
    Dictionary<string, object> model = new Dictionary<string, object>
            {
                { StringData.Username, matchHistoryModel.Username },
                { StringData.RoundNumberReached, matchHistoryModel.RoundNumberReached },
                { StringData.SessionLength, matchHistoryModel.SessionLength },
                { StringData.DamageTaken, matchHistoryModel.DamageTaken },
                { StringData.TotalScore, matchHistoryModel.TotalScore },
                { StringData.GoldEarned, matchHistoryModel.GoldEarned },
                { StringData.MobsDefeatedCount, matchHistoryModel.MobsDefeatedCount },
                { StringData.BossesDefeatedCount, matchHistoryModel.BossesDefeatedCount },
                { StringData.MysticDungeonsEnteredCount, matchHistoryModel.MysticDungeonsEnteredCount },
                { StringData.BloodLine, matchHistoryModel.BloodLine },
                { StringData.SkillUsed, matchHistoryModel.SkillUsed },
                { StringData.Attack, matchHistoryModel.Attack },
                { StringData.Ability, matchHistoryModel.Ability },
                { StringData.Health, matchHistoryModel.Health },
                { StringData.EffectsObtained, matchHistoryModel.EffectsObtained },
            };
    Debug.Log(String.Format("Added document with ID: {0}.", document.Id));
    return document.SetAsync(model).ContinueWithOnMainThread(task => { });
  }

  public void GetLeaderboardsList()
  {
    foreach (DocumentSnapshot documentSnapshot in _LeaderboardsSnapShot.Result)
    {
      Console.WriteLine("Document data for {0} document:", documentSnapshot.Id);
      Dictionary<string, object> city = documentSnapshot.ToDictionary();
      foreach (KeyValuePair<string, object> pair in city)
      {
        Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
      }
      Console.WriteLine("");
    }
  }

  public async Task<DocumentSnapshot> GetLeaderboards(string docId)
  {
    DocumentReference docRef = _LeaderboardsCollection.Document(docId);
    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
    if (snapshot.Exists)
    {
      Console.WriteLine("Document data for {0} document:", snapshot.Id);
      Dictionary<string, object> leaderboards = snapshot.ToDictionary();
      foreach (KeyValuePair<string, object> pair in leaderboards)
      {
        Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
      }
      return snapshot;
    }
    else
    {
      Console.WriteLine("Document {0} does not exist!", snapshot.Id);
      return null;
    }
  }

  public Task CreateLeaderboards(LeaderboardsModel leaderboardsModel)
  {
    DocumentReference document = _LeaderboardsCollection.Document(leaderboardsModel.Username);
    Dictionary<string, object> model = new Dictionary<string, object>
            {
                { StringData.Username, leaderboardsModel.Username },
                { StringData.RoundNumberReached, leaderboardsModel.HighestRoundNumberReached },
                { StringData.SessionLength, leaderboardsModel.HighestSessionLength },
                { StringData.DamageTaken, leaderboardsModel.HighestDamageTaken },
                { StringData.TotalScore, leaderboardsModel.HighestTotalScore },
                { StringData.GoldEarned, leaderboardsModel.HighestGoldEarned },
                { StringData.MobsDefeatedCount, leaderboardsModel.HighestMobsDefeatedCount },
                { StringData.BossesDefeatedCount, leaderboardsModel.HighestBossesDefeatedCount },
                { StringData.MysticDungeonsEnteredCount, leaderboardsModel.HighestMysticDungeonsEnteredCount },
                { StringData.BloodLine, leaderboardsModel.BloodLine },
                { StringData.SkillUsed, leaderboardsModel.SkillUsed },
                { StringData.Attack, leaderboardsModel.HighestAttack },
                { StringData.Ability, leaderboardsModel.HighestAbility },
                { StringData.Health, leaderboardsModel.HighestHealth },
                { StringData.EffectsObtained, leaderboardsModel.EffectsObtained },
            };
    Debug.Log(String.Format("Created document with ID: {0}.", document.Id));
    return document.SetAsync(model).ContinueWithOnMainThread(task => { });
  }

  public Task UpdateLeaderboards(LeaderboardsModel leaderboardsModel)
  {
    DocumentReference document = _LeaderboardsCollection.Document(leaderboardsModel.Username);
    Dictionary<string, object> model = new Dictionary<string, object>
            {
                { StringData.Username, leaderboardsModel.Username },
                { StringData.RoundNumberReached, leaderboardsModel.HighestRoundNumberReached },
                { StringData.SessionLength, leaderboardsModel.HighestSessionLength },
                { StringData.DamageTaken, leaderboardsModel.HighestDamageTaken },
                { StringData.TotalScore, leaderboardsModel.HighestTotalScore },
                { StringData.GoldEarned, leaderboardsModel.HighestGoldEarned },
                { StringData.MobsDefeatedCount, leaderboardsModel.HighestMobsDefeatedCount },
                { StringData.BossesDefeatedCount, leaderboardsModel.HighestBossesDefeatedCount },
                { StringData.MysticDungeonsEnteredCount, leaderboardsModel.HighestMysticDungeonsEnteredCount },
                { StringData.BloodLine, leaderboardsModel.BloodLine },
                { StringData.SkillUsed, leaderboardsModel.SkillUsed },
                { StringData.Attack, leaderboardsModel.HighestAttack },
                { StringData.Ability, leaderboardsModel.HighestAbility },
                { StringData.Health, leaderboardsModel.HighestHealth },
                { StringData.EffectsObtained, leaderboardsModel.EffectsObtained },
            };
    Debug.Log(String.Format("Updated document with ID: {0}.", document.Id));
    return document.UpdateAsync(model).ContinueWithOnMainThread(task => { });
  }
}

//public IEnumerator GetUser(MatchHistoryModel user, Action<MatchHistoryModel> callback)
//{
//    IList<MatchHistoryModel> userList = new List<MatchHistoryModel>();
//    int userCount = 0;
//    _MatchHistorySnapShot.ContinueWithOnMainThread(task =>
//    {
//        var collection = task.Result;
//        if (collection.Count == 0)
//            Debug.LogError("There are no users available in the collection 'Users'.");

//        userCount = collection.Count;
//        foreach (var userData in collection.Documents)
//        {
//            var userName = userData.ToDictionary()["Username"] as string;

//            if (userName == user.Username)
//            {
//                var id = userData.ToDictionary()["Id"] as string;
//                var score = userData.ToDictionary()["Score"];
//                Debug.Log("User: " + id + ", Username: " + userName + " has a score of " + score);

//                user = new MatchHistoryModel(userName, Convert.ToInt32(score));
//                break;
//            }
//        }

//    });
//    yield return new WaitUntil(() => userList.Count == userCount && userList.Count != 0);
//    Debug.Log("Done Getting User: " + user.Id + ", Username: " + user.Username + " has a score of " + user.Score);
//    callback(user);
//}

//public IEnumerator GetUsersList(Action<IList<MatchHistoryModel>> callback)
//{
//    IList<MatchHistoryModel> userList = new List<MatchHistoryModel>();
//    int userCount = 0;
//    _MatchHistorySnapShot.ContinueWithOnMainThread(task =>
//    {
//        var collection = task.Result;
//        if (collection.Count == 0)
//            Debug.LogError("There are no users available in the collection 'Users'.");

//        userCount = collection.Count;
//        Debug.Log("Found " + userCount + " users!");

//        foreach (var userData in collection.Documents)
//        {
//            var id = userData.ToDictionary()["Id"] as string;
//            var userName = userData.ToDictionary()["Username"] as string;
//            var score = userData.ToDictionary()["Score"];
//            Debug.Log("User: " + id + ", Username: " + userName + " has a score of " + score);
//        }

//    });
//    yield return new WaitUntil(() => userList.Count == userCount && userList.Count != 0);
//    Debug.Log("Done getting " + userCount + " users!");
//    callback(userList);
//}

#endif