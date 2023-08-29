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
    [SerializeField] float _dashSpeed = 10f;
    [SerializeField] float _dashDuration = 0.2f;
    [SerializeField] float _dashCooldown = 2f;
    [SerializeField] float _minJumpCooldown = 0.2f;
    float _currentDashCooldown = 0f;
    IGroundCheck _iGroundCheck;
    Vector2 _outsideContiniousForce;
    List<Rigidbody2D> _followedObjects=new List<Rigidbody2D>();
    Rigidbody2D _rigidBody2D;
    SpriteRenderer _spriteRenderer;
    float _xInput;
    bool _jumpScheduled = false;
    bool _dashScheduled = false;
    bool _isDashing = false;
    float _jumpLenghtTimer = 0;
    float _yVelocity =0;
    float _jumpCooldown;
    Vector2 _dashDirection;
    public static event Action OnJumpPerformed;
    public static event Action<float> OnDashPerformed;

    void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _iGroundCheck = _feet.GetComponent<IGroundCheck>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
    public void HandleDashInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(_currentDashCooldown < 0)
            {
                _dashScheduled = true;
            }
        }
    }
    public void HadleDashDirection(InputAction.CallbackContext callbackContext)
    {
        _dashDirection = callbackContext.ReadValue<Vector2>();
        if(_dashDirection== Vector2.zero)
        {
            bool facingLeft = _spriteRenderer.flipX;
            _dashDirection = facingLeft ? new Vector2(-1, 0.1f) : new Vector2(1, 0.1f);
        }
    }
    private void Update()
    {
        _currentDashCooldown -= Time.deltaTime;
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
        else if(!_isDashing)
        {
            _yVelocity -= _gravity * Time.fixedDeltaTime;
        }

        // Jump
        if (isOnGround && _jumpScheduled)
        {
            Jump();
        }
        if(_currentDashCooldown<= 0 && _dashScheduled)
        {
            StartCoroutine(Dash());
        }
        float clampedYVelocity = Mathf.Clamp(_yVelocity + totalOutsideVelocity.y, -_maxFallSpeed, 100);
        _rigidBody2D.velocity = new Vector2(_xInput * _moveSpeed + totalOutsideVelocity.x, clampedYVelocity);
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
    public IEnumerator Dash()
    {
        if (_isDashing) yield break;
        _dashScheduled = false;
        _currentDashCooldown = _dashDuration + _dashCooldown;
        Vector2 movementVector = _dashDirection.normalized;
        if ((movementVector.y > 0 && _yVelocity < 0) || (movementVector.y < 0 && _yVelocity > 0))
        {
            _yVelocity = 0;
        } 
        _isDashing = true;
        movementVector *= _dashSpeed;
        SetContinoiusForce(movementVector);
        OnDashPerformed?.Invoke(_dashDuration);
        yield return new WaitForSeconds(_dashDuration);
        _isDashing = false;
        UnsetContinoiusForce(movementVector);
    }

}