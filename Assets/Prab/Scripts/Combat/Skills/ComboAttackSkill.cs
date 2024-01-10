using Paraverse;
using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

public class ComboAttackSkill : MobSkill, IMobSkill
{
  #region Variables
  [SerializeField]
  protected GameObject offHandAttackColliderGO;
  protected AttackCollider offHandAttackCollider;

  [SerializeField]
  private ComboAttack[] comboDetails;

  private int comboIdx = 0;
  private int maxComboIdx;
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

    foreach (ComboAttack details in comboDetails)
    {
      details.Init(anim);
    }
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
    ComboAttack combo = comboDetails[comboIdx];
    if (combo.State.Equals(ComboState.Complete))
      ProceedToNextCombo();

    combo.ComboUpdate();
    if (combo.AnimFinished && comboDetails[comboIdx].State.Equals(ComboState.Attack))
      RotateToTarget();
  }

  protected override void ExecuteSkillLogic()
  {
    base.ExecuteSkillLogic();

    ComboInit();
    comboDetails[comboIdx].StartAttack();
  }
  #endregion

  #region Private Methods
  private void ComboInit()
  {
    comboIdx = 0;
    maxComboIdx = comboDetails.Length - 1;
    comboDetails[comboIdx].Init(anim);

    foreach (ComboAttack details in comboDetails)
    {
      details.Init(anim);
    }
  }

  private void ProceedToNextCombo()
  {
    comboIdx++;
    ComboAttack combo = comboDetails[comboIdx];

    if (comboIdx > maxComboIdx)
      CompleteCombo();

    // Updates state instantly to attack 
    // Can specify conditions like rotation in between if required
    if (combo.State.Equals(ComboState.Idle))
      combo.StartAttack();
  }

  private void CompleteCombo()
  {
    comboDetails[comboIdx].OnAttackComplete();
    comboIdx = 0; 
    OnSkillComplete();
  }
  #endregion

  #region Animation Events
  public void EnableMainHandAttackCollider()
  {
    if (attackColliderGO != null)
      attackColliderGO.SetActive(true);

    //skillOn = true;
    SetSkillState(SkillState.InUse);
  }

  public void DisableMainHandAttackCollider()
  {
    if (attackColliderGO != null)
      attackColliderGO.SetActive(false);

    //skillOn = false;
  }

  public void EnableOffHandAttackCollider()
  {
    if (offHandAttackCollider != null)
      offHandAttackColliderGO.SetActive(true);

    //skillOn = true;
    SetSkillState(SkillState.InUse);
  }

  public void DisableOffHandAttackCollider()
  {
    if (offHandAttackCollider != null)
      offHandAttackColliderGO.SetActive(false);

    //skillOn = false;
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