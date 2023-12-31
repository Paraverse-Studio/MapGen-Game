using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using UnityEngine;

public class JumpSmashAttack : MobSkill, IMobSkill
{
  #region Variables
  protected MobController controller;
  [SerializeField]
  protected string landAnimName = "LandAnim";
  [SerializeField]
  protected int layerIdx = 1;
  [SerializeField]
  protected float layerWeight = 1;
  #endregion


  #region Inherited Methods
  public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
  {
    base.ActivateSkill(mob, anim, stats, target);
    if (controller == null) controller = mob.GetComponent<MobController>();
    SubscribeAnimationEventListeners();
  }

  public override void SubscribeAnimationEventListeners()
  {
    base.SubscribeAnimationEventListeners();
    controller.OnLandEvent += OnLand;
    mob.OnEnableSkillColliderSOneEvent += EnableSmashAttackCollider;
  }

  public override void UnsubscribeAnimationEventListeners()
  {
    base.UnsubscribeAnimationEventListeners();
    controller.OnLandEvent -= OnLand;
    mob.OnDisableSkillColliderSOneEvent += DisableSmashAttackCollider;
  }

  protected override void ExecuteSkillLogic()
  {
    base.ExecuteSkillLogic();
    controller.ApplyJump(target.transform.position);
  }

  protected override void OnSkillComplete()
  {
    base.OnSkillComplete();
  }
  #endregion

  #region Private Methods
  private void OnLand()
  {
    anim.Play(landAnimName);
    anim.SetLayerWeight(layerIdx, layerWeight);
    anim.SetBool(StringData.IsGrounded, true);
  }

  private void EnableSmashAttackCollider()
  {
    attackColliderGO.SetActive(true);
  }

  private void DisableSmashAttackCollider()
  {
    attackColliderGO.SetActive(false);
  }
  #endregion
}
