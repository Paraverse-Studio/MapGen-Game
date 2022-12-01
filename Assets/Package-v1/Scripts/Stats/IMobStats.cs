namespace Paraverse.Mob.Stats
{
    public interface IMobStats
    {
        #region Properties
        // Properties 
        public float MaxHealth { get; }
        public float CurHealth { get; }
        public float AttackDamage { get; }
        public float AttackSpeed { get; }
        public float MoveSpeed { get; }
        #endregion

        // Methods
        #region Update Stat Methods
        /// <summary>
        /// Updates mob health stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateMaxHealth(float amount);

        /// <summary>
        /// Updates mob current health stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateCurrentHealth(float amount);

        /// <summary>
        /// Updates mob attack damage stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateAttackDamage(float amount);

        /// <summary>
        /// Updates mob attack speed stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateAttackSpeed(float amount);

        /// <summary>
        /// Updates mob movement speed stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateMovementSpeed(float amount);
        #endregion
    }
}