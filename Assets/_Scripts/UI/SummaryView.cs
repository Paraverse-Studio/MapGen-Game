using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using ParaverseWebsite.Models;
using System.Collections;
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
    Debug.Log("players skill!! " + playerCombat.ActiveSkill.SkillName);
    string SkillUsed = ParaverseHelper.GetSkillName(playerCombat.ActiveSkill.SkillName);
    List<string> EffectsObtained = new List<string>();

    StringBuilder effectsSB = new StringBuilder();

    // For Leaderboards
    BloodlineType BloodLineEnum = bloodlinesController.chosenBloodline;
    SkillName SkillUsedEnum = playerCombat.ActiveSkill.SkillName;
    List<EffectName> EffectsObtainedEnums = new List<EffectName>();

    foreach (MobEffect effect in playerCombat.Effects)
    {
      string name = ParaverseHelper.GetEffectName(effect.EffectNameDB);
      EffectsObtainedEnums.Add(effect.EffectNameDB);
      EffectsObtained.Add(name);
      Debug.Log("Effect: " + name);
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

    SessionDataModel sessionDataModel = new SessionDataModel(
      Username,
      RoundNumberReached,
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
      SkillUsedEnum,
      EffectsObtainedEnums
      );

    // Only update database if user exists!
    FirebaseDatabaseManager.Instance.GetUser(username,
      // SUCCESSFULLY RETRIEVED USER
      (response) =>
      {
        Debug.Log($"User Exists!    username: {response.I_Username}, password: {response.I_Password}, email: {response.I_Email}, start date: {response.I_StartDate}, caption: {response.P_Caption}");

        UpdateDatabase(sessionDataModel);
      },
      // FAILED TO RETRIEVE USER
      () =>
      {
        Debug.Log("User does not exist!");
      }
     );
  }

  private void UpdateDatabase(SessionDataModel sessionData)
  {
    // Create match history model
    MatchHistoryModel matchHistoryModel = new MatchHistoryModel(
      sessionData.Username,
      sessionData.RoundNumberReached,
      sessionData.SessionLength,
      sessionData.DamageTaken,
      sessionData.TotalScore,
      sessionData.GoldEarned,
      sessionData.MobsDefeatedCount,
      sessionData.BossesDefeatedCount,
      sessionData.MysticDungeonsEnteredCount,
      sessionData.Health,
      sessionData.Attack,
      sessionData.Ability,
      sessionData.BloodLine,
      sessionData.SkillUsed,
      sessionData.EffectsObtainedEnums
      );

    // Post match history to database 
    FirebaseDatabaseManager.Instance.PostMatchHistory(matchHistoryModel, (matchHistoryModel) => Debug.Log("Match History Created Successfully!"));

    LeaderboardsDatabaseHandler(sessionData);
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
      sessionData.SessionLength,
      sessionData.TotalScore,
      sessionData.TotalScore,
      sessionData.MobsDefeatedCount,
      sessionData.BossesDefeatedCount,
      sessionData.MysticDungeonsEnteredCount,
      new BloodlineOccurancesModel(sessionData.BloodLineEnum),
      new SkillsUsedOccurancesModel(sessionData.SkillUsedEnum),
      new EffectsObtainedOccurancesModel(sessionData.EffectsObtainedEnums)
    );

    // Create a new leaderboards entry for the user
    FirebaseDatabaseManager.Instance.PostLeaderboards(leaderboardsModel, (updatedLeaderboardsModel) => Debug.Log("Created new leaderboards entry!"));
  }
}