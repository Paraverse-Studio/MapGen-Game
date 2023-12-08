using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

namespace Paraverse.Combat
{
    public class DualAttackSkill : MobSkill, IMobSkill
    {
        [SerializeField]
        private GameObject mainHandAttackCollider;
        [SerializeField]
        private GameObject offHandAttackCollider;

        #region Inheritable Methods
        public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
        {
            base.ActivateSkill(mob, anim, stats, target);
        }

        /// <summary>
        /// Registers the skills animation events to the animation event methods in combat script.
        /// </summary>
        public override void SubscribeAnimationEventListeners()
        {
            mob.OnEnableMainHandColliderSOneEvent += EnableMainHandCollider;
            mob.OnEnableOffHandColliderSOneEvent += EnableOffHandCollider;
        }

        /// <summary>
        /// Unsubscribes the skills animation events to the animation event methods in combat script.
        /// </summary>
        public override void UnsubscribeAnimationEventListeners()
        {
            mob.OnDisableMainHandColliderSOneEvent += DisableMainHandCollider;
            mob.OnDisableOffHandColliderSOneEvent += DisableOffHandCollider;
        }
        #endregion

        #region Private Methods
        private void EnableMainHandCollider()
        {
            mainHandAttackCollider.SetActive(true);
        }

        private void DisableMainHandCollider()
        {
            mainHandAttackCollider.SetActive(false);
        }

        private void EnableOffHandCollider()
        {
            offHandAttackCollider.SetActive(true);
        }

        private void DisableOffHandCollider()
        {
            offHandAttackCollider.SetActive(false);
        }
        #endregion
    }
}