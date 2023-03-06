using Paraverse.Mob.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class AttackTrail : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private XWeaponTrail _trail;
    private GameObject _holder = null;

    // Start is called before the first frame update
    public void Start()
    {
        if (null == _holder) _holder = new GameObject("(Temp) X-Weapon Trail");
        _holder.layer = 17;
        _trail = _holder.AddComponent<XWeaponTrail>();
        _trail.MaxFrame = 5;
        _trail.Granularity = 50;
        _trail.Fps = 50;
        _trail.MyColor = Color.white;
        _trail.MyMaterial = GlobalSettings.Instance.attackTrailMaterial;
        _trail.PointStart = startPoint;
        _trail.PointEnd = endPoint;
        _trail.UseWithSRP = true;

        _trail.Init();
        _trail.Deactivate();
        StartCoroutine(UtilityFunctions.IDelayedAction(1f, () => _trail.MaxFrame = 60));

        gameObject.GetComponent<MobCombat>().OnEnableBasicAttackCollider += ActivateTrail;
        gameObject.GetComponent<MobCombat>().OnDisableBasicAttackCollider += DeactivateTrail;
    }

    private void OnDestroy()
    {
        gameObject.GetComponent<MobCombat>().OnEnableBasicAttackCollider -= ActivateTrail;
        gameObject.GetComponent<MobCombat>().OnDisableBasicAttackCollider -= DeactivateTrail;

        Destroy(_holder);
    }

    public void ActivateTrail(float duration)
    {
        _trail.Activate();
        _trail.StopSmoothly(duration);
    }

    public void ActivateTrail()
    {
        ActivateTrail(2f);
    }

    public void DeactivateTrail()
    {
        _trail.StopSmoothly(0.15f);
    }
}
