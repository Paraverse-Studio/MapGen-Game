using Paraverse.Mob.Stats;
using Paraverse.Player;
using ParaverseWebsite.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SummaryView : MonoBehaviour
{
  [Header("External References")]
  public BloodlinesController bloodlinesController;

  [Header("Internal References")]
  public TextMeshProUGUI roundsReachedText;
  public TextMeshProUGUI sessionLengthText;
  public TextMeshProUGUI damageTakenText;
  public TextMeshProUGUI averageScoreText;
  public TextMeshProUGUI goldEarnedText;
  public TextMeshProUGUI mobsDefeatedText;
  public TextMeshProUGUI bossesDefeatedText;
  public TextMeshProUGUI mysticDungeonsText;

  public TextMeshProUGUI bloodlineText;
  public TextMeshProUGUI skillUsedText;
  public TextMeshProUGUI attackText;
  public TextMeshProUGUI abilityText;
  public TextMeshProUGUI healthText;
  public TextMeshProUGUI mobsObtainedText;

  public string username;


  public void Populate(GameLoopManager.PlayerSessionData sessionData, MobStats stats, PlayerCombat combat)
  {
    roundsReachedText.text = sessionData.roundReached.ToString();
    sessionLengthText.text = UtilityFunctions.GetFormattedTime(sessionData.sessionLength);
    damageTakenText.text = sessionData.damageTaken.ToString();
    averageScoreText.text = ((int)(sessionData.totalScore / (sessionData.roundReached - 1.0f))) + "%";
    goldEarnedText.text = sessionData.goldEarned.ToString();
    mobsDefeatedText.text = sessionData.mobsDefeated.ToString();
    bossesDefeatedText.text = sessionData.bossesDefeated.ToString();
    mysticDungeonsText.text = sessionData.mysticDungeons.ToString();
    bloodlineText.text = bloodlinesController.chosenBloodline.ToString();
    skillUsedText.text = combat.ActiveSkill != null ? combat.ActiveSkill.Name : "N/A";
    attackText.text = stats.AttackDamage.BaseValue.ToString();
    abilityText.text = stats.AbilityPower.BaseValue.ToString();
    healthText.text = stats.CurHealth + "/" + stats.MaxHealth.BaseValue;
    mobsObtainedText.text = ModsManager.Instance.PurchasedMods.Count.ToString();



    // Gets the logged in user
    username = MainMenuController.Instance.Username;

    // Only update database if user exists!
    FirebaseDatabaseManager.Instance.GetUser(username,
      // SUCCESSFULLY RETRIEVED USER
      (response) =>
      {
        Debug.Log($"User Exists!    username: {response.I_Username}, password: {response.I_Password}, email: {response.I_Email}, start date: {response.I_StartDate}, caption: {response.P_Caption}");

        UpdateDatabase(sessionData, stats, combat);
      },
      // FAILED TO RETRIEVE USER
      () => {
        Debug.Log("User does not exist!");
      }
     );
  }

  private void UpdateDatabase(GameLoopManager.PlayerSessionData sessionData, MobStats stats, PlayerCombat combat)
  {
    MatchHistoryModel model = MatchHistoryDatabaseHandler(sessionData, stats, combat);

    LeaderboardsDatabaseHandler(model);
  }

  private MatchHistoryModel MatchHistoryDatabaseHandler(GameLoopManager.PlayerSessionData sessionData, MobStats stats, PlayerCombat combat)
  {
    // Init variables 
    string Username = MainMenuController.Instance.Username;
    int RoundNumberReached = sessionData.roundReached;
    int SessionLength = (int)sessionData.sessionLength;
    int DamageTaken = sessionData.damageTaken;
    int TotalScore = sessionData.totalScore;
    int GoldEarned = sessionData.goldEarned;
    int MobsDefeatedCount = sessionData.mobsDefeated;
    int BossesDefeatedCount = sessionData.bossesDefeated;
    int MysticDungeonsEnteredCount = sessionData.mysticDungeons;
    string BloodLine = bloodlinesController.chosenBloodline.ToString();
    string SkillUsed = combat.ActiveSkill != null ? combat.ActiveSkill.Name : "N/A";
    int Attack = (int)stats.AttackDamage.BaseValue;
    int Ability = (int)stats.AbilityPower.BaseValue;
    string Health = (stats.CurHealth + "/" + stats.MaxHealth.BaseValue).ToString();
    string EffectsObtained = ModsManager.Instance.PurchasedMods.Count.ToString();

    // Create match history model
    MatchHistoryModel matchHistoryModel = new MatchHistoryModel(
      Username,
      RoundNumberReached,
      SessionLength,
      DamageTaken,
      TotalScore,
      GoldEarned,
      MobsDefeatedCount,
      BossesDefeatedCount,
      MysticDungeonsEnteredCount,
      BloodLine,
      SkillUsed,
      Attack,
      Ability,
      Health,
      EffectsObtained);

    // Post match history to database 
    FirebaseDatabaseManager.Instance.PostMatchHistory(matchHistoryModel, (matchHistoryModel) => Debug.Log("Match History Created Successfully!"));

    return matchHistoryModel;
  }

  private void LeaderboardsDatabaseHandler(MatchHistoryModel matchHistoryModel)
  {
    // get user id and use it to get leaderboards of that user
    LeaderboardsModel oldLeaderboardsModel = new LeaderboardsModel();
    LeaderboardsModel updatedLeaderboardsModel = new LeaderboardsModel();

    FirebaseDatabaseManager.Instance.GetLeaderboard(matchHistoryModel.Username,
      //  IF USER IS FOUND!!
      (oldLeaderboardsModel) => UpdateLeaderboards(oldLeaderboardsModel, matchHistoryModel),
      //  IF USER IS NOT FOUND!!
      () => PostLeaderboards(matchHistoryModel)
    );
  }

  private void UpdateLeaderboards(LeaderboardsModel oldLeaderboardsModel, MatchHistoryModel matchHistoryModel)
  {
    oldLeaderboardsModel.HighestRoundNumberReached = Mathf.Max(oldLeaderboardsModel.HighestRoundNumberReached, matchHistoryModel.RoundNumberReached);
    oldLeaderboardsModel.CumulativeSessionLength += matchHistoryModel.SessionLength;
    oldLeaderboardsModel.CumulativeTotalScore += matchHistoryModel.TotalScore;
    oldLeaderboardsModel.CumulativeMobsDefeatedCount += matchHistoryModel.MobsDefeatedCount;
    oldLeaderboardsModel.CumulativeBossesDefeatedCount += matchHistoryModel.BossesDefeatedCount;
    oldLeaderboardsModel.CumulativeMysticDungeonsEnteredCount += matchHistoryModel.MysticDungeonsEnteredCount;

    // Create updated leaderboards
    LeaderboardsModel updatedLeaderboardsModel = new LeaderboardsModel
    {
      Username = MainMenuController.Instance.Username,
      HighestRoundNumberReached = oldLeaderboardsModel.HighestRoundNumberReached,
      CumulativeSessionLength = oldLeaderboardsModel.CumulativeSessionLength,
      CumulativeTotalScore = oldLeaderboardsModel.CumulativeTotalScore,
      CumulativeParaverseScore = oldLeaderboardsModel.CumulativeTotalScore,
      CumulativeMobsDefeatedCount = oldLeaderboardsModel.CumulativeMobsDefeatedCount,
      CumulativeBossesDefeatedCount = oldLeaderboardsModel.CumulativeBossesDefeatedCount,
      CumulativeMysticDungeonsEnteredCount = oldLeaderboardsModel.CumulativeMysticDungeonsEnteredCount,
      BloodLine = oldLeaderboardsModel.BloodLine,
      SkillUsed = oldLeaderboardsModel.SkillUsed,
      EffectsObtained = oldLeaderboardsModel.EffectsObtained,
    };

    // Updates previous leaderboards entry into database
    FirebaseDatabaseManager.Instance.PostLeaderboards(updatedLeaderboardsModel, (updatedLeaderboardsModel) => Debug.Log("Leaderboards Updated Successfully!"));
  }

  private void PostLeaderboards(MatchHistoryModel matchHistoryModel)
  {
    LeaderboardsModel updatedLeaderboardsModel = new LeaderboardsModel
    {
      Username = MainMenuController.Instance.Username,
      HighestRoundNumberReached = matchHistoryModel.RoundNumberReached,
      CumulativeSessionLength = matchHistoryModel.SessionLength,
      CumulativeTotalScore = matchHistoryModel.TotalScore,
      CumulativeParaverseScore = matchHistoryModel.TotalScore,
      CumulativeMobsDefeatedCount = matchHistoryModel.MobsDefeatedCount,
      CumulativeBossesDefeatedCount = matchHistoryModel.BossesDefeatedCount,
      CumulativeMysticDungeonsEnteredCount = matchHistoryModel.MysticDungeonsEnteredCount,
      BloodLine = matchHistoryModel.BloodLine,
      SkillUsed = matchHistoryModel.SkillUsed,
      EffectsObtained = matchHistoryModel.EffectsObtained,
    };

    // Create a new leaderboards entry for the user
    FirebaseDatabaseManager.Instance.PostLeaderboards(updatedLeaderboardsModel, (updatedLeaderboardsModel) => Debug.Log("Created new leaderboards entry!"));
  }
}
