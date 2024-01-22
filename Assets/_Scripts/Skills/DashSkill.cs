using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using Paraverse.Stats;
using UnityEngine;

public class DashSkill : MobSkill, IMobSkill
{
  #region variables
  [Header("Dash Skill:")]
  [SerializeField] private GameObject VFX;
  [SerializeField] private GameObject[] VFXObjects;
  [SerializeField] private Vector3 thrustForce;
  [SerializeField] private Vector3 jumpForce;
  [SerializeField] private Vector3 colliderSize;

  private Transform _userWeapon = null;
  private GameObject _VFX = null;
  private StatModifier _buff = null;

  private float _originalJumpGravity = 0;
  private Vector3 _forces = Vector3.zero;
  private float _resistance = 40f;
  private bool _thrustStarted = false;
  //private CharacterController _controller;
  //private PlayerController _player;

  #endregion

  public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
  {
    base.ActivateSkill(mob, anim, stats, target);
  }

  public override void SkillUpdate()
  {
    base.SkillUpdate();

    if (_controller)
    {
      _controller.Move(new Vector3(_forces.x, _forces.y, _forces.z) * Time.deltaTime);

      float y = _forces.y; _forces.y = 0;
      _forces = Vector3.MoveTowards(_forces, Vector3.zero, _resistance * Time.deltaTime);
      _forces.y = Mathf.MoveTowards(y, 0, 17f * Time.deltaTime);
    }
  }

  protected override void ExecuteSkillLogic()
  {
    base.ExecuteSkillLogic();

    // COMPONENTS, AND BASIC DATA
    if (null == _controller) _controller = mob.GetComponent<CharacterController>();
    if (null == _player) _player = mob.GetComponent<PlayerController>();

    _userWeapon = attackColliderGO.transform.parent;
    if (0 == _originalJumpGravity) _originalJumpGravity = _player.JumpGravity;
    _player.JumpGravity = _originalJumpGravity / 3f;
    _thrustStarted = false;
    _forces = Vector3.zero;

    // GLOWY PARTICLES
    if (null == _VFX && null != VFX) _VFX = Instantiate(VFX, _userWeapon);
    ToggleParticleSystem(turnParticlesOn: true);

    // BUFF OF THE DAMAGE
    if (null == _buff)
    {
      _buff = new StatModifier(GetPowerAmount());
      stats.AttackDamage.AddMod(_buff);
    }

    _forces += jumpForce;
    Physics.IgnoreLayerCollision(14, 15, true);
  }

  protected override void OnSkillComplete()
  {
    base.OnSkillComplete();

    if (_VFX)
    {
      ToggleParticleSystem(turnParticlesOn: false);
    }

    if (null != _buff)
    {
      stats.AttackDamage.RemoveMod(_buff);
      _buff = null;

      Physics.IgnoreLayerCollision(14, 15, false);
      _player.JumpGravity = _originalJumpGravity;

      Vector3 scale = attackColliderGO.transform.localScale;
      attackColliderGO.transform.localScale = Vector3.one;
    }
  }

  public override void SubscribeAnimationEventListeners()
  {
    base.SubscribeAnimationEventListeners();
    mob.OnChargeSkillOneEvent += AddThrust;
    mob.OnDisableSkillOneEvent += OnSkillComplete;
  }

  public override void UnsubscribeAnimationEventListeners()
  {
    base.UnsubscribeAnimationEventListeners();
    mob.OnChargeSkillOneEvent -= AddThrust;
  }

  private void AddThrust()
  {
    if (_thrustStarted) return;

    _thrustStarted = true;
    Vector3 direction = transform.forward;
    if (mob.Target) direction = (ParaverseHelper.GetPositionXZ(mob.Target.transform.position - mob.transform.position)).normalized;
    _forces += new Vector3(direction.x * thrustForce.x, direction.y * thrustForce.y, direction.z * thrustForce.z);

    Vector3 scale = attackColliderGO.transform.localScale;
    attackColliderGO.transform.localScale = new Vector3(scale.x + colliderSize.x, scale.y + colliderSize.y, scale.z + colliderSize.z);
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
