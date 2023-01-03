using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Knockback Effect", menuName ="SOs/Skills/Knockback Effect")]
public class KnockBackEffect : ScriptableObject
{
    [Header("Knockback Values")]
    [Tooltip("The knockback force applied to the target.")]
    public float knockForce = 5f;
    [Range(0, 2), Tooltip("The max duration of knockback applied to the target.")]
    public float maxKnockbackDuration = 1f;
    [Tooltip("The max distance applied to the target.")]
    public float maxKnockbackRange = 1.5f;
    [Tooltip("Start position")]
    public Vector3 startPos;

    public KnockBackEffect(KnockBackEffect effect)
    {
        knockForce = effect.knockForce;
        maxKnockbackDuration = effect.maxKnockbackDuration;
        maxKnockbackRange = effect.maxKnockbackRange;
        this.startPos = effect.startPos;
    }
    public KnockBackEffect(float force, float maxDur, float maxRange, Vector3 startPos)
    {
        knockForce = force;
        maxKnockbackDuration = maxDur;
        maxKnockbackRange = maxRange;
        this.startPos = startPos;
    }
}
