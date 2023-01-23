using Paraverse;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesCollider : Projectile
{
    public void OnParticleCollision(GameObject other)
    {
        Collider c = other.GetComponent<Collider>();
        if (c) OnTriggerEnter(c);       
    }

}
