using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FlowEffect : MobEffect
{
  public PlayerController controller;
  public float maxFlow = 100f;
  public float curFlow = 0f;
  public bool IsMaxFlow => curFlow >= maxFlow;
  public bool MaxFlowApplied = false;

  [SerializeField]
  public float[] increaseFlowRate;
  [SerializeField, Tooltip("The duration of shield")]
  protected float[] shieldDuration;
  protected float timer = 0f;

  protected float shieldAmount;


  public override void EffectTick()
  {
    Debug.Log($"shieldAmount: {shieldAmount} on health: {_stats.CurHealth} / {_stats.MaxHealth.FinalValue}");
    if (IsMaxFlow && false == MaxFlowApplied)
    {
      ApplyShield();
      MaxFlowApplied = true;
      Debug.Log($"Max flow reached!");
    }

    if (IsMaxFlow)
      StartShieldTimer();

    if (controller.IsMoving)
    {
      curFlow = Mathf.Clamp(curFlow += Time.deltaTime * increaseFlowRate[effectLevel], 0, maxFlow);
    }
  }

  private void ApplyShield()
  {
    shieldAmount = _stats.MaxHealth.FinalValue * scalingStatData[effectLevel].healthScaling;
    _stats.UpdateCurrentHealth((int)shieldAmount);
    _stats.UpdateMaxHealth((int)shieldAmount); 
    Debug.Log($"ApplyShield() with shieldAmount: {shieldAmount} on health: {_stats.CurHealth} / {_stats.MaxHealth.FinalValue}");
  }

  public override void ActivateEffect(MobStats stats, int id, int level)
  {
    base.ActivateEffect(stats, id, level);
    controller = _combat.GetComponent<PlayerController>();
  }

  public override void AddSubscribersToSkillEvents(Damage col)
  {
    base.AddSubscribersToSkillEvents(col);
  }


  public override void RemoveSubscribersToSkillEvents(Damage col)
  {
    base.RemoveSubscribersToSkillEvents(col);
  }

  private void StartShieldTimer()
  {
    if (timer >= shieldDuration[effectLevel])
      ResetFlow();
    else
      timer += Time.deltaTime;
  }

  private void ResetFlow()
  {
    _stats.UpdateCurrentHealth(-(int)shieldAmount);
    _stats.UpdateMaxHealth(-(int)shieldAmount);
    timer = 0;
    curFlow = 0f;
    shieldAmount = 0;
  }
}
