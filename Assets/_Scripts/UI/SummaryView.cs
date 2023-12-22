using Paraverse.Mob.Stats;
using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummaryView : MonoBehaviour
{
    [Header("References")]
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

        bloodlineText.text = "TBD";
        skillUsedText.text = combat.ActiveSkill != null ? combat.ActiveSkill.Name : "N/A";
        attackText.text = stats.AttackDamage.BaseValue.ToString();
        abilityText.text = stats.AbilityPower.BaseValue.ToString();
        healthText.text = stats.CurHealth + "/" + stats.MaxHealth.BaseValue;
        mobsObtainedText.text = ModsManager.Instance.PurchasedMods.Count.ToString();
    }
}