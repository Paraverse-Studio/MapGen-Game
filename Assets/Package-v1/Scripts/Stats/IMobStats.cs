namespace Paraverse.Mob.Stats
{
    public interface IMobStats
    {
        // Properties 
        #region Properties
        public int MaxHealth { get; }
        public int CurHealth { get; }
        public float AttackDamage { get; }
        public float AttackSpeed { get; }
        public float MoveSpeed { get; }
        public float MaxEnergy { get; }
        public float CurrentEnergy { get; }
        public float EnergyRegen { get; }



        public int Gold { get; }
        #endregion

        // Methods
        #region Update Stat Methods
        /// <summary>
        /// Updates mob health stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateMaxHealth(int amount);

        /// <summary>
        /// Updates mob current health stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateCurrentHealth(int amount);

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

        /// <summary>
        /// Updates mob energy stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateMaxEnergy(float amount);

        /// <summary>
        /// Updates mob current energy stat value.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateCurrentEnergy(float amount);

        /// <summary>
        /// Resets mob stats.
        /// </summary>
        /// <param name="amount"></param>
        public void ResetStats();

        /// <summary>
        /// Consumes mob energy upon dive.
        /// </summary>
        /// <param name="amount"></param>
        public void ConsumeDiveEnergy();

        /// <summary>
        /// Updates mob current gold.
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateGold(int amount);
        #endregion
    }
}