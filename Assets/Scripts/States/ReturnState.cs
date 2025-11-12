using UnityEngine;
using UnityEngine.AI;

public class ReturnState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var collector = animator.GetComponent<CollectorAI>();
        collector.GoToReturnPoint();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var agent = animator.GetComponent<NavMeshAgent>();

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            var collector = animator.GetComponent<CollectorAI>();
            collector.ResetCollection();
            animator.SetTrigger("Returned");
        }
    }
}