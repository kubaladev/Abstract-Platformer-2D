using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : Enemy, IKillableByJump
{
    enum WalikingDirection
    {
        Right = 1,
        Left = -1
    }
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _floorCheckDistance = 0.3f;
    [SerializeField] float _wallCheckDistance = 0.5f;
    [SerializeField] float _gravityPower = 8f;
    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] Vector2 _feetOffset = new Vector2(0,0.3f);
    [SerializeField] WalikingDirection _walikingDirection = WalikingDirection.Right;
    bool _isCloseToGround;
    GameObject _feet;
    

    void CreateFeet()
    {
        _feet = new GameObject("Feet");
        Collider2D collider2D = GetComponent<Collider2D>();
        _feet.transform.position = new Vector2(transform.position.x, collider2D.bounds.min.y)+_feetOffset;
        _feet.transform.parent = this.transform;
    }

    protected override void Awake()
    {
        base.Awake();
        CreateFeet();
    }
    private void FixedUpdate()
    {
        _isCloseToGround = GroundCheck();
        ApplyGravityForce(); 
        Walk();
    }
    void Walk()
    {
        if (CheckWallInMovementDirection())
        {
            _walikingDirection =(WalikingDirection)((int)_walikingDirection * -1);
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }
        _rigidbody2D.velocity = new Vector2((int)_walikingDirection * _movementSpeed * Time.fixedDeltaTime, _rigidbody2D.velocity.y);

    }
    bool CheckWallInMovementDirection()
    {
        // Cast a ray downwards to check for ground.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * (int)_walikingDirection, _wallCheckDistance, _groundLayer);

        // If the ray hits something on the Ground layer, return true.
        return hit.collider != null;
    }
    void ApplyGravityForce()
    {
        if (_isCloseToGround)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
            _animator.speed = 1;
            return;
        }
        _animator.speed = 0;
        _rigidbody2D.velocity += _gravityPower* Time.fixedDeltaTime * Vector2.down;

    }

    bool GroundCheck()
    {
        // Cast a ray downwards to check for ground.
        RaycastHit2D hit = Physics2D.Raycast(_feet.transform.position, Vector2.down, _floorCheckDistance, _groundLayer);

        // If the ray hits something on the Ground layer, return true.
        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        if (_feet != null)
        {
            Gizmos.color = _isCloseToGround ? Color.green : Color.red;
            Gizmos.DrawRay(_feet.transform.position, Vector2.down * _floorCheckDistance);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (int)_walikingDirection * _wallCheckDistance * Vector2.right);
    }

    public void TakeHitFromJump(int damage)
    {
        TakeDamage(damage);
    }
}
