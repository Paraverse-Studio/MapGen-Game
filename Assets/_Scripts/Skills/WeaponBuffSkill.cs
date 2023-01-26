using Paraverse.Combat;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBuffSkill : MobSkill, IMobSkill
{
    #region variables
    [Header("Weapon Buff Skill:")]
    [SerializeField] private GameObject weaponBuffVFX;
    [SerializeField] private float buffDuration;
    [SerializeField] private Transform userWeapon;

    private float _buffDurationElapsed = 0f;
    private GameObject _weaponVFX = null;
    private StatModifier _buff = null;
    #endregion

    public override void SkillUpdate()
    {
        base.SkillUpdate();
        BuffDurationHandler();

        if (null != _buff) _buff.Value = GetPowerAmount();
    }

    protected override void ExecuteSkillLogic()
    {
        base.ExecuteSkillLogic();
        _buffDurationElapsed = buffDuration;

        // Add the buff VFX and stats to the player
        if (null == _weaponVFX) _weaponVFX = Instantiate(weaponBuffVFX, userWeapon);
        ToggleParticleSystem(turnParticlesOn: true);

        if (null == _buff)
        {
            _buff = new StatModifier(GetPowerAmount());
            stats.AttackDamage.AddMod(_buff);
        }
    }

    private void BuffDurationHandler()
    {
        if (_buffDurationElapsed <= 0)
        {
            DisableSkill();
        }
        else
        {            
            _buffDurationElapsed = Mathf.Clamp(_buffDurationElapsed - Time.deltaTime, 0f, buffDuration);
        }
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();

        if (_weaponVFX) 
        {
            ToggleParticleSystem(turnParticlesOn: false);
        }

        if (null != _buff)
        {
            stats.AttackDamage.RemoveMod(_buff);
            _buff = null;
        }
    }

    private float GetPowerAmount()
    {
        return flatPower + (stats.AttackDamage.FinalValue * attackScaling) + (stats.AbilityPower.FinalValue * abilityScaling);
    }

    private void ToggleParticleSystem(bool turnParticlesOn)
    {
        if (null == _weaponVFX) return;

        var list = _weaponVFX.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in list)
        {
            if (turnParticlesOn) ps.Play();
            else ps.Stop();
        }
    }


}
