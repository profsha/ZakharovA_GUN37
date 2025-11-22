using States;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : CommonState
{
    private float idleStartTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        idleStartTime = Time.time;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float elapsed = Time.time - idleStartTime;

        if (elapsed > 5f)
        {
            animator.SetTrigger("FinishIdle");
        }
    }
}