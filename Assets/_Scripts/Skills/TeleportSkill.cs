using Paraverse.Combat;
using Paraverse.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSkill : MobSkill, IMobSkill
{
    #region variables
    [Header("Teleport Properties")]
    [SerializeField]
    private int rangeLimit;
    [SerializeField]
    private GameObject FX;
    #endregion

    public override void SkillUpdate()
    {
        base.SkillUpdate();

        Vector3 targetDir = ParaverseHelper.GetPositionXZ(mob.Target.position - mob.transform.position).normalized;
        mob.transform.forward = targetDir;
    }

    protected override void ExecuteSkillLogic()
    {
        base.ExecuteSkillLogic();

        // teleport
        if (null == mob.Target) return;

        if (UtilityFunctions.IsDistanceLessThan(mob.transform.position, mob.Target.position, rangeLimit))
        {
            Vector3 oldPosition = mob.gameObject.transform.position;
            Vector3 position = ((mob.Target.position - mob.transform.position).normalized * 1.2f) + mob.Target.position;
            var block = MapGeneration.Instance.GetClosestValidGroundBlock(position);

            ResetCollider();
            UtilityFunctions.TeleportObject(mob.gameObject, block.transform.position + new Vector3(0, 0.4f, 0));
            
            if (FX)
            {
                Instantiate(FX, oldPosition, Quaternion.identity);
                Instantiate(FX, block.transform.position, Quaternion.identity);
            }

            anim.SetBool(StringData.IsInteracting, false);
            DisableSkill();
        }
    }

    private void ResetCollider()
    {
        attackColliderGO.SetActive(false);
        StartCoroutine(UtilityFunctions.IDelayedAction(0.001f, () =>
        {
            attackColliderGO.SetActive(true);
        }));
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();
    }

}
