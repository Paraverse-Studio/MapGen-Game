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

        public IntIntEvent OnHealthChange = new IntIntEvent();
        #endregion

        #region Start Method
        protected virtual void Start()
        {
            curHealth = maxHealth;
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
        #endregion
    }
}