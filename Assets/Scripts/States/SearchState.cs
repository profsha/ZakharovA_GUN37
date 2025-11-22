using States;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : CommonState
{
    private float nextMoveTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var agent = animator.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        MoveToRandomPosition(agent);
        nextMoveTime = Time.time + 1f;
    }

    void MoveToRandomPosition(NavMeshAgent agent)
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10;
        randomDirection += agent.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var agent = animator.GetComponent<NavMeshAgent>();
        if (!agent.pathPending && agent.remainingDistance < 0.5f && Time.time >= nextMoveTime)
        {
            MoveToRandomPosition(agent);
            nextMoveTime = Time.time + 1f;
        }
    }
}