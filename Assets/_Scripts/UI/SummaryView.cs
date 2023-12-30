using Paraverse.Mob.Stats;
using Paraverse.Player;
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

  public async Task Populate(GameLoopManager.PlayerSessionData sessionData, MobStats stats, PlayerCombat combat)
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

    // if user exists, update database
    if (await FirebaseDatabaseManager.Instance.UserExists(username))
    {
      await UpdateDatabase(sessionData, stats, combat);
    }

  }

  private async Task UpdateDatabase(GameLoopManager.PlayerSessionData sessionData, MobStats stats, PlayerCombat combat)
  {
    // Pass match history to db
    MatchHistoryModel matchHistoryModel = new MatchHistoryModel
    {
      Username = MainMenuController.Instance.Username,
      RoundNumberReached = sessionData.roundReached,
      SessionLength = UtilityFunctions.GetFormattedTime(sessionData.sessionLength),
      DamageTaken = sessionData.damageTaken,
      TotalScore = sessionData.totalScore,
      GoldEarned = sessionData.goldEarned,
      MobsDefeatedCount = sessionData.mobsDefeated,
      BossesDefeatedCount = sessionData.bossesDefeated,
      MysticDungeonsEnteredCount = sessionData.mysticDungeons,
      BloodLine = bloodlinesController.chosenBloodline.ToString(),
      SkillUsed = combat.ActiveSkill != null ? combat.ActiveSkill.Name : "N/A",
      Attack = (int)stats.AttackDamage.BaseValue,
      Ability = (int)stats.AbilityPower.BaseValue,
      Health = (stats.CurHealth + "/" + stats.MaxHealth.BaseValue).ToString(),
      EffectsObtained = ModsManager.Instance.PurchasedMods.Count.ToString(),
    };

    // compare values of users current match history values with the leaderboards value 
    int highestRoundNumberReached = sessionData.roundReached;
    var highestSessionLength = UtilityFunctions.GetFormattedTime(sessionData.sessionLength);
    int highestDamageTaken = sessionData.damageTaken;
    int highestTotalScore = sessionData.totalScore;
    int highestGoldEarned = sessionData.goldEarned;
    int highestMobsDefeatedCount = sessionData.mobsDefeated;
    int highestBossesDefeatedCount = sessionData.bossesDefeated;
    int highestMysticDungeonsEnteredCount = sessionData.mysticDungeons;
    int highestAttack = (int)stats.AttackDamage.BaseValue;
    int highestAbility = (int)stats.AbilityPower.BaseValue;
    string highestHealth = (stats.CurHealth + "/" + stats.MaxHealth.BaseValue).ToString();

    // get user id and use it to get leaderboards of that user
    LeaderboardsModel model;
    if (await FirebaseDatabaseManager.Instance.LeaderboardsExists(matchHistoryModel.Username))
    {
      model = await FirebaseDatabaseManager.Instance.GetLeaderboards(matchHistoryModel.Username);

      highestRoundNumberReached = Mathf.Max(model.HighestDamageTaken, highestRoundNumberReached);
      highestAttack = Mathf.Max(model.HighestDamageTaken, highestAttack);
      highestAbility = Mathf.Max(model.HighestDamageTaken, highestAbility);
      highestMobsDefeatedCount = Mathf.Max(model.HighestDamageTaken, highestMobsDefeatedCount);
    }

    // update the leaderboards with new high scores
    LeaderboardsModel leaderboardsModel = new LeaderboardsModel
    {
      Username = MainMenuController.Instance.Username,
      HighestRoundNumberReached = highestRoundNumberReached,
      HighestSessionLength = highestSessionLength,
      HighestDamageTaken = highestDamageTaken,
      HighestTotalScore = highestTotalScore,
      HighestGoldEarned = highestGoldEarned,
      HighestMobsDefeatedCount = highestMobsDefeatedCount,
      HighestBossesDefeatedCount = highestBossesDefeatedCount,
      HighestMysticDungeonsEnteredCount = highestMysticDungeonsEnteredCount,
      BloodLine = bloodlinesController.chosenBloodline.ToString(),
      SkillUsed = combat.ActiveSkill != null ? combat.ActiveSkill.Name : "N/A",
      HighestAttack = highestAttack,
      HighestAbility = highestAbility,
      HighestHealth = highestHealth,
      EffectsObtained = ModsManager.Instance.PurchasedMods.Count.ToString(),
    };

    // Creates entries into database
    await FirebaseDatabaseManager.Instance.CreateMatchHistory(matchHistoryModel);
    await FirebaseDatabaseManager.Instance.CreateLeaderboards(leaderboardsModel);
  }
}
