using Paraverse.Mob.Stats;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuViewController : MonoBehaviour
{
    public TextMeshProUGUI roundLabel;
    public TextMeshProUGUI roundDescriptionText;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI crystalsText;

    public TextMeshProUGUI statLabel;
    public TextMeshProUGUI statText;

    private MobStats stats;

    private void Start()
    {
        stats = GlobalSettings.Instance.player.GetComponentInChildren<MobStats>();
    }


    public void UpdateDisplay()
    {
        roundLabel.text = "Round " + GameLoopManager.Instance.nextRoundNumber.ToString();
        roundDescriptionText.text = MapGeneration.Instance.M.mapDescription;

        goldText.text = stats.Gold.ToString();
        statText.text = $"{Mathf.CeilToInt(stats.MaxHealth.FinalValue)}\n";
        statText.text += $"{Mathf.CeilToInt(stats.AttackDamage.FinalValue)} <color=#C8C8C8>({Mathf.CeilToInt(stats.AttackDamage.BaseValue)})</color>\n";
        statText.text += $"{stats.AttackSpeed.FinalValue}/sec\n\n";
        statText.text += $"{Mathf.CeilToInt(stats.AbilityPower.FinalValue)} <color=#C8C8C8>({Mathf.CeilToInt(stats.AbilityPower.BaseValue)})</color>\n";
        statText.text += $"{stats.MoveSpeed.FinalValue} m/s\n\n";
        statText.text += $"+{(int)((stats.MobBoosts.AttackDamageBoost.FinalValue - 1f) * 100f)}%\n";
        statText.text += $"+{(int)((stats.MobBoosts.AbilityPowerBoost.FinalValue - 1f) * 100f)}%\n";
        statText.text += $"+{(int)((stats.MobBoosts.OverallDamageBoost.FinalValue - 1f) * 100f)}%\n";
    }

}
