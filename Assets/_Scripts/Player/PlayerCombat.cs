using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] int _health = 3;
    [SerializeField] int _jumpDamage = 1;
    [SerializeField] float _bouncePower = 6;
    Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    void ResolveJumpOnEnemy(IKillableByJump enemy, Transform enemyTransform)
    {
        if (enemyTransform.position.y < transform.position.y && _rigidbody2D.velocity.y < 0)
        {
            enemy.TakeHitFromJump(_jumpDamage);
            Bounce();
        }
        else
        {
            LoseLife();
        }
    }

    void Bounce()
    {
        _rigidbody2D.velocity = new Vector3(_rigidbody2D.velocity.x, _bouncePower);
    }
    void LoseLife()
    {
        _health -= 1;
        if (_health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            IKillableByJump iKillableByJump = collision.GetComponent<IKillableByJump>();
            if (iKillableByJump == null) return;
            ResolveJumpOnEnemy(iKillableByJump, collision.transform);
            
        }
        else if (collision.gameObject.CompareTag("Collectible"))
        {
            ICollectible iCollectible = collision.GetComponent<ICollectible>();
            if (iCollectible == null) return;
            iCollectible.Collect();
        }
    }
}
