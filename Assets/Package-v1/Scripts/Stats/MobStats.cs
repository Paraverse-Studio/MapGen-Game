using System.ComponentModel;
using UnityEngine;

namespace Paraverse.Mob.Stats
{
    public class MobStats : MonoBehaviour, IMobStats
    {
        #region Variables
        [SerializeField]
        protected int maxHealth = 100;
        [DisplayName("Max Health")]
        public int MaxHealth { get { return maxHealth; } }

        public int curHealth = 100;
        [DisplayName("Current Health")]
        public int CurHealth { get { return curHealth; } }

        [SerializeField]
        protected float attackDamage = 5f;
        [DisplayName("Attack")]
        public float AttackDamage { get { return attackDamage; } }

        [SerializeField, Range(0.2f, 3f), Tooltip("Attacks per second.")]
        protected float attackSpeed = 0.2f;
        [DisplayName("Attack Speed")]
        public float AttackSpeed { get { return attackSpeed; } }

        [SerializeField]
        protected float moveSpeed = 2f;
        [DisplayName("Movement Speed")]
        public float MoveSpeed { get { return moveSpeed; } }

        [SerializeField]
        protected float maxEnergy = 100f;
        [DisplayName("Max Energy")]
        public float MaxEnergy { get { return maxEnergy; } }

        [SerializeField]
        protected float curEnergy = 100f;
        [DisplayName("Current Energy")]
        public float CurrentEnergy { get { return curEnergy; } }

        [SerializeField]
        protected float energyRegen = 25f;
        [DisplayName("Energy Regen /s")]
        public float EnergyRegen { get { return energyRegen; } }


        [SerializeField]
        protected float gold = 100f;
        [DisplayName("Current Gold")]
        public float Gold { get { return gold; } }

        int IMobStats.Gold => throw new System.NotImplementedException();

        [HideInInspector]
        public IntIntEvent OnHealthChange = new IntIntEvent();
        
        [HideInInspector]
        public IntIntEvent OnEnergyChange = new IntIntEvent();

        [SerializeField, Tooltip("Energy cost for mob dive.")]
        private float diveEnergyCost = 30f;

        #endregion

        #region Start Method
        protected virtual void Start()
        {
            curHealth = maxHealth;
            curEnergy = maxEnergy;
            OnHealthChange?.Invoke((int)curHealth, (int)maxHealth);
            OnEnergyChange?.Invoke((int)curEnergy, (int)maxEnergy);
        }
        #endregion

        #region Update Stat Methods
        public void UpdateMaxHealth(int amount)
        {
            maxHealth += amount;
        }

        public void UpdateCurrentHealth(int amount)
        {
            curHealth += amount;
            OnHealthChange?.Invoke(curHealth, maxHealth);
        }

        public void UpdateAttackDamage(float amount)
        {
            attackDamage += amount;
        }

        public void UpdateAttackSpeed(float amount)
        {
            attackSpeed += amount;
        }

        public void UpdateMovementSpeed(float amount)
        {
            moveSpeed += amount;
        }

        public void UpdateMaxEnergy(float amount)
        {
            maxEnergy += amount;
        }

        public void ConsumeDiveEnergy()
        {
            UpdateCurrentEnergy(-diveEnergyCost);
        }

        public void UpdateCurrentEnergy(float amount)
        {
            curEnergy = Mathf.Clamp(curEnergy + amount, 0f, MaxEnergy);
            OnEnergyChange?.Invoke((int)curEnergy, (int)maxEnergy);
        }

        public void UpdateGold(int amount)
        {
            gold += amount;
        }
        #endregion
    }
}