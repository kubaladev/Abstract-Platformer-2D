using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] float _dashSpeed = 10f;
    [SerializeField] float _dashDuration = 0.2f;
    [SerializeField] float _dashCooldown = 2f;
    IAcceptsOutsideForces _playerBody;
    IGravityControl _playerGravityControl;
    float _currentDashCooldown = 0f;
    bool _isDashing = false;
    Vector2 _dashDirection;
    bool _dashScheduled = false;
    SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerGravityControl = GetComponent<IGravityControl>();
        _playerBody = GetComponent<IAcceptsOutsideForces>();
    }
    private void Update()
    {
        _currentDashCooldown -= Time.deltaTime;
    }
    public static event Action<float> OnDashPerformed;
    public void HandleDashInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (_currentDashCooldown < 0)
            {
                _dashScheduled = true;
            }
        }
    }
    public void HadleDashDirection(InputAction.CallbackContext callbackContext)
    {
        _dashDirection = callbackContext.ReadValue<Vector2>();
        if (_dashDirection == Vector2.zero)
        {
            bool facingLeft = _spriteRenderer.flipX;
            _dashDirection = facingLeft ? new Vector2(-1, 0.1f) : new Vector2(1, 0.1f);
        }
    }
    private void FixedUpdate()
    {
        if (_currentDashCooldown <= 0 && _dashScheduled)
        {
            StartCoroutine(Dash());
        }
    }
    public IEnumerator Dash()
    {
        if (_isDashing) yield break;
        _dashScheduled = false;
        _currentDashCooldown = _dashDuration + _dashCooldown;
        Vector2 movementVector = _dashDirection.normalized;
        Vector2 actualVelocity = _playerBody.GetVelocity();
        if ((movementVector.y > 0 && actualVelocity.y < 0) || (movementVector.y < 0 && actualVelocity.y > 0))
        {
            _playerBody.ResetVelocity(false, true);
        }
        _isDashing = true;
        movementVector *= _dashSpeed;
        _playerBody.SetContinoiusForce(movementVector);
        _playerGravityControl.DeactivateGravity();
        OnDashPerformed?.Invoke(_dashDuration);
        yield return new WaitForSeconds(_dashDuration);
        _isDashing = false;
        _playerBody.UnsetContinoiusForce(movementVector);
        _playerGravityControl.ActivateGravity();
    }
}
