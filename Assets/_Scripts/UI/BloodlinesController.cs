using Paraverse.Mob.Stats;
using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Paraverse.Stats;

public enum BloodlineType
{
    Vagabond = 0, Harrier = 1, Pioneer = 2, Scholar = 3
}

public class BloodlinesController : MonoBehaviour
{
    [Header("Exteral References")]
    public MobStats playerStats;
    public PlayerController playerController;
    public PlayerCombat playerCombat;
    public Animator playerAnimator;

    [Header("Internal References")]
    public BloodlineType chosenBloodline;
    public TextMeshProUGUI playAsText;
    public string playAsPhrase;
    public ModCard[] bloodlineCards;
    public Button continueButton;

    // cache values
    private float _playerDiveForce = -1;
    private float _playerDiveDuration = -1;

    private void Awake()
    {
        continueButton.interactable = false;
        _playerDiveForce = playerController.diveForce;
        _playerDiveDuration = playerController.maxDiveDuration;
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
                ResetDiveValues();
                playerStats.AttackDamage.AddMod(new StatModifier(5));
                playerStats.MaxHealth.AddMod(new StatModifier(50));
                playerStats.SetCurrentHealth((int)playerStats.MaxHealth.FinalValue);
                GameLoopManager.Instance.GameLoopEvents.OnStartRound.AddListener(VagabondRoundHeal);
                break;
            case BloodlineType.Harrier:
                playerStats.UpdateMovementSpeed(2);
                playerController.maxDiveDuration *= 1.5f;
                playerController.diveForce += 5;
                playerAnimator.speed = 1.2f;
                break;
            case BloodlineType.Pioneer:
                ResetDiveValues();
                playerStats.CooldownReduction.AddMod(new StatModifier(30));
                playerStats.MaxEnergy.AddMod(new StatModifier(100));
                break;
            case BloodlineType.Scholar:
                ResetDiveValues();
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
                GameLoopManager.Instance.GameLoopEvents.OnStartRound.RemoveListener(VagabondRoundHeal);
                break;
            case BloodlineType.Harrier:
                if (_playerDiveForce != -1) playerController.diveForce = _playerDiveForce;
                if (_playerDiveDuration != -1) playerController.maxDiveDuration = _playerDiveDuration;
                playerAnimator.speed = 1f;
                break;
            case BloodlineType.Pioneer:
                break;
            case BloodlineType.Scholar:
                playerStats.AttackDamage.OnStatBaseValueUpdatedEvent -= ScholarEffectAttack;
                playerStats.AbilityPower.OnStatBaseValueUpdatedEvent -= ScholarEffectAbility;
                break;
        }
        ResetDiveValues();
    }

    private void ResetDiveValues()
    {
        _playerDiveForce = playerController.diveForce;
        _playerDiveDuration = playerController.maxDiveDuration;
    }

    public void VagabondRoundHeal()
    {
        playerStats.UpdateCurrentHealth((int)(playerStats.MaxHealth.FinalValue * 0.05f));
    }

    public void ScholarEffectAttack(float v)
    {
        playerStats.AbilityPower.UpdateBaseValue(playerStats.AbilityPower.BaseValue + Mathf.CeilToInt((float)v / 2.0f), invokeEvent: false);
    }

    public void ScholarEffectAbility(float v)
    {
        playerStats.AttackDamage.UpdateBaseValue(playerStats.AttackDamage.BaseValue + Mathf.CeilToInt((float)v / 4.0f), invokeEvent: false);
    }

}
