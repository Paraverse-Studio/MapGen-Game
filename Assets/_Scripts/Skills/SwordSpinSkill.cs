using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using Paraverse.Stats;
using UnityEngine;

public class SwordSpinSkill : MobSkill, IMobSkill
{
    #region variables
    [Header("Sword Spin Skill:")]
    [SerializeField] private GameObject VFX;
    [SerializeField] private GameObject VFX2;
    [SerializeField] private GameObject[] VFXObjects;
    [SerializeField] private Vector3 colliderSize;
    [SerializeField] private float movementRatio;
    [SerializeField] private float attackRatio;
    [SerializeField] private float movementBoost;

    private CharacterController _controller;
    private PlayerController _player;
    private Transform _userWeapon = null;
    private GameObject _VFX = null;
    private GameObject _VFX2 = null;
    private StatModifier _buff = null;
    private bool _spinStarted = false;
    private float _boost = 0f;
    #endregion

    public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);
        OnSkillExecuted();
        Destroy(_VFX);
        Destroy(_VFX2);
    }

    public override void SkillUpdate()
    {
        base.SkillUpdate();

        if (false == mob.IsSkilling)
        {
            OnSkillExecuted();
            return;
        }

        _boost -= Time.deltaTime * 0.5f;
        if (_spinStarted) _player.ControlMovement((_player.GetWalkSpeed() * Mathf.Clamp(movementRatio, 0, 5f)) + Mathf.Max(0, _boost));
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

        _spinStarted = false;
    }

    protected override void OnSkillExecuted()
    {
        base.OnSkillExecuted();

        if (_VFX)
        {
            ToggleParticleSystem(turnParticlesOn: false);
            ToggleParticleSystem2(turnParticlesOn: false);
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
        mob.OnChargeSkillOneEvent += ResetCollider;
        mob.OnEnableChargeReleaseSkillOneEvent += AddSpin;
        mob.OnAttackInterrupted += OnSkillExecuted;
    }

    public override void UnsubscribeAnimationEventListeners()
    {
        base.UnsubscribeAnimationEventListeners();
        mob.OnChargeSkillOneEvent -= ResetCollider;
        mob.OnEnableChargeReleaseSkillOneEvent -= AddSpin;
        mob.OnAttackInterrupted -= OnSkillExecuted;
    }

    private void ResetCollider()
    {
        attackColliderGO.SetActive(false);
        StartCoroutine(UtilityFunctions.IDelayedAction(0.001f, () =>
        {
            if (skillOn) attackColliderGO.SetActive(true);
        }));
    }

    private void AddSpin()
    {
        _spinStarted = true;

        _boost = movementBoost;
        attackColliderGO.SetActive(true);

        Vector3 scale = attackColliderGO.transform.localScale;
        attackColliderGO.transform.localScale = new Vector3(scale.x + colliderSize.x, scale.y + colliderSize.y, scale.z + colliderSize.z);

        if (null == _buff)
        {
            _buff = new StatModifier(-(stats.AttackDamage.BaseValue * (1.0f - attackRatio)));
            stats.AttackDamage.AddMod(_buff);
        }

        if (null == _VFX2 && null != VFX2)
        {
            _VFX2 = Instantiate(VFX2, mob.transform);
            _VFX2.transform.localPosition += new Vector3(0, 0.55f, 0);
        }
        ToggleParticleSystem2(turnParticlesOn: true);
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
                if (null != obj)
                {
                    GameObject ob = Instantiate(obj, _userWeapon);
                    mob.OnAttackInterrupted += () => Destroy(ob);
                }
            }
        }
    }

    private void ToggleParticleSystem2(bool turnParticlesOn)
    {
        if (null != _VFX2)
        {
            var list = _VFX2.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in list)
            {
                if (turnParticlesOn) ps.Play();
                else ps.Stop();
            }
        }
    }

}
