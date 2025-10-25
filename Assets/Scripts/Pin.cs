using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField]
    private Vector3 startPos;
    [SerializeField]
    private Quaternion startRotation;
    [SerializeField]
    private float distanceTolerance = 1.0f;
    [SerializeField]
    private float angleTolerance = 30f;

    void Start()
    {
        startPos = transform.position;
        startRotation = transform.rotation;
    }

    public bool IsKnockedOver()
    {
        float distance = Vector3.Distance(transform.position, startPos);
        Vector3 currentUp = transform.up;
        float angle = Vector3.Angle(Vector3.up, currentUp);
        return distance > distanceTolerance || angle > angleTolerance;
    }

    public void ResetPin()
    {
        gameObject.SetActive(true);
        transform.position = startPos;
        transform.rotation = startRotation;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void RemovePin()
    {
        gameObject.SetActive(false);
    }
}