using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField]
    private Vector3 _start, _end;
    [SerializeField]
    private float _speed, _delay;
    
    [SerializeField, Header("Gizmos Settings")]
    private float _gizmoSphereSize = 0.5f;
    [SerializeField]
    private Color _gizmoColor = Color.cyan;

    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        var start = transform.position + _start;
        var end = transform.position + _end;
        var toEnd = true;
        var rb = GetComponent<Rigidbody>();
        rb.position = start;
        while (true)
        {
            Vector3 targetPos = toEnd ? end : start;
            while (Vector3.Distance(rb.position, targetPos) > 0.1f)
            {
                rb.position = Vector3.MoveTowards(rb.position, targetPos, _speed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(_delay);

            toEnd = !toEnd;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        var start = transform.position + _start;
        var end = transform.position + _end;
        Gizmos.color = _gizmoColor;

        Gizmos.DrawSphere(start, _gizmoSphereSize);
        Gizmos.DrawSphere(end, _gizmoSphereSize);

        Gizmos.DrawLine(start, end);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(start + Vector3.up * 0.5f, "Start");
        UnityEditor.Handles.Label(end + Vector3.up * 0.5f, "End");
#endif
    }
}
