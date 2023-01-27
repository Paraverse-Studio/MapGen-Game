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
    [SerializeField] private float attackRangeLengthen = 0f;

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

        Vector3 scale = attackColliderGO.transform.localScale;
        attackColliderGO.transform.localScale = new Vector3(scale.x, scale.y + attackRangeLengthen, scale.z);
        Debug.Log("HUH??? " + attackColliderGO.transform.localScale);
    }

    private void BuffDurationHandler()
    {
        if (_buffDurationElapsed <= 0 && _buffDurationElapsed > -10f)
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
        _buffDurationElapsed = -11f;

        if (_weaponVFX) 
        {
            ToggleParticleSystem(turnParticlesOn: false);
        }

        if (null != _buff)
        {
            stats.AttackDamage.RemoveMod(_buff);
            _buff = null;

            Vector3 scale = attackColliderGO.transform.localScale;
            attackColliderGO.transform.localScale = new Vector3(scale.x, scale.y - attackRangeLengthen, scale.z);
        }                    
    }

    private float GetPowerAmount()
    {
        return scalingStatData.flatPower + (stats.AttackDamage.FinalValue * scalingStatData.attackScaling) + (stats.AbilityPower.FinalValue * scalingStatData.abilityScaling);
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
