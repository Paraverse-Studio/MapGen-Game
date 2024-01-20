using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using ParaverseWebsite.Models;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class SummaryView : MonoBehaviour
{
  private MobCombat playerCombat;

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

  private void Start()
  {
    playerCombat = PlayerController.Instance.GetComponent<MobCombat>();
  }


  public void Populate(GameLoopManager.PlayerSessionData sessionData, MobStats stats, PlayerCombat playerCombat)
  {
    // Init variables 
    string Username = MainMenuController.Instance.Username;
    int RoundNumberReached = sessionData.roundReached;
    int SessionLength = (int)sessionData.sessionLength;
    int GamesPlayed = 1;
    int DamageTaken = sessionData.damageTaken;
    int TotalScore = sessionData.totalScore;
    int GoldEarned = sessionData.goldEarned;
    int MobsDefeatedCount = sessionData.mobsDefeated;
    int BossesDefeatedCount = sessionData.bossesDefeated;
    int MysticDungeonsEnteredCount = sessionData.mysticDungeons;
    string MaxHealth = stats.MaxHealth.FinalValue.ToString();
    int Attack = (int)stats.AttackDamage.FinalValue;
    int Ability = (int)stats.AbilityPower.FinalValue;
    string BloodLine = bloodlinesController.chosenBloodline.ToString();
    string SkillUsed;
    BloodlineType BloodLineEnum = bloodlinesController.chosenBloodline;
    SkillName SkillUsedEnum;
    List<EffectName> EffectsObtained = new List<EffectName>();
    StringBuilder effectsSB = new StringBuilder();

    if (playerCombat.ActiveSkill == null)
    {
      SkillUsed = "No Skill Obtained";
      SkillUsedEnum = SkillName.None;
    }
    else
    {
      SkillUsed = ParaverseHelper.GetSkillName(playerCombat.ActiveSkill._skillNameDB);
      SkillUsedEnum = playerCombat.ActiveSkill._skillNameDB;
    }

    foreach (MobEffect effect in playerCombat.Effects)
    {
      string name = ParaverseHelper.GetEffectName(effect.EffectNameDB);
      EffectsObtained.Add(effect.EffectNameDB);
      effectsSB.Append(name + " | ");
    }

    // Populate summary view 
    roundsReachedText.text = RoundNumberReached.ToString();
    sessionLengthText.text = UtilityFunctions.GetFormattedTime(SessionLength);
    damageTakenText.text = DamageTaken.ToString();
    averageScoreText.text = TotalScore.ToString();
    goldEarnedText.text = GoldEarned.ToString();
    mobsDefeatedText.text = MobsDefeatedCount.ToString();
    bossesDefeatedText.text = BossesDefeatedCount.ToString();
    mysticDungeonsText.text = MysticDungeonsEnteredCount.ToString();
    healthText.text = MaxHealth;
    attackText.text = Attack.ToString();
    abilityText.text = Ability.ToString();
    bloodlineText.text = BloodLine;
    skillUsedText.text = SkillUsed;
    mobsObtainedText.text = effectsSB.ToString();

    // Gets the logged in user
    username = MainMenuController.Instance.Username;
    Debug.Log(username);

    SessionDataModel sessionDataModel = new SessionDataModel(
      Username,
      RoundNumberReached,
      GamesPlayed,
      SessionLength,
      DamageTaken,
      TotalScore,
      GoldEarned,
      MobsDefeatedCount,
      BossesDefeatedCount,
      MysticDungeonsEnteredCount,
      MaxHealth,
      Attack,
      Ability,
      BloodLine,
      SkillUsed,
      EffectsObtained,
      BloodLineEnum,
      SkillUsedEnum
      );

    FirebaseDatabaseManager.Instance.GetUser(username,
      // SUCCESSFULLY RETRIEVED USER
      (response) =>
      {
        Debug.Log($"User Exists!    username: {response.Username}, password: {response.Password}, email: {response.Email}, start date: {response.StartDate}, caption: {response.Caption}");

        UpdateDatabase(response.Username, sessionDataModel);
      },
      // FAILED TO RETRIEVE USER
      () =>
      {
        Debug.Log("User does not exist!");
      }
     );
  }

  private void UpdateDatabase(string username, SessionDataModel sessionDataModel)
  {
    // Create match history model
    MatchHistoryModel matchHistoryModel = new MatchHistoryModel(
      username,
      sessionDataModel.RoundNumberReached,
      sessionDataModel.SessionLength,
      sessionDataModel.DamageTaken,
      sessionDataModel.TotalScore,
      sessionDataModel.GoldEarned,
      sessionDataModel.MobsDefeatedCount,
      sessionDataModel.BossesDefeatedCount,
      sessionDataModel.MysticDungeonsEnteredCount,
      sessionDataModel.Health,
      sessionDataModel.Attack,
      sessionDataModel.Ability,
      sessionDataModel.BloodLine,
      sessionDataModel.SkillUsed,
      sessionDataModel.EffectsObtained
      );

    // Post match history to database 
    FirebaseDatabaseManager.Instance.PostMatchHistory(matchHistoryModel, (matchHistoryModel) => Debug.Log("Match History Created Successfully!"));

    LeaderboardsDatabaseHandler(sessionDataModel);
  }

  /// <summary>
  /// Checks if leaderboards exists already or not, and handle accordingly
  /// </summary>
  /// <param name="sessionData"></param>
  private void LeaderboardsDatabaseHandler(SessionDataModel sessionData)
  {
    // get user id and use it to get leaderboards of that user
    LeaderboardsModel oldLeaderboardsModel = new LeaderboardsModel();
    LeaderboardsModel updatedLeaderboardsModel = new LeaderboardsModel();

    FirebaseDatabaseManager.Instance.GetLeaderboard(sessionData.Username,
      //  IF USER IS FOUND!!
      (oldLeaderboardsModel) => UpdateLeaderboards(oldLeaderboardsModel, sessionData),
      //  IF USER IS NOT FOUND!!
      () => PostLeaderboards(sessionData)
    );
  }

  /// <summary>
  /// Runs if leaderboards already exists for user
  /// </summary>
  /// <param name="oldLeaderboardsModel"></param>
  /// <param name="sessionDataModel"></param>
  private void UpdateLeaderboards(LeaderboardsModel oldLeaderboardsModel, SessionDataModel sessionDataModel)
  {
    // Create updated leaderboards
    LeaderboardsModel updatedLeaderboardsModel = new LeaderboardsModel(oldLeaderboardsModel, sessionDataModel);

    // Updates previous leaderboards entry into database
    FirebaseDatabaseManager.Instance.PostLeaderboards(updatedLeaderboardsModel, (updatedLeaderboardsModel) => Debug.Log("Leaderboards Updated Successfully!"));
  }

  /// <summary>
  /// Creates a new instance of leaderboards for user
  /// </summary>
  /// <param name="sessionData"></param>
  private void PostLeaderboards(SessionDataModel sessionData)
  {
    LeaderboardsModel leaderboardsModel = new LeaderboardsModel
    (
      MainMenuController.Instance.Username,
      sessionData.RoundNumberReached,
      sessionData.GamesPlayed,
      sessionData.SessionLength,
      sessionData.TotalScore,
      sessionData.MobsDefeatedCount,
      sessionData.BossesDefeatedCount,
      sessionData.MysticDungeonsEnteredCount,
      new BloodlineOccurancesModel(sessionData.BloodLineEnum),
      new SkillsUsedOccurancesModel(sessionData.SkillUsedEnum),
      new EffectsObtainedOccurancesModel(sessionData.EffectsObtained)
    );

    // Create a new leaderboards entry for the user
    FirebaseDatabaseManager.Instance.PostLeaderboards(leaderboardsModel, (updatedLeaderboardsModel) => Debug.Log("Created new leaderboards entry!"));
  }
}