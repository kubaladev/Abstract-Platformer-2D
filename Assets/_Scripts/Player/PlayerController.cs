using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpForce = 5f;
    [SerializeField] GameObject _feet;
    IGroundCheck _iGroundCheck;
    Rigidbody2D rb;
    float _xInput;
    bool _jumpScheduled = false;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _iGroundCheck = _feet.GetComponent<IGroundCheck>();
    }

    void Update()
    {
        // Basic left and right movement
         _xInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            _jumpScheduled = true;
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(_xInput * _moveSpeed, rb.velocity.y);

        // Check if the player is grounded
        bool isOnGround = _iGroundCheck.IsOnGround();

        // Jump
        if (isOnGround && _jumpScheduled)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpForce);
            _jumpScheduled = false;
        }
    }
}