using Paraverse.IK;
using Paraverse.Mob;
using Paraverse.Mob.Controller;

public class BossIKMobController : MobController, IMobController
{
  private HeadIK headIK;

  protected override void Start()
  {
    base.Start();
    headIK = GetComponent<HeadIK>();
  }

  protected override void StateHandler()
  {
    base.StateHandler();

    if (headIK == null) return;
    if (CurMobState.Equals(MobState.Pursue) || CurMobState.Equals(MobState.Combat))
    {
      headIK.SetLookAtObj(pursueTarget);
    }
    else
    {
      headIK.SetLookAtObj(null);
    }
  }
}
