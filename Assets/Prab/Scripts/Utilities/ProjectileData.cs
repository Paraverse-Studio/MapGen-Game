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
    [Tooltip("The projectile's speed.")]
    public float basicAtkProjSpeed = 10f;
}
