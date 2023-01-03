using UnityEngine;

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


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (disableUponEnter)
                animator.SetBool(parameter, false);
            else
                animator.SetBool(parameter, true);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enableUponExit)
                animator.SetBool(parameter, true);
            else
                animator.SetBool(parameter, false);
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