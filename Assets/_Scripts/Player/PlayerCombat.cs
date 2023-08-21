using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] int _health = 3;
    [SerializeField] int _jumpDamage = 1;
    [SerializeField] float _bouncePower = 6;
    Rigidbody2D _rigidbody2D;
    SpriteRenderer _spriteRenderer;
    public static event Action OnPlayerKilled;
    public static event Action<int> OnPlayerLifeChanged;
    bool _canTakeDamage = true;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        StartCoroutine(UpdateStartingIndicators());
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
        if (!_canTakeDamage) return;
        _health -= 1;
        OnPlayerLifeChanged?.Invoke(_health);
        if (_health <= 0)
        {
            Die();
        }
        else
        {
            BecomeInvurnerable();
        }

    }

    void Die()
    {
        OnPlayerKilled?.Invoke();
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
        else if (collision.gameObject.CompareTag("Void"))
        {
            Die();
        }
        else if (collision.gameObject.CompareTag("Trap"))
        {
            LoseLife();
        }
    }
    void BecomeInvurnerable()
    {
        _canTakeDamage = false;
        Sequence mySequence = DOTween.Sequence();
        for(int i=0; i < 5; i++)
        {
            mySequence.Append(_spriteRenderer.DOFade(0, 0.25f));
            mySequence.Append(_spriteRenderer.DOFade(1, 0.25f));
        }
        mySequence.OnComplete(() => { _canTakeDamage = true; mySequence.Kill(); });
        mySequence.Play();
    }
    IEnumerator UpdateStartingIndicators()
    {
        yield return new WaitForFixedUpdate();
        OnPlayerLifeChanged?.Invoke(_health);
    }
}
