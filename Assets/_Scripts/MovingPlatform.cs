using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Rigidbody2D _rigidbody2D;
    [SerializeField] Vector2 _speed;
    [SerializeField] float _moveInterval = 2f;
    [SerializeField] float _waitInterval = 1f;
    [SerializeField] bool _isActive = true;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        StartCoroutine(MovementCoroutine());
    }
    IEnumerator MovementCoroutine()
    {
        while (_isActive && (_moveInterval >0 || _waitInterval>0))
        {
            ChangeSpeed(_speed);
            yield return new WaitForSeconds(_moveInterval);
            ChangeSpeed(Vector2.zero);
            yield return new WaitForSeconds(_waitInterval);
            ChangeSpeed(_speed * -1);
            yield return new WaitForSeconds(_moveInterval);
            ChangeSpeed(Vector2.zero);
            yield return new WaitForSeconds(_waitInterval);
        }
    }
    void ChangeSpeed(Vector2 speed)
    {
        _rigidbody2D.velocity = speed;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IAcceptsOutsideForces touchingPlayer = collision.gameObject.GetComponent<IAcceptsOutsideForces>();
            touchingPlayer.FollowObject(_rigidbody2D);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IAcceptsOutsideForces touchingPlayer = collision.gameObject.GetComponent<IAcceptsOutsideForces>();
            touchingPlayer.UnFollowObject(_rigidbody2D);
        }
    }
}
