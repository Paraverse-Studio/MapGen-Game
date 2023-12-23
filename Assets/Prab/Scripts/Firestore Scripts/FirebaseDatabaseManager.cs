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
    private Task<QuerySnapshot> _MatchHistorySnapShot;
    public MatchHistoryModel _MatchHistoryModel;



    private void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        // Init
        _db = FirebaseFirestore.DefaultInstance;
        _MatchHistorySnapShot = FirebaseFirestore.DefaultInstance.Collection("MatchHistory").GetSnapshotAsync();
        _MatchHistoryCollection = _db.Collection("MatchHistory");

        _MatchHistoryModel = new MatchHistoryModel
        {
            Username = "Prab",
            RoundNumberReached = 4,
            SessionLength = "4:20",
            DamageTaken = 690,
            TotalScore = 696969,
            GoldEarned = 420,
            MobsDefeatedCount = 42,
            BossesDefeatedCount = 5,
            MysticDungeonsEnteredCount = 3,
            BloodLine = "Period Blood",
            SkillUsed = "Azurite Infusion",
            Attack = 20,
            Ability = 30,
            Health = "100/100",
            EffectsObtained = "Reapers Kill"
        };

        //CreateMatchHistory(_MatchHistoryModel);
    }

    private void Update()
    {
    }

    public void CreateMatchHistory(MatchHistoryModel matchHistoryModel)
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
        document.SetAsync(model);
        Debug.Log(String.Format("Added document with ID: {0}.", document.Id));
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