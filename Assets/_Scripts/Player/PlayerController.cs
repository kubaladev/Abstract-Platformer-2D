using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IAcceptsOutsideForces
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpForce = 5f;
    [SerializeField] float _maxFallSpeed = 7f;
    [SerializeField] GameObject _feet;
    [SerializeField] float _gravity = 9.81f;
    [SerializeField] float _jumpMaxCooldown;
    IGroundCheck _iGroundCheck;
    Vector2 _outsideContiniousForce;
    List<Rigidbody2D> _followedObjects=new List<Rigidbody2D>();
    Rigidbody2D rb;
    float _xInput;
    bool _jumpScheduled = false;
    float _jumpLenghtTimer = 0;
    float _yVelocity =0;
    float _jumpCooldown;
    public static event Action OnJumpPerformed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _iGroundCheck = _feet.GetComponent<IGroundCheck>();
    }

    public void HandleMovementInput(InputAction.CallbackContext callbackContext)
    {
        _xInput = callbackContext.ReadValue<float>();
    }
    public void HandleJumpInput(InputAction.CallbackContext callbackContext)
    {
        if (!_iGroundCheck.IsOnGround()) return;
        else if (callbackContext.started)
        {
            _jumpLenghtTimer = Time.time;
        }
        else if (callbackContext.canceled || callbackContext.performed)
        {
            _jumpScheduled = true;
            _jumpLenghtTimer = Time.time- _jumpLenghtTimer;
        }
    }
    private void FixedUpdate()
    {
        Vector2 totalOutsideVelocity = _outsideContiniousForce;
        foreach (Rigidbody2D rb in _followedObjects)
        {
            totalOutsideVelocity += rb.velocity;
        }

        //Gravity
        bool isOnGround = _iGroundCheck.IsOnGround();
        if (isOnGround && _yVelocity < 0)
        {
            _yVelocity = 0;
        }
        else 
        {
            _yVelocity -= _gravity * Time.fixedDeltaTime;
        }

        // Jump
        if (isOnGround && _jumpScheduled)
        {
            Jump();
        }
        float clampedYVelocity = Mathf.Clamp(_yVelocity + totalOutsideVelocity.y, -_maxFallSpeed, 100);
        rb.velocity = new Vector2(_xInput * _moveSpeed + totalOutsideVelocity.x, clampedYVelocity);
    }

    void Jump()
    {
        float jumpTime = Mathf.Clamp(_jumpLenghtTimer, 0.01f, 0.2f);
        if (_yVelocity < 0) _yVelocity = 0;
        _yVelocity += _jumpForce * 0.5f + _jumpForce * jumpTime * 0.5f / 0.2f;
        _jumpScheduled = false;
        _jumpLenghtTimer = 0;
        OnJumpPerformed?.Invoke();
    }
    public void SetContinoiusForce(Vector2 force)
    {
        _outsideContiniousForce += force;
    }

    public void UnsetContinoiusForce(Vector2 force)
    {
        _outsideContiniousForce -= force;
    }

    public void ApplyImmediateForce(Vector2 force,bool resetsVelocity)
    {
        if (resetsVelocity) _yVelocity = 0;
        _yVelocity += force.y;
    }

    public void FollowObject(Rigidbody2D followedRB)
    {
        _followedObjects.Add(followedRB);
    }

    public void UnFollowObject(Rigidbody2D followedRB)
    {
        _followedObjects.Remove(followedRB);
    }

}