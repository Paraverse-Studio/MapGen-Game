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
        roundLabel.text = "Round " + GameLoopManager.Instance.roundNumber.ToString();
        roundDescriptionText.text = MapGeneration.Instance.M.mapDescription;

        goldText.text = stats.Gold.ToString();
        statText.text = $"{Mathf.CeilToInt(stats.MaxHealth.FinalValue)}\n";
        statText.text += $"{Mathf.CeilToInt(stats.AttackDamage.FinalValue)} <color=#C8C8C8>({Mathf.CeilToInt(stats.AttackDamage.BaseValue)})</color>\n";
        statText.text += $"{stats.AttackSpeed.FinalValue}/sec\n\n";
        statText.text += $"{Mathf.CeilToInt(stats.AbilityPower.FinalValue)} <color=#C8C8C8>({Mathf.CeilToInt(stats.AbilityPower.BaseValue)})</color>\n";
        statText.text += $"{stats.MoveSpeed.FinalValue} m/s\n\n";
        statText.text += $"+{(Mathf.RoundToInt((stats.MobBoosts.GetAttackDamageBoost() - 1f) * 100f))}%\n";
        statText.text += $"+{(Mathf.RoundToInt((stats.MobBoosts.GetAbilityPowerBoost() - 1f) * 100f))}%\n";
        statText.text += "0%\n"; // $"+{(Mathf.RoundToInt((stats.MobBoosts.GetHealthBoost() - 1f) * 100f))}%\n"; // TODO
        statText.text += $"+{(Mathf.RoundToInt((stats.MobBoosts.GetOverallDamageOutputBoost() - 1f) * 100f))}%\n";
    }

}
