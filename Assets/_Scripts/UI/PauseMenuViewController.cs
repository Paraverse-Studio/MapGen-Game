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

    private MobStats playerStats;

    private void Start()
    {
        playerStats = GlobalSettings.Instance.player.GetComponentInChildren<MobStats>();
    }


    public void UpdateDisplay()
    {
        roundLabel.text = "Round " + GameLoopManager.Instance.nextRoundNumber.ToString();
        roundDescriptionText.text = MapGeneration.Instance.M.mapDescription;

        goldText.text = playerStats.Gold.ToString();

        statText.text = playerStats.MaxHealth + "\n" +
                        playerStats.AttackDamage + "\n" +
                        playerStats.AttackSpeed + "/sec\n\n" +
                        "0\n" + // placeholder for ability
                        playerStats.MoveSpeed + " m/s\n";
    }

}
