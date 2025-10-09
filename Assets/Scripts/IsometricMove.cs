using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricMove : MonoBehaviour
{
    
    [Header("Movement")]
    public float moveSpeed = 5f;

    private float _horizontalInput;
    private float _verticalInput;
    private Vector2 _moveDirection;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        _moveDirection = new Vector2(_horizontalInput, _verticalInput);

        if (_moveDirection.magnitude > 0.5f)
        {
            _moveDirection.Normalize();
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _moveDirection * moveSpeed;
    }
}
