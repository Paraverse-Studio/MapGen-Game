using Paraverse;
using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

public class DualComboSkill : MobSkill, IMobSkill
{
  #region Variables
  [SerializeField]
  protected GameObject offHandAttackColliderGO;
  protected AttackCollider offHandAttackCollider;
  #endregion


  #region Inherited Methods
  public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
  {
    base.ActivateSkill(mob, anim, stats, target);

    // Checks if melee users have basic attack collider script on weapon
    if (offHandAttackColliderGO == null)
    {
      Debug.LogWarning(gameObject.name + " doesn't have an attack collider.");
      return;
    }
    offHandAttackColliderGO.SetActive(true);
    offHandAttackCollider = offHandAttackColliderGO.GetComponent<AttackCollider>();
    offHandAttackCollider.Init(mob, scalingStatData);
    offHandAttackColliderGO.SetActive(false);

    SubscribeAnimationEventListeners();
  }

  public override void SubscribeAnimationEventListeners()
  {
    base.SubscribeAnimationEventListeners();
    mob.OnEnableMainHandColliderSOneEvent += EnableMainHandAttackCollider;
    mob.OnDisableMainHandColliderSOneEvent += DisableMainHandAttackCollider;
    mob.OnEnableOffHandColliderSOneEvent += EnableOffHandAttackCollider;
    mob.OnDisableOffHandColliderSOneEvent += DisableOffHandAttackCollider;
    mob.OnDisableSkillOneEvent += DisableSkillAndCollider;
  }

  public override void UnsubscribeAnimationEventListeners()
  {
    base.UnsubscribeAnimationEventListeners();
    mob.OnEnableMainHandColliderSOneEvent -= EnableMainHandAttackCollider;
    mob.OnDisableMainHandColliderSOneEvent -= DisableMainHandAttackCollider;
    mob.OnEnableOffHandColliderSOneEvent -= EnableOffHandAttackCollider;
    mob.OnDisableOffHandColliderSOneEvent -= DisableOffHandAttackCollider;
    mob.OnDisableSkillOneEvent -= DisableSkillAndCollider;
  }

  /// <summary>
  /// Contains all methods required to run in Update within MobCombat script.
  /// </summary>
  public override void SkillUpdate()
  {
    base.SkillUpdate();
  }

  protected override void ExecuteSkillLogic()
  {
    base.ExecuteSkillLogic();
  }
  #endregion

  #region Animation Events
  public void EnableMainHandAttackCollider()
  {
    if (attackColliderGO != null)
      attackColliderGO.SetActive(true);

    skillOn = true;
  }

  public void DisableMainHandAttackCollider()
  {
    if (attackColliderGO != null)
      attackColliderGO.SetActive(false);

    skillOn = false;
  }

  public void EnableOffHandAttackCollider()
  {
    if (offHandAttackCollider != null)
      offHandAttackColliderGO.SetActive(true);

    skillOn = true;
  }

  public void DisableOffHandAttackCollider()
  {
    if (offHandAttackCollider != null)
      offHandAttackColliderGO.SetActive(false);

    skillOn = false;
  }

  public void DisableSkillAndCollider()
  {
    if (attackColliderGO != null)
      attackColliderGO.SetActive(false);
    if (offHandAttackCollider != null)
      offHandAttackColliderGO.SetActive(false);

    OnSkillComplete();
  }
  #endregion
}
