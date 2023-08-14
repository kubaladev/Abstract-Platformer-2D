using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpForce = 5f;
    [SerializeField] float _maxFallSpeed = 7f;
    [SerializeField] GameObject _feet;
    IGroundCheck _iGroundCheck;
    Rigidbody2D rb;
    float _xInput;
    bool _jumpScheduled = false;
    float _jumpLenghtTimer = 0;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _iGroundCheck = _feet.GetComponent<IGroundCheck>();
    }

    void Update()
    {
        // Basic left and right movement
        _xInput = Input.GetAxis("Horizontal");
        HandleJumpInput();

    }
    void HandleJumpInput()
    {
        if (!_iGroundCheck.IsOnGround()) return;
        if (Input.GetButton("Jump"))
        {
            _jumpLenghtTimer += Time.deltaTime;
        }

        if ((Input.GetButtonUp("Jump") || _jumpLenghtTimer >= 0.2f))
        {
            _jumpScheduled = true;
        }
    }
    private void FixedUpdate()
    {
        // Clamp fall speed
        float yVelocity = Mathf.Clamp(rb.velocity.y,-_maxFallSpeed,100);
        rb.velocity = new Vector2(_xInput * _moveSpeed, yVelocity);

        // Check if the player is grounded
        bool isOnGround = _iGroundCheck.IsOnGround();

        // Jump
        if (isOnGround && _jumpScheduled)
        {
            float jumpTime = Mathf.Clamp(_jumpLenghtTimer, 0.01f, 0.2f);
            float jumpPower = _jumpForce * 0.5f + _jumpForce * jumpTime * 0.5f/0.2f;
            Debug.Log($"Jump time {jumpTime}  Jump Power {jumpPower}");
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            _jumpScheduled = false;
            _jumpLenghtTimer = 0;
        }
    }
}