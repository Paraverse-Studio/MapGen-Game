using System;
using UnityEngine;

[Serializable]
public class ComboAttack
  : MonoBehaviour
{
  private Animator anim;
  public ComboState State = ComboState.Idle;
  public string animName;
  [Tooltip("This is the state used in between each combo attack.")]
  private string comboIdleStateName = "Combo Idle";
  [SerializeField, Range(0,2f)]
  private float delayAfterAttackTime = 0.5f;
  private float curDelayTimer;
  public bool AnimFinished => _animFinished;
  private bool _animFinished = false;

  public void Init(Animator anim)
  {
    this.anim = anim;
    SetState(ComboState.Idle);
    _animFinished = false;
    ResetTimer();
  }

  public void ComboUpdate()
  {
    AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
    // Only Set state to Attack when this particular animation is playing
    // This is to avoid setting animFinished to true instantly after the prev animation is completed
    if (info.shortNameHash == Animator.StringToHash(animName) && !State.Equals(ComboState.Attack))
      SetState(ComboState.Attack);

    // exit when combo is completed 
    if (State.Equals(ComboState.Complete) || State.Equals(ComboState.Idle)) return;

    // Ensure that this is only applied for current animation
    if (info.shortNameHash == Animator.StringToHash(comboIdleStateName) && !_animFinished)
      _animFinished = true;

    // Only begin the delay timer when animation has finished 
    if (AnimFinished)
    {
      if (curDelayTimer <= 0)
        OnAttackComplete();
      else
        curDelayTimer -= Time.deltaTime;
    }
  }

  /// <summary>
  /// Plays the specific combo attack animation
  /// </summary>
  public void StartAttack()
  {
    anim.Play(animName);
  }


  /// <summary>
  /// Resets the combo attacks states and variables 
  /// </summary>
  public void OnAttackComplete()
  {
    SetState(ComboState.Complete);
    ResetTimer();
    _animFinished = false;
  }

  private void SetState(ComboState state)
  {
    State = state;
  }

  private void ResetTimer()
  {
    curDelayTimer = delayAfterAttackTime;
  }
}

public enum ComboState
{
  Idle,
  Attack,
  Complete
}
