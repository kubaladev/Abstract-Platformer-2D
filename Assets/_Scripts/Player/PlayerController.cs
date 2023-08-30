using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IAcceptsOutsideForces, IGravityControl
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpForce = 5f;
    [SerializeField] float _maxFallSpeed = 7f;
    [SerializeField] GameObject _feet;
    [SerializeField] float _gravity = 9.81f;
    [SerializeField] float _jumpMaxCooldown;
    [SerializeField] float _minJumpCooldown = 0.2f;
    IGroundCheck _iGroundCheck;
    Vector2 _outsideContiniousForce;
    List<Rigidbody2D> _followedObjects=new List<Rigidbody2D>();
    Rigidbody2D _rigidBody2D;
    float _xInput;
    bool _jumpScheduled = false;
    float _jumpLenghtTimer = 0;
    float _yVelocity =0;
    float _xVelocity = 0;
    float _jumpCooldown;
    bool _gravityAcitvated = true;
    public static event Action OnJumpPerformed;

    void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
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
        else if ((callbackContext.canceled || callbackContext.performed ) && _jumpScheduled == false && _jumpCooldown<0)
        {
            _jumpScheduled = true;
            _jumpLenghtTimer = Time.time- _jumpLenghtTimer;
        }
    }

    private void Update()
    {
        _jumpCooldown -= Time.deltaTime;
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
        else if(_gravityAcitvated)
        {
            _yVelocity -= _gravity * Time.fixedDeltaTime;
        }

        // Jump
        if (isOnGround && _jumpScheduled)
        {
            Jump();
        }
        _xVelocity = _xInput * _moveSpeed + totalOutsideVelocity.x;
        float clampedYVelocity = Mathf.Clamp(_yVelocity + totalOutsideVelocity.y, -_maxFallSpeed, 100);
        _rigidBody2D.velocity = new Vector2(_xVelocity, clampedYVelocity);
    }

    void Jump()
    {
        _jumpCooldown = _minJumpCooldown;
        float maxButtonDuration = 0.2f;
        float jumpTime = Mathf.Clamp(_jumpLenghtTimer, 0.01f, maxButtonDuration);
        if (_yVelocity < 0) _yVelocity = 0;
        _yVelocity += _jumpForce * 0.5f + _jumpForce * jumpTime * 0.5f / maxButtonDuration;
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

    public void ApplyImmediateForce(Vector2 force)
    {
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

    public void ResetVelocity(bool resetX, bool resetY)
    {
        //TODO _xVelocity is not calculated properly
        if (resetX) _xVelocity = 0;
        if (resetY) _yVelocity = 0;
    }

    public Vector2 GetVelocity()
    {
        return new Vector2(_xVelocity, _yVelocity);
    }

    public void ActivateGravity()
    {
        _gravityAcitvated = true;
    }

    public void DeactivateGravity()
    {
        _gravityAcitvated = false;
    }

    public bool IsGravityActive()
    {
        return _gravityAcitvated = true;
    }
}