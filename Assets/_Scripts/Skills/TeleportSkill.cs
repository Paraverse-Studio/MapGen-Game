using Paraverse.Combat;
using Paraverse.Helper;
using UnityEngine;

public class TeleportSkill : MobSkill, IMobSkill
{
    #region variables
    [Header("Teleport Properties")]
    [SerializeField]
    private int rangeLimit;
    [SerializeField]
    private GameObject FX;

    private bool performing = false;
    #endregion


    // Check if target exists
    protected override bool CanUseSkill()
    {
        if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && IsBasicAttack == false)
        {
            if (null == mob.Target)
            {
                mob.Target = SelectableSystem.Instance.ToggleSelect();
            }

            if (null != mob.Target && UtilityFunctions.IsDistanceLessThan(mob.transform.position, mob.Target.position, rangeLimit))
            {
                return true;
            }

            return false;
        }

        return false;
    }

    public override void SkillUpdate()
    {
        base.SkillUpdate();

        if (null != mob.Target && performing)
        {
            Vector3 targetDir = ParaverseHelper.GetPositionXZ(mob.Target.position - mob.transform.position).normalized;
            mob.transform.forward = targetDir;
        }
    }

    protected override void ExecuteSkillLogic()
    {
        performing = true;

        base.ExecuteSkillLogic();

        // teleport to closest enemy
        if (null == mob.Target)
        {            
            return;            
        }

        anim.SetBool(StringData.IsInteracting, true);
        Vector3 oldPosition = mob.gameObject.transform.position;
        Vector3 position = ((mob.Target.position - mob.transform.position).normalized * 0.5f) + mob.Target.position;
        var block = MapGeneration.Instance.GetClosestValidGroundBlock(position);

        ResetCollider();
        UtilityFunctions.TeleportObject(mob.gameObject, block.transform.position + new Vector3(0, 0.4f, 0));

        if (FX)
        {
            Instantiate(FX, oldPosition, Quaternion.identity);
            Instantiate(FX, block.transform.position, Quaternion.identity);
        }

        DisableSkill();

        StartCoroutine(UtilityFunctions.IDelayedAction(0.2f, () =>
        {
            performing = false;
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsUsingSkill, false);
        }));
    }

    private void ResetCollider()
    {
        attackColliderGO.SetActive(false);
        StartCoroutine(UtilityFunctions.IDelayedAction(0.001f, () =>
        {
            attackColliderGO.SetActive(true);
        }));

        StartCoroutine(UtilityFunctions.IDelayedAction(0.25f, () =>
        {
            attackColliderGO.SetActive(false);
        }));
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();
    }

}
