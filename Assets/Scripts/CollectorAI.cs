using UnityEngine;
using UnityEngine.AI;

public class CollectorAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float searchRadius = 5f;
    [SerializeField] private LayerMask collectibleLayer;
    [SerializeField] private int targetCollectedCount;
    [SerializeField] private Transform returnPoint;

    private Animator animator;
    private Transform targetCollectible;
    private int collectedCount;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("IsSearching", animator.GetCurrentAnimatorStateInfo(0).IsName("Search"));
        animator.SetBool("IsCollecting", animator.GetCurrentAnimatorStateInfo(0).IsName("Collect"));
        animator.SetBool("IsFull", targetCollectedCount <= collectedCount);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Search"))
        {
            FindNearestCollectible();
        }
    }

    void FindNearestCollectible()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, collectibleLayer);

        Transform nearestVisible = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            Vector3 direction = hit.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance < closestDistance)
            {
                if (IsVisible(hit.transform))
                {
                    closestDistance = distance;
                    nearestVisible = hit.transform;
                }
            }
        }

        if (nearestVisible != null)
        {
            targetCollectible = nearestVisible;
            animator.SetTrigger("FoundCollectible");
        }
    }
    
    bool IsVisible(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        Ray ray = new Ray(transform.position, direction.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.collider.transform == target)
            {
                return true;
            }
        }

        return false;
    }

    public void CollectItem()
    {
        if (targetCollectible != null)
        {
            Destroy(targetCollectible.gameObject);
            collectedCount++;
            animator.SetTrigger("CollectDone");
        }
    }
    
    public void GoToReturnPoint()
    {
        agent.SetDestination(returnPoint.position);
    }

    public void ResetCollection()
    {
        collectedCount = 0;
    }
}