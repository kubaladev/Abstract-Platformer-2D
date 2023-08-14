using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    enum HorizontalState
    {
        Stabilized = 0,
        Jumping = 1,
        Falling = -1
    }
    HorizontalState _horizontalState;
    Animator _animator;
    Rigidbody2D _rigidbody2D;
    SpriteRenderer _spriteRenderer;
    IGroundCheck _iGorundCheck;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _iGorundCheck = GetComponentInChildren<IGroundCheck>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        HandleVerticalState();
        HandleHorizontalState();
    }
    void HandleVerticalState()
    {
        if (Mathf.Abs(_rigidbody2D.velocity.x) > 0.05f)
        {
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }
        if (_rigidbody2D.velocity.x < -0.05f)
        {
            _spriteRenderer.flipX = true;
        }
        else if(_rigidbody2D.velocity.x>0.05f)
        {
            _spriteRenderer.flipX = false;
        }
    }
    void HandleHorizontalState()
    {
        if (_rigidbody2D.velocity.y > 0.1f)
        {
            _horizontalState = HorizontalState.Jumping;
        }
        else if (_rigidbody2D.velocity.y < -7f)
        {
            _horizontalState = HorizontalState.Falling;
        }
        else if(_iGorundCheck.IsOnGround())
        {
            _horizontalState = HorizontalState.Stabilized;
        }
        SetHorizontalAnimation();
    }
    void SetHorizontalAnimation()
    {
        _animator.SetInteger("horizontalState", (int)_horizontalState);
    }

}
