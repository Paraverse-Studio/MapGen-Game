//#if !UNITY_WEBGL
using Firebase.Extensions;
using Firebase.Firestore;
using ParaverseWebsite.Models;
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

  public List<LeaderboardsModel> _LeaderboardsModels = new List<LeaderboardsModel>();




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
  }

  private void Update()
  {
  }

  public Task CreateMatchHistory(MatchHistoryModel matchHistoryModel)
  {
    System.Random rnd = new System.Random();
    int randomNum = rnd.Next();
    string id = matchHistoryModel.Username + "-" + randomNum;

    DocumentReference docRef = _MatchHistoryCollection.Document(id);
    MatchHistoryModel model = new MatchHistoryModel(
                matchHistoryModel.Username,
                matchHistoryModel.RoundNumberReached,
                matchHistoryModel.SessionLength,
                matchHistoryModel.DamageTaken,
                matchHistoryModel.TotalScore,
                matchHistoryModel.GoldEarned,
                matchHistoryModel.MobsDefeatedCount,
                matchHistoryModel.BossesDefeatedCount,
                matchHistoryModel.MysticDungeonsEnteredCount,
                matchHistoryModel.BloodLine,
                matchHistoryModel.SkillUsed,
                matchHistoryModel.Attack,
                matchHistoryModel.Ability,
                matchHistoryModel.Health,
                matchHistoryModel.EffectsObtained);
    return docRef.SetAsync(model).ContinueWithOnMainThread(task => { });
  }

  public async Task<bool> LeaderboardsExists(string id)
  {
    DocumentReference docRef = _LeaderboardsCollection.Document(id);
    DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();

    if (docSnap.Exists) return true;
    return false;
  }

  public List<LeaderboardsModel> GetLeaderboardsList()
  {
    _LeaderboardsModels.Clear();
    foreach (DocumentSnapshot docSnapshot in _LeaderboardsSnapShot.Result)
    {
      Console.WriteLine("Document data for {0} document:", docSnapshot.Id);
      LeaderboardsModel leaderboardsModel = docSnapshot.ConvertTo<LeaderboardsModel>();
      _LeaderboardsModels.Add(leaderboardsModel);
    }
    return _LeaderboardsModels;
  }

  public async Task<LeaderboardsModel> GetLeaderboards(string docId)
  {
    DocumentReference docRef = _LeaderboardsCollection.Document(docId);
    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
    if (snapshot.Exists)
    {
      LeaderboardsModel leaderboardsModel = snapshot.ConvertTo<LeaderboardsModel>();
      return leaderboardsModel;
    }
    else
    {
      Console.WriteLine("Document {0} does not exist!", snapshot.Id);
      return null;
    }
  }
  public async Task<UserModel> GetUser(string userId)
  {
    DocumentReference docRef = _LeaderboardsCollection.Document(userId);
    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
    if (snapshot.Exists)
    {
      UserModel userModel = snapshot.ConvertTo<UserModel>();
      return userModel;
    }
    return null;
  }

  public Task CreateLeaderboards(LeaderboardsModel leaderboardsModel)
  {
    DocumentReference document = _LeaderboardsCollection.Document(leaderboardsModel.Username);
    LeaderboardsModel model = new LeaderboardsModel(
                leaderboardsModel.Username,
                leaderboardsModel.HighestRoundNumberReached,
                leaderboardsModel.HighestSessionLength,
                leaderboardsModel.HighestDamageTaken,
                leaderboardsModel.HighestTotalScore,
                leaderboardsModel.HighestGoldEarned,
                leaderboardsModel.HighestMobsDefeatedCount,
                leaderboardsModel.HighestBossesDefeatedCount,
                leaderboardsModel.HighestMysticDungeonsEnteredCount,
                leaderboardsModel.BloodLine,
                leaderboardsModel.SkillUsed,
                leaderboardsModel.HighestAttack,
                leaderboardsModel.HighestAbility,
                leaderboardsModel.HighestHealth,
                leaderboardsModel.EffectsObtained);
    return document.SetAsync(model).ContinueWithOnMainThread(task => { });
  }
}
