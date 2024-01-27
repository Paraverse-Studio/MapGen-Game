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

  public delegate void GetUsersCallback(Dictionary<string, UserModel> models);

  #region Skill/Effect Sprites Data
  public Sprite VagabondSprite;
  public Sprite HarrierSprite;
  public Sprite PioneerSprite;
  public Sprite ScholarSprite;

  public Sprite NoSkillSprite;
  public Sprite RegalCrescentSprite;
  public Sprite MoonlightSlashSprite;
  public Sprite DescendingThrustSprite;
  public Sprite AzuriteInfusionSprite;
  public Sprite BladeWhirlSprite;
  public Sprite StealthStepSprite;
  public Sprite LightningBoltSprite;
  public Sprite AvatarStateSprite;

  public Sprite EmpoweredAttackSprite;
  public Sprite SunfireSprite;
  public Sprite CooldownRefundSprite;
  public Sprite LichbaneSprite;
  public Sprite RepearKillSprite;
  public Sprite SweepingDashSprite;
  #endregion


  private void Awake()
  {
    // Singleton
    if (Instance == null) Instance = this;
    else Destroy(this);
    
    DataMapper.BloodlineSpriteMapper.Add(BloodlineType.Vagabond, VagabondSprite);
    DataMapper.BloodlineSpriteMapper.Add(BloodlineType.Harrier, HarrierSprite);
    DataMapper.BloodlineSpriteMapper.Add(BloodlineType.Pioneer, PioneerSprite);
    DataMapper.BloodlineSpriteMapper.Add(BloodlineType.Scholar, ScholarSprite);

    DataMapper.SkillSpriteMapper.Add(SkillName.None, NoSkillSprite);
    DataMapper.SkillSpriteMapper.Add(SkillName.RegalCrescent, RegalCrescentSprite);
    DataMapper.SkillSpriteMapper.Add(SkillName.MoonlightSlash, MoonlightSlashSprite);
    DataMapper.SkillSpriteMapper.Add(SkillName.DescendingThrust, DescendingThrustSprite);
    DataMapper.SkillSpriteMapper.Add(SkillName.AzuriteInfusion, AzuriteInfusionSprite);
    DataMapper.SkillSpriteMapper.Add(SkillName.BladeWhirl, BladeWhirlSprite);
    DataMapper.SkillSpriteMapper.Add(SkillName.StealthStep, StealthStepSprite);
    DataMapper.SkillSpriteMapper.Add(SkillName.LightningBolt, LightningBoltSprite);
    DataMapper.SkillSpriteMapper.Add(SkillName.AvatarState, AvatarStateSprite);

    DataMapper.EffectSpriteMapper.Add(EffectName.None, NoSkillSprite);
    DataMapper.EffectSpriteMapper.Add(EffectName.EmpoweredAttack, EmpoweredAttackSprite);
    DataMapper.EffectSpriteMapper.Add(EffectName.Sunfire, SunfireSprite);
    DataMapper.EffectSpriteMapper.Add(EffectName.CooldownRefund, CooldownRefundSprite);
    DataMapper.EffectSpriteMapper.Add(EffectName.Lichbane, LichbaneSprite);
    DataMapper.EffectSpriteMapper.Add(EffectName.RepearKill, RepearKillSprite);
    DataMapper.EffectSpriteMapper.Add(EffectName.SweepingDash, SweepingDashSprite);
  }

  #region MATCH HISTORY CRUD OPERATIONS

  public void PostMatchHistory(MatchHistoryModel model, PostMatchHistoryCallback callback)
  {
    string id = DateTime.Now.ToString("yy-MM-dd-HH:mm:ss:ffffff");

    RestClient.Put<SessionDataModel>($"{databasePath}{matchHistoriesPath}/{model.Username}/{id}.json", model)
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
      .Then(user =>
      {
        callback?.Invoke(user);
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
        .Then(user =>
        {
          onSuccessCallback?.Invoke(user);
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

  public void GetUsers(GetUsersCallback onSuccessCallback, GetUserFailureCallback onFailureCallback)
  {
    try
    {
      RestClient.Get($"{databasePath}{usersPath}.json")
        .Then(response =>
        {
          var responseJson = response.Text;

          var data = fsJsonParser.Parse(responseJson);
          object deserialized = null;
          serializer.TryDeserialize(data, typeof(Dictionary<string, UserModel>), ref deserialized);

          var users = deserialized as Dictionary<string, UserModel>;
          onSuccessCallback?.Invoke(users);
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
