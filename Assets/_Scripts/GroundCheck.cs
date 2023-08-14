using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour, IGroundCheck
{
    bool _isGrounded;
    [SerializeField] float _checkRadius = 0.5f;
    [SerializeField] LayerMask _whatIsGround;
    public bool IsOnGround() => _isGrounded;


    // Update is called once per frame
    void Update()
    {
        // Check if the player is grounded
        _isGrounded = Physics2D.OverlapCircle(transform.position, _checkRadius, _whatIsGround);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, _checkRadius);
    }
}
