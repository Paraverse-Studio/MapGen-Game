using Paraverse.Combat;
using UnityEngine;

namespace Paraverse.Mob 
{
    public interface IMobCombat
    {
        public bool IsBasicAttacking { get; }
        public bool IsAttackLunging { get; }
        public BasicAttackSkill BasicAttackSkill { get; }
        public float BasicAtkRange { get; }
        public bool CanBasicAtk { get; }
        public bool IsSkilling { get; set; }
        public bool IsInCombat { get; }
        public Transform StrafeBackTarget { get; }
        public Transform StrafeLeftTarget { get; }
        public Transform StrafeRightTarget { get; }
        public bool IsStrafer { get; }
        public abstract void OnAttackInterrupt();
    }
}