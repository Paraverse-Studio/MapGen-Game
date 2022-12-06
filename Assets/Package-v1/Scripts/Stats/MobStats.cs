using System.ComponentModel;
using UnityEngine;

namespace Paraverse.Mob.Stats
{
    public class MobStats : MonoBehaviour, IMobStats
    {
        #region Variables

        [SerializeField]
        protected float maxHealth = 100f;
        [DisplayName("Max Health")]
        public float MaxHealth { get { return maxHealth; } }

        public float curHealth = 100f;
        [DisplayName("Current Health")]
        public float CurHealth { get { return curHealth; } }

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
        protected float currentEnergy = 100f;
        [DisplayName("Current Energy")]
        public float CurrentEnergy { get { return currentEnergy; } }

        [HideInInspector]
        public IntIntEvent OnHealthChange = new IntIntEvent();
        
        [HideInInspector]
        public IntIntEvent OnEnergyChange = new IntIntEvent();
        #endregion

        #region Start Method
        protected virtual void Start()
        {
            curHealth = maxHealth;
            currentEnergy = maxEnergy;
            OnHealthChange?.Invoke((int)curHealth, (int)maxHealth);
            OnEnergyChange?.Invoke((int)currentEnergy, (int)maxEnergy);
        }
        #endregion

        #region Update Stat Methods
        public void UpdateMaxHealth(float amount)
        {
            maxHealth += amount;
        }

        public void UpdateCurrentHealth(float amount)
        {
            curHealth += amount;
            OnHealthChange?.Invoke((int)curHealth, (int)maxHealth);
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

        public void UpdateCurrentEnergy(float amount)
        {
            currentEnergy += amount;
            OnEnergyChange?.Invoke((int)currentEnergy, (int)maxEnergy);
        }
        #endregion
    }
}