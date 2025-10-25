using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallLauncher : MonoBehaviour
{
    [SerializeField]
    private float maxPower = 20f;
    [SerializeField]
    private float minPower = 2f;
    private BowlingGame bowlingGame;
    private Plane groundPlane;
    private LineRenderer lineRenderer;
    private Camera cam;
    private bool isAiming = false;
    private Vector3 aimTarget;
    private Rigidbody rb;

    void Start()
    {
        bowlingGame = FindObjectOfType<BowlingGame>();
        groundPlane = new Plane(Vector3.up, new Vector3(0f, 0.5f, 0f));
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isAiming = true;
                    lineRenderer.enabled = true;
                }
            }
        }

        if (Input.GetMouseButton(0) && isAiming)
        {
            UpdateAimLine();
        }

        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            Shoot();
            isAiming = false;
            lineRenderer.enabled = false;
        }

        if (!isAiming)
        {
            lineRenderer.enabled = false;
        }
    }

    void UpdateAimLine()
    {
        // Получаем позицию мыши на плоскости Y=0 (или Y=0.01, если мяч выше)
        Vector3 planePoint = GetMouseWorldPosOnPlane();
        if (planePoint != Vector3.zero)
        {
            Vector3 ballPos = transform.position;

            aimTarget = planePoint;

            lineRenderer.SetPosition(0, ballPos);
            lineRenderer.SetPosition(1, planePoint);
        }
    }

    Vector3 GetMouseWorldPosOnPlane()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    void Shoot()
    {
        bowlingGame.StartThrow();
        Vector3 ballPos = transform.position;
        Vector3 direction = (aimTarget - ballPos).normalized;
        float distance = Vector3.Distance(aimTarget, ballPos);
        float power = Mathf.Clamp(distance, minPower, maxPower);
        rb.AddForce(direction * power, ForceMode.Impulse);
    }
}
