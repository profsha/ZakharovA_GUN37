using States;
using UnityEngine;
using UnityEngine.AI;

public class CollectState : CommonState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var agent = animator.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        var collector = animator.GetComponent<CollectorAI>();
        collector.CollectItem();
    }
}