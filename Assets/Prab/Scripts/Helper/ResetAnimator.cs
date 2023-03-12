using Paraverse.Mob.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Paraverse.Animations
{
    public class ResetAnimator : StateMachineBehaviour
    {
        [SerializeField]
        private string parameter = StringData.IsInteracting;

        [SerializeField, Tooltip("Disable boolean upon entering state. [By Default, boolean is enabled and disabled upon entering and exiting state]")]
        private bool disableUponEnter = false;

        [SerializeField, Tooltip("Enable boolean upon exiting state. [By Default, boolean is enabled and disabled upon entering and exiting state]")]
        private bool enableUponExit = false;
                
        [SerializeField, Header("Disable Attacking End-State"), Tooltip("Disables all attacking properties at the end of this state.")]
        private bool disableAttackEndState = false;

        [SerializeField, Header("Enables parameter after a delay"), Tooltip("Enable boolean after a % of the animation clip has already played")]
        private bool useEnableDelay;
        [SerializeField, Range(0.1f, 0.9f)]
        private float enableAfterAnimPercentage = 0f;
        private float eTimer = 0;

        [SerializeField, Header("Disables parameter after a delay"), Tooltip("Disable boolean after a % of the animation clip has already played")]
        private bool useDisableDelay;
        [SerializeField, Range(0.1f, 0.9f)]
        private float disableBeforeAnimPercentage = 0f;
        private float dTimer = 0;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            eTimer = 0f;
            dTimer = 0f;

            if (disableUponEnter)
                animator.SetBool(parameter, false);
            else if (!useEnableDelay)
                animator.SetBool(parameter, true);
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (useEnableDelay) // when you want a parameter to get enabled after a % of the animation is done
            {
                eTimer += Time.deltaTime;
                if ((eTimer / stateInfo.length) >= enableAfterAnimPercentage)
                    animator.SetBool(parameter, true);
            }

            if (useDisableDelay && !enableUponExit) // when you want a parameter to get disabled after a % of the animation is done
            {
                dTimer += Time.deltaTime;
                if ((dTimer / stateInfo.length) >= disableBeforeAnimPercentage)
                    animator.SetBool(parameter, false);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enableUponExit)
                animator.SetBool(parameter, true);
            else
                animator.SetBool(parameter, false);
        

            if (animator.gameObject.TryGetComponent(out MobCombat mobbie) && disableAttackEndState)
            {
                mobbie.OnAttackInterrupt();
            }
        }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
}