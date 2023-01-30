using System;
using UnityEngine;

[Serializable]
public class ProjectileData 
{
    [Tooltip("Basic attack projectile prefab.")]
    public GameObject projPf;
    [Tooltip("The projectile object held by the mob.")]
    public GameObject projHeld;
    [Tooltip("The projectile's origin position.")]
    public Transform projOrigin;
    [Tooltip("The projectile's rotation. [if null, will get the look rotation from mob to target.]")]
    public Transform projRotation;
    [Tooltip("The projectile's speed.")]
    public float basicAtkProjSpeed = 10f;

    // Updated via Mob Skill
    public ScalingStatData scalingStatData;
}
