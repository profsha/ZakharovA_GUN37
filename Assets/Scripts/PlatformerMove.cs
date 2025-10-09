using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public float groundCheckAngle = 45f; // Угол, при котором считается, что персонаж на земле
    public LayerMask groundLayerMask = ~0; // Слои, считающиеся "землёй"

    private float _horizontalInput;
    private bool _isJumping;
    private bool _isGrounded = false;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
            _rb = gameObject.AddComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Ввод
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _isJumping = Input.GetButtonDown("Jump");
        // Движение
        _rb.velocity = new Vector2(_horizontalInput * moveSpeed, _rb.velocity.y);

        // Прыжок только если на земле и кнопка нажата
        if (_isGrounded && _isJumping)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            _isGrounded = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Vector2.Angle(contact.normal, Vector2.up) <= groundCheckAngle)
                {
                    _isGrounded = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            _isGrounded = false;
        }
    }

    private bool IsGroundLayer(GameObject obj)
    {
        return ((1 << obj.layer) & groundLayerMask) != 0;
    }
}
