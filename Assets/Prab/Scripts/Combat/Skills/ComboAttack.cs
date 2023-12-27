using Paraverse.Helper;
using Paraverse.Mob.Combat;
using UnityEngine;
using static GreenPlains;
using UnityEngine.Windows;

public class ComboAttack : MonoBehaviour
{
  [SerializeField, Tooltip("At what index does this attack exist within the combo")]
  private int comboIdx = 0;
  [SerializeField]
  private float rotSpeed = 10f;

  [SerializeField]
  private string idleName = "Idle";
  [SerializeField]
  private string animName;

  public ComboState comboState;


  public void Init(Animator anim, MobCombat target)
  {
    StartCombo(anim, target);
  }

  private void StartCombo(Animator anim, MobCombat target)
  {
    comboState = ComboState.Start;

    if (target != null)
    {
      RotateToTarget(target.transform);

      // Ensures mob is looking at the target before attacking
      Vector3 dir = (target.transform.position - transform.position).normalized;
      dir = ParaverseHelper.GetPositionXZ(dir);
      Quaternion lookRot = Quaternion.LookRotation(dir);
      float angle = Quaternion.Angle(transform.rotation, lookRot);

      // condition to move to next state
      if (angle <= 0)
      {
        anim.Play(animName);
        comboState = ComboState.During;
      }
    }
  }

  private void DuringCombo(Animator anim, MobCombat target)
  {
    // when animation finishes
  }

  private void EndCombo(Animator anim, MobCombat target)
  {
    comboState = ComboState.End;
  }

  private void RotateToTarget(Transform target)
  {
    Vector3 lookDir = (target.position - target.position).normalized;
    Quaternion lookRot = Quaternion.LookRotation(lookDir);
    target.rotation = Quaternion.Slerp(target.rotation, lookRot, rotSpeed * Time.deltaTime);
  }
}

public enum ComboState
{
  Start,
  During,
  End
}
