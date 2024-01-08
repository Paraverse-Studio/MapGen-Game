using Paraverse.Mob.Combat;

namespace Paraverse
{
  public class AttackCollider : Damage, IDamage
  {
    public override void Init(MobCombat mob, ScalingStatData scalingStatData)
    {
      this.mob = mob;
      this.scalingStatData.flatPower = scalingStatData.flatPower;
      this.scalingStatData.attackScaling = scalingStatData.attackScaling;
      this.scalingStatData.abilityScaling = scalingStatData.abilityScaling;
      gameObject.SetActive(false);
    }

    public override void Init(MobCombat mob)
    {
      this.mob = mob;
      gameObject.SetActive(false);
    }
  }
}