using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IAcceptsOutsideForces
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpForce = 5f;
    [SerializeField] float _maxFallSpeed = 7f;
    [SerializeField] GameObject _feet;
    IGroundCheck _iGroundCheck;
    Vector2 _outsideContiniousForce;
    List<Rigidbody2D> _followedObjects=new List<Rigidbody2D>();
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
        // Check if the player is grounded
        bool isOnGround = _iGroundCheck.IsOnGround();
        float yVelocity = Mathf.Clamp(rb.velocity.y, -_maxFallSpeed,100);
        Vector2 totalOutsideVelocity = _outsideContiniousForce;
        foreach(Rigidbody2D rb in _followedObjects)
        {
            totalOutsideVelocity += rb.velocity;
        }
        rb.velocity = new Vector2(_xInput * _moveSpeed + totalOutsideVelocity.x, yVelocity+ totalOutsideVelocity.y);


        // Jump
        if (isOnGround && _jumpScheduled)
        {
            float jumpTime = Mathf.Clamp(_jumpLenghtTimer, 0.01f, 0.2f);
            float jumpPower = _jumpForce * 0.5f + _jumpForce * jumpTime * 0.5f/0.2f;
            //Debug.Log($"Jump time {jumpTime}  Jump Power {jumpPower}");
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            _jumpScheduled = false;
            _jumpLenghtTimer = 0;
        }
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
        rb.velocity += force;
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