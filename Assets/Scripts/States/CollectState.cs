using UnityEngine;

public class CollectState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var collector = animator.GetComponent<CollectorAI>();
        collector.CollectItem();
    }
}