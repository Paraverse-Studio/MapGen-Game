//#if !UNITY_WEBGL 
//using Firebase.Extensions;
//using Firebase.Firestore;
//#endif
using FullSerializer;
using ParaverseWebsite.Models;
using Proyecto26;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
  public static FirebaseDatabaseManager Instance;
  private readonly string databasePath = "https://paraverse-games-default-rtdb.firebaseio.com/";
  private readonly string matchHistoriesPath = "MatchHistories";
  private readonly string leaderboardsPath = "Leaderboards";
  private readonly string usersPath = "Users";

  private static fsSerializer serializer = new fsSerializer();

  public delegate void PostUserCallback(UserModel user);
  public delegate void PostMatchHistoryCallback(SessionDataModel matchHistory);

  public delegate void UpdateLeaderboardCallback(LeaderboardsModel leaderboard);
  public delegate void PostLeaderboardFailureCallback();
  public delegate void GetLeaderboardCallback(LeaderboardsModel model);

  public delegate void GetLeaderboardsCallback(Dictionary<string, LeaderboardsModel> model);
  
  public delegate void GetUserCallback(UserModel model);
  public delegate void GetUserFailureCallback();


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

    RestClient.Put<ParaverseWebsite.Models.SessionDataModel>($"{databasePath}{matchHistoriesPath}/{id}.json", model)
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

  public void PostLeaderboards(LeaderboardsModel model, UpdateLeaderboardCallback callback)
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

  public void GetLeaderboard(string username, GetLeaderboardCallback onSuccessCallback, PostLeaderboardFailureCallback onFailureCallback)
  {
    try
    {
      RestClient.Get<LeaderboardsModel>($"{databasePath}{leaderboardsPath}/{username}.json")
        .Then(response =>
        {
          onSuccessCallback?.Invoke(response);
        })
        .Catch(error =>
        {
          Debug.Log("Error: " + error);
          onFailureCallback?.Invoke();
        });
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
      onFailureCallback?.Invoke();
    }
  }

  public void GetLeaderboards(GetLeaderboardsCallback callback)
  {
    try
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
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }
  #endregion


  #region USERS CRUD OPERATIONS

  public void PostUser(UserModel model, PostUserCallback callback)
  {
    RestClient.Put<UserModel>($"{databasePath}{usersPath}/{model.Username}.json", model)
      .Then(response =>
      {
        callback?.Invoke(response);
        MainMenuController.Instance.Username = response.Username;
      })
      .Catch(error =>
      {
        Debug.Log("Error: " + error);
      });
  }

  public void GetUser(string username, GetUserCallback onSuccessCallback, GetUserFailureCallback onFailureCallback)
  {
    try
    {
      RestClient.Get<UserModel>($"{databasePath}{usersPath}/{username}.json")
        .Then(response =>
        {
          onSuccessCallback?.Invoke(response);
        })
        .Catch(error =>
        {
          Debug.Log("Error: " + error);
          onFailureCallback?.Invoke();
        });
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
      onFailureCallback?.Invoke();
    }
  }
  #endregion
}
