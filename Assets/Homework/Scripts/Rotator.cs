using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotate;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        var rb = GetComponent<Rigidbody>();
        while (true)
        {
            Quaternion deltaRotation = Quaternion.Euler(_rotate * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
            yield return new WaitForFixedUpdate();
        }
    }

}
