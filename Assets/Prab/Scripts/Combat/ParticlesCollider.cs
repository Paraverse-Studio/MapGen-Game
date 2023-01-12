using Paraverse;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesCollider : MonoBehaviour
{

    #region Variables
    private MobCombat mob;
    [SerializeField, Tooltip("Speed of the projectile.")]
    private string targetTag = "Player";
    private Vector3 target = Vector3.forward;
    [SerializeField, Tooltip("Speed of the projectile.")]
    private float speed;
    [SerializeField, Tooltip("Range of the projectile.")]
    private float range;
    [SerializeField, Tooltip("Range of the projectile.")]
    private float damage;
    [SerializeField, Tooltip("Projectile is destroyed after this duration.")]
    private float deathTimer = 5f;
    [SerializeField, Tooltip("Stationary projectile.")]
    protected bool stationary = false;
    [SerializeField, Tooltip("Damage over time.")]
    protected bool dot = false;
    [SerializeField, Tooltip("Apply damage upon enter.")]
    protected bool applyDamageOnEnter = false;
    [SerializeField, Tooltip("Applies damage every given second.")]
    protected float dotIntervalTimer = 1f;
    private float dotTimer = 0f;

    [Header("Knockback Effect")]
    [SerializeField]
    private KnockBackEffect knockBackEffect;

    [Header("Special Properties")]
    public bool pierce = false;

    [Header("VFX")]
    public GameObject launchFX;
    public GameObject hitFX;

    private float curdeathTimer = 0f;
    private Vector3 origin;
    #endregion


    public void OnParticleCollision(GameObject other)
    {
        Debug.Log("THIS SHIT HIT SOMETHING: " + other.name);
        
        IMobController controller = other.GetComponent<IMobController>();
        controller.Stats.UpdateCurrentHealth((int)-damage);

        // Apply knock back effect
        if (null != knockBackEffect)
        {
            KnockBackEffect effect = new KnockBackEffect(knockBackEffect);
            controller.ApplyKnockBack(mob.transform.position, effect);
        }

        if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);        
    }

}
