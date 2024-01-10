//#if !UNITY_WEBGL 
//using Firebase.Extensions;
//using Firebase.Firestore;
//#endif
using FullSerializer;
using ParaverseWebsite.Models;
using Proyecto26;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
  public static FirebaseDatabaseManager Instance;
  private readonly string databasePath = "https://paraverse-games-default-rtdb.firebaseio.com/";
  private readonly string matchHistoriesPath = "MatchHistories";
  private readonly string leaderboardsPath = "Leaderboards";

  private static fsSerializer serializer = new fsSerializer();
  public delegate void PostMatchHistoryCallback(MatchHistoryModel matchHistory);
  public delegate void PostLeaderboardsCallback(LeaderboardsModel leaderboard);
  public delegate void GetLeaderboardCallback(LeaderboardsModel model);
  public delegate void GetLeaderboardsCallback(Dictionary<string, LeaderboardsModel> model);


  private void Awake()
  {
    // Singleton
    if (Instance == null) Instance = this;
    else Destroy(this);
  }

  #region MATCH HISTORY CRUD OPERATIONS

  public void PostMatchHistory(MatchHistoryModel model, PostMatchHistoryCallback callback)
  {
    System.Random rnd = new System.Random();
    int randomNum = rnd.Next();
    string id = model.Username + "-" + randomNum;

    RestClient.Put<MatchHistoryModel>($"{databasePath}{matchHistoriesPath}/{id}.json", model)
      .Then(response =>
      {
        callback?.Invoke(response);
      })
      .Catch(error =>
      {
        Debug.Log("Error: " + error);
      });
  }

  #endregion

  #region LEADERBOARDS CRUD OPERATIONS

  public void PostLeaderboards(LeaderboardsModel model, PostLeaderboardsCallback callback)
  {
    string username = model.Username;

    RestClient.Put<LeaderboardsModel>($"{databasePath}{leaderboardsPath}/{username}.json", model)
      .Then(response =>
      {
        callback?.Invoke(response);
      })
      .Catch(error =>
      {
        Debug.Log("Error: " + error);
      });
  }

  public void GetLeaderboard(string username, GetLeaderboardCallback callback)
  {
    RestClient.Get<LeaderboardsModel>($"{databasePath}{leaderboardsPath}/{username}.json")
      .Then(response =>
      {
        Debug.Log("Rsponse: " + response);
        callback?.Invoke(response);
      })
      .Catch(error =>
      {
        Debug.Log("Error: " + error);
      });
  }

  public void GetLeaderboards(GetLeaderboardsCallback callback)
  {
    RestClient.Get($"{databasePath}{leaderboardsPath}.json")
      .Then(response =>
      {
        var responseJson = response.Text;

        // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
        // to serialize more complex types (a Dictionary, in this case)
        var data = fsJsonParser.Parse(responseJson);
        object deserialized = null;
        serializer.TryDeserialize(data, typeof(Dictionary<string, LeaderboardsModel>), ref deserialized);

        var leaderboardsDictionary = deserialized as Dictionary<string, LeaderboardsModel>;

        callback?.Invoke(leaderboardsDictionary);
      })
      .Catch(error =>
      {
        Debug.Log("Error: " + error);
      });
  }
  #endregion
}
