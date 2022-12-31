using Paraverse.Stats;
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
        protected float abilityPower = 10f;
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

        #endregion

        #region Start & Update Methods
        protected virtual void Awake()
        {
            Init();
            _curHealth = (int)MaxHealth.FinalValue;
            _curEnergy = (int)MaxEnergy.FinalValue;

            OnHealthChange?.Invoke(CurHealth, (int)MaxHealth.FinalValue);
            OnEnergyChange?.Invoke((int)CurEnergy, (int)MaxEnergy.FinalValue);
        }

        private void Init()
        {
            _maxHealth = new Stat(maxHealth);
            _maxEnergy = new Stat(maxEnergy);
            _attackDamage = new Stat(attackDamage);
            _attackSpeed = new Stat(attackSpeed);
            _moveSpeed = new Stat(moveSpeed);
            _energyRegen = new Stat(energyRegen);  
        }

        protected virtual void Update()
        {
            UpdateCurrentEnergy(EnergyRegen.FinalValue * Time.deltaTime);
        }
        #endregion

        #region Update Stat Methods
        public void UpdateMaxHealth(int amount)
        {
            _maxHealth.AddMod(new StatModifier(amount));
            UpdateCurrentHealth(amount);
        }

        public void UpdateCurrentHealth(int amount)
        {
            _curHealth += amount;
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

        public void ConsumeDiveEnergy()
        {
            UpdateCurrentEnergy(-diveEnergyCost);
        }

        public void ResetStats()
        {
            _curHealth = (int)MaxHealth.FinalValue;
            _curEnergy = (int)MaxEnergy.FinalValue;
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