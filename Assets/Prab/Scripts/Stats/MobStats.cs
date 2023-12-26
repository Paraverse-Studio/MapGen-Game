using Paraverse.Mob.Boosts;
using Paraverse.Stats;
using System.Collections;
using UnityEngine;

namespace Paraverse.Mob.Stats
{
    public class MobStats : MonoBehaviour, IMobStats
    {
        #region Variables
        [SerializeField]
        protected int maxHealth = 100;
        public Stat MaxHealth { get { return _maxHealth; } }
        private Stat _maxHealth;

        [SerializeField]
        public int _curHealth = 100;
        public int CurHealth { get { return _curHealth; } }

        [SerializeField]
        protected float attackDamage = 5f;
        public Stat AttackDamage { get { return _attackDamage; } }
        private Stat _attackDamage;

        [SerializeField]
        protected float abilityPower = 0f;
        public Stat AbilityPower { get { return _abilityPower; } }
        private Stat _abilityPower;

        [SerializeField, Range(0.05f, 3f), Tooltip("Attacks per second.")]
        protected float attackSpeed = 0.2f;
        public Stat AttackSpeed { get { return _attackSpeed; } }
        private Stat _attackSpeed;

        [SerializeField]
        protected float moveSpeed = 2f;
        public Stat MoveSpeed { get { return _moveSpeed; } }
        private Stat _moveSpeed;

        [SerializeField]
        protected float maxEnergy = 100f;
        public Stat MaxEnergy { get { return _maxEnergy; } }
        private Stat _maxEnergy;

        [SerializeField]
        protected float _curEnergy = 100f;
        public float CurEnergy { get { return _curEnergy; } }

        [SerializeField]
        protected float cooldownReduction = 0;
        public Stat CooldownReduction { get { return _cooldownReduction; } }
        private Stat _cooldownReduction;

        [SerializeField]
        protected float healthRegen = 0f;
        public Stat HealthRegen { get { return _healthRegen; } }
        private Stat _healthRegen;

        [SerializeField]
        protected float energyRegen = 25f;
        public Stat EnergyRegen { get { return _energyRegen; } }
        private Stat _energyRegen;

        [SerializeField]
        protected float _gold = 100f;
        public int Gold { get { return (int)_gold; } }


        [HideInInspector]
        public IntIntEvent OnHealthChange = new IntIntEvent();

        [HideInInspector]
        public IntIntEvent OnEnergyChange = new IntIntEvent();

        [SerializeField, Tooltip("Energy cost for mob dive.")]
        private float diveEnergyCost = 30f;

        public bool unkillable = false;

        [SerializeField, Header("Mob Boosts")]
        protected IMobBoosts _mobBoosts;
        public IMobBoosts MobBoosts { get { return _mobBoosts; } }
        #endregion

        #region Start & Update Methods
        protected void Awake()
        {
            ResetStats();
            _mobBoosts = GetComponent<IMobBoosts>();
        }

        private IEnumerator Start()
        {
            yield return null;
            OnHealthChange?.Invoke(CurHealth, (int)MaxHealth.FinalValue);
            OnEnergyChange?.Invoke((int)CurEnergy, (int)MaxEnergy.FinalValue);            
        }

        protected virtual void Update()
        {
            if (HealthRegen.FinalValue > 0 && Time.frameCount % 240 == 0)
            {
                UpdateCurrentHealth((int)HealthRegen.FinalValue);
            }
            UpdateCurrentEnergy((MaxEnergy.FinalValue * (EnergyRegen.FinalValue/100.0f)) * Time.deltaTime);
        }
        #endregion

        #region Update Stat Methods
        public void UpdateMaxHealth(int amount)
        {
            if (0 != amount) _maxHealth.AddMod(new StatModifier(amount));
            UpdateCurrentHealth(amount);
        }

        // This is made so that damage-related health updates are all to one function,
        // instead of using UpdateCurrentHealth which can happen through non-combat ways (healing, etc.)
        public void TakeDamage(float damage)
        {
            UpdateCurrentHealth(-Mathf.CeilToInt(damage));
        }

        public void UpdateCurrentHealth(int amount)
        {
            if (unkillable && amount < 0) return;
            _curHealth = Mathf.Clamp(_curHealth + amount, 0, (int)MaxHealth.FinalValue);
            OnHealthChange?.Invoke(CurHealth, (int)MaxHealth.FinalValue);
        }

        public void SetFullHealth()
        {
            UpdateCurrentHealth((int)(MaxHealth.FinalValue) - CurHealth);
        }

        public void UpdateAttackDamage(float amount)
        {
            _attackDamage.AddMod(new StatModifier(amount));
        }

        public void UpdateAbilityPower(float amount)
        {
            _abilityPower.AddMod(new StatModifier(amount));
        }

        public void UpdateAttackSpeed(float amount)
        {
            _attackSpeed.AddMod(new StatModifier(amount));
        }

        public void UpdateMovementSpeed(float amount)
        {
            _moveSpeed.AddMod(new StatModifier(amount));
        }

        public void UpdateMaxEnergy(float amount)
        {
            _maxEnergy.AddMod(new StatModifier(amount));
        }

        public void UpdateHealthRegen(float amount)
        {
            _healthRegen.AddMod(new StatModifier(amount));
        }

        public void ConsumeDiveEnergy()
        {
            UpdateCurrentEnergy(-diveEnergyCost);
        }

        public void ResetStats()
        {
            _maxHealth = new Stat(maxHealth);
            _maxHealth.FactoryResetMods();
            _maxEnergy = new Stat(maxEnergy);
            _maxEnergy.FactoryResetMods();
            _attackDamage = new Stat(attackDamage);
            _attackDamage.FactoryResetMods();
            _abilityPower = new Stat(abilityPower);
            _abilityPower.FactoryResetMods();
            _attackSpeed = new Stat(attackSpeed);
            _attackSpeed.FactoryResetMods();
            _moveSpeed = new Stat(moveSpeed);
            _moveSpeed.FactoryResetMods();
            _healthRegen = new Stat(healthRegen);
            _healthRegen.FactoryResetMods();
            _energyRegen = new Stat(energyRegen);
            _energyRegen.FactoryResetMods();
            _cooldownReduction = new Stat(cooldownReduction);
            _cooldownReduction.FactoryResetMods();

            _curHealth = (int)MaxHealth.FinalValue;
            _curEnergy = (int)MaxEnergy.FinalValue;
            _gold = 0f;
        }

        public void UpdateCurrentEnergy(float amount)
        {
            _curEnergy = Mathf.Clamp(_curEnergy + amount, 0f, MaxEnergy.FinalValue);
            OnEnergyChange?.Invoke((int)_curEnergy, (int)MaxEnergy.FinalValue);
        }

        public void UpdateGold(int amount)
        {
            _gold += amount;
        }
        #endregion
    }
}