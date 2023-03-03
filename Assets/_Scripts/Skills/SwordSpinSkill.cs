using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Player;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSpinSkill : MobSkill, IMobSkill
{
    #region variables
    [Header("Sword Spin Skill:")]
    [SerializeField] private GameObject VFX;
    [SerializeField] private GameObject[] VFXObjects;
    [SerializeField] private Vector3 colliderSize;

    private Transform _userWeapon = null;
    private GameObject _VFX = null;
    private StatModifier _buff = null;

    private CharacterController _controller;
    private PlayerController _player;

    #endregion

    public override void SkillUpdate()
    {
        base.SkillUpdate();

        if (false == mob.IsSkilling)
        {
            DisableSkill();
            return;
        }
    }

    protected override void ExecuteSkillLogic()
    {
        base.ExecuteSkillLogic();

        // COMPONENTS, AND BASIC DATA
        if (null == _controller) _controller = mob.GetComponent<CharacterController>();
        if (null == _player) _player = mob.GetComponent<PlayerController>();

        _userWeapon = attackColliderGO.transform.parent;

        // GLOWY PARTICLES
        if (null == _VFX && null != VFX) _VFX = Instantiate(VFX, _userWeapon);
        ToggleParticleSystem(turnParticlesOn: true);

        AddSpin();
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

            Vector3 scale = attackColliderGO.transform.localScale;
            attackColliderGO.transform.localScale = Vector3.one;

            attackColliderGO.SetActive(false);
        }
    }

    public override void SubscribeAnimationEventListeners()
    {
        base.SubscribeAnimationEventListeners();
        mob.OnChargeSkillOneEvent += AddSpin;
    }

    public override void UnsubscribeAnimationEventListeners()
    {
        base.UnsubscribeAnimationEventListeners();
        mob.OnChargeSkillOneEvent -= AddSpin;
    }

    private void AddSpin()
    {
        attackColliderGO.SetActive(true);

        Vector3 scale = attackColliderGO.transform.localScale;
        attackColliderGO.transform.localScale = new Vector3(scale.x + colliderSize.x, scale.y + colliderSize.y, scale.z + colliderSize.z);

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
        if (null != _VFX)
        {
            var list = _VFX.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in list)
            {
                if (turnParticlesOn) ps.Play();
                else ps.Stop();
            }
        }
        if (null != VFXObjects && true == turnParticlesOn)
        {
            foreach (GameObject obj in VFXObjects)
            {
                if (null != obj) Instantiate(obj, _userWeapon);
            }
        }
    }

}
