using Paraverse.Combat;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : MobSkill, IMobSkill
{
    #region variables
    [Header("Dash Skill:")]
    [SerializeField] private GameObject VFX;
    [SerializeField] private float buffDuration;

    private Transform _userWeapon = null;
    private GameObject _VFX = null;
    private StatModifier _buff = null;

    private float ayo;
    #endregion

    public override void SkillUpdate()
    {
        base.SkillUpdate();
        // move character here towards targetspot or target transform

        ayo -= Time.deltaTime;
        if (ayo <= 0) DisableSkill();
    }

    protected override void ExecuteSkillLogic()
    {
        base.ExecuteSkillLogic();

        _userWeapon = attackColliderGO.transform.parent;

        // Add the glowy VFX and stats to the player
        if (null == _VFX && null != VFX) _VFX = Instantiate(VFX, _userWeapon);
        ToggleParticleSystem(turnParticlesOn: true);

        AddBuff();

        ayo = 10f;
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();

        if (_VFX)
        {
            ToggleParticleSystem(turnParticlesOn: false);
        }

        if (null != _buff)
        {
            stats.AttackDamage.RemoveMod(_buff);
            _buff = null;
        }
    }

    public override void SubscribeAnimationEventListeners()
    {
        base.SubscribeAnimationEventListeners();
        mob.OnChargeSkillOneEvent += AddBuff;
    }

    public override void UnsubscribeAnimationEventListeners()
    {
        base.UnsubscribeAnimationEventListeners();
        mob.OnChargeSkillOneEvent -= AddBuff;
    }

    private void AddBuff()
    {
        if (null == _buff)
        {
            _buff = new StatModifier(GetPowerAmount());
            stats.AttackDamage.AddMod(_buff);
        }
    }

    private float GetPowerAmount()
    {
        return scalingStatData.flatPower + (stats.AttackDamage.FinalValue * scalingStatData.attackScaling) + (stats.AbilityPower.FinalValue * scalingStatData.abilityScaling);
    }

    private void ToggleParticleSystem(bool turnParticlesOn)
    {
        if (null == _VFX) return;

        var list = _VFX.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in list)
        {
            if (turnParticlesOn) ps.Play();
            else ps.Stop();
        }
    }

}
