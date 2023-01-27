using Paraverse.Mob.Stats;
using Paraverse.Player;
using Paraverse.Stats;
using UnityEngine;

public class ThirdAttackEmpowerEffect : MonoBehaviour
{
    private MobStats stats;
    private PlayerCombat combat;

    [SerializeField]
    private float attackStatValue = 10f;
    private int basicAttackCounter = 0;
    private StatModifier mod;

    private void Awake()
    {
        stats = GameObject.FindGameObjectWithTag(StringData.PlayerTag).GetComponent<MobStats>();
        combat = stats.GetComponent<PlayerCombat>();
    }

    private void OnEnable()
    {
        mod = new StatModifier(attackStatValue);
        basicAttackCounter = 0;
        combat.basicAttackCollider.OnBasicAttackPreHitEvent += IncrementBasicAttackCounter;
        combat.basicAttackCollider.OnBasicAttackApplyDamageEvent += RemoveMod;
    }

    private void IncrementBasicAttackCounter()
    {
        basicAttackCounter++;
        if (basicAttackCounter % 3 == 0)
        {
            stats.AttackDamage.AddMod(mod);
        }
    }

    private void RemoveMod(float empty = 0)
    {
        stats.AttackDamage.RemoveMod(mod);
    }


    private void OnDisable()
    {
        RemoveMod();
        combat.basicAttackCollider.OnBasicAttackPreHitEvent -= IncrementBasicAttackCounter;
        combat.basicAttackCollider.OnBasicAttackApplyDamageEvent -= RemoveMod;
    }
}
