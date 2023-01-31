using Paraverse.Mob.Stats;
using Paraverse.Player;
using Paraverse.Stats;
using UnityEngine;

public class EmpoweredAttack : MonoBehaviour
{
    private MobStats _stats;
    private PlayerCombat _combat;

    [SerializeField, Range(1, 10), Header("Empowered attack every X hit:")]
    private int _empoweredHitIndex;
    [SerializeField, Header("Empowered attack scaling:")]
    private ScalingStatData _empoweredScaling;

    private int _hitCounter = 0;
    private StatModifier _mod;


    private void Awake()
    {
        _stats = GameObject.FindGameObjectWithTag(StringData.PlayerTag).GetComponent<MobStats>();
        _combat = _stats.GetComponent<PlayerCombat>();
    }

    private void OnEnable()
    {
        _mod = new StatModifier(_empoweredScaling.FinalValue(_stats));
        _hitCounter = 0;
        _combat.basicAttackCollider.OnBasicAttackPreHitEvent += IncrementBasicAttackCounter;
        _combat.basicAttackCollider.OnBasicAttackApplyDamageEvent += RemoveMod;
    }

    private void IncrementBasicAttackCounter()
    {
        _hitCounter++;
        if (_hitCounter % _empoweredHitIndex == 0)
        {
            _mod.Value = _empoweredScaling.FinalValue(_stats);
            _stats.AttackDamage.AddMod(_mod);
        }
    }

    private void RemoveMod(float empty = 0)
    {
        _stats.AttackDamage.RemoveMod(_mod);
    }


    private void OnDisable()
    {
        RemoveMod();
        _combat.basicAttackCollider.OnBasicAttackPreHitEvent -= IncrementBasicAttackCounter;
        _combat.basicAttackCollider.OnBasicAttackApplyDamageEvent -= RemoveMod;
    }
}
