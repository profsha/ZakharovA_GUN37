using UnityEngine;

namespace States
{
    public class CommonState : StateMachineBehaviour
    {
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger("FinishIdle");
            animator.ResetTrigger("CollectDone");
            animator.ResetTrigger("Returned");
            animator.ResetTrigger("FoundCollectible");
        }
    }
}