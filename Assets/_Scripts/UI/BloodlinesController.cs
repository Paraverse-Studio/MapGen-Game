using Paraverse.Mob.Stats;
using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Paraverse.Stats;

public class BloodlinesController : MonoBehaviour
{
    public enum BloodlineType
    {
        Vagabond = 0, Harrier = 1, Pioneer = 2, Scholar
    }

    [Header("Exteral References")]
    public MobStats playerStats;
    public PlayerController playerController;
    public PlayerCombat playerCombat;

    [Header("Internal References")]
    public BloodlineType chosenBloodline;
    public TextMeshProUGUI playAsText;
    public string playAsPhrase;
    public ModCard[] bloodlineCards;
    public Button continueButton;

    private void Awake()
    {
        continueButton.interactable = false;
    }

    // UI button callback
    public void ChooseBloodline(int type)
    {
        chosenBloodline = (BloodlineType)type;
        playAsText.text = playAsPhrase.Replace("[BLOODLINE]", chosenBloodline.ToString());
        continueButton.interactable = true;
    }

    // UI button callback
    public void DeselectAll()
    {
        foreach (ModCard card in bloodlineCards)
        {
            card.ToggleSelect(false);
        }
    }

    // UI button callback
    public void ApplyBloodline()
    {
        switch (chosenBloodline)
        {
            case BloodlineType.Vagabond:
                playerStats.AttackDamage.AddMod(new StatModifier(5));
                playerStats.MaxHealth.AddMod(new StatModifier(50));
                break;
            case BloodlineType.Harrier:
                playerStats.UpdateMovementSpeed(2);
                break;
            case BloodlineType.Pioneer:
                playerStats.CooldownReduction.AddMod(new StatModifier(30));
                playerStats.MaxEnergy.AddMod(new StatModifier(100));
                break;
            case BloodlineType.Scholar:
                playerStats.AttackDamage.OnStatBaseValueUpdatedEvent += ScholarEffectAttack;
                playerStats.AbilityPower.OnStatBaseValueUpdatedEvent += ScholarEffectAbility;
                break;
        }
    }

    public void ResetBloodline()
    {
        switch (chosenBloodline)
        {
            case BloodlineType.Vagabond:
                break;
            case BloodlineType.Harrier:
                break;
            case BloodlineType.Pioneer:
                break;
            case BloodlineType.Scholar:
                playerStats.AttackDamage.OnStatBaseValueUpdatedEvent -= ScholarEffectAttack;
                playerStats.AbilityPower.OnStatBaseValueUpdatedEvent -= ScholarEffectAbility;
                break;
        }
    }

    public void ScholarEffectAttack(float v)
    {
        Debug.Log("GIVING PLAYER ability: " + Mathf.CeilToInt((float)v / 2.0f));
        playerStats.AbilityPower.UpdateBaseValue(playerStats.AbilityPower.BaseValue + Mathf.CeilToInt((float)v / 2.0f), invokeEvent: false);
    }

    public void ScholarEffectAbility(float v)
    {
        Debug.Log("GIVING PLAYER attack: " + Mathf.CeilToInt((float)v / 4.0f));
        playerStats.AttackDamage.UpdateBaseValue(playerStats.AttackDamage.BaseValue + Mathf.CeilToInt((float)v / 4.0f), invokeEvent: false);
    }

}
