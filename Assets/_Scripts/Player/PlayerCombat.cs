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
    int _canTakeDamage = 0;
    IAcceptsOutsideForces playerPhysics;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        playerPhysics = GetComponent<IAcceptsOutsideForces>();
        PlayerDash.OnDashPerformed += DashingDodge;
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
        playerPhysics.ResetVelocity(false, true);
        playerPhysics.ApplyImmediateForce(new Vector3(0, _bouncePower));
    }
    void LoseLife()
    {
        if (_canTakeDamage>0) return;
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
        _spriteRenderer.DOKill();
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            IKillableByJump iKillableByJump = collision.GetComponent<IKillableByJump>();
            if (iKillableByJump != null)
            {
                ResolveJumpOnEnemy(iKillableByJump, collision.transform);
            }
            else
            {
                LoseLife();
            }
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
        _canTakeDamage ++;
        Sequence mySequence = DOTween.Sequence();
        for(int i=0; i < 5; i++)
        {
            mySequence.Append(_spriteRenderer.DOFade(0, 0.25f));
            mySequence.Append(_spriteRenderer.DOFade(1, 0.25f));
        }
        mySequence.OnComplete(() => { _canTakeDamage --; mySequence.Kill(); });
        mySequence.Play();
    }
    void DashingDodge(float duration)
    {

        StartCoroutine(Dashing(duration));
    }
    IEnumerator Dashing(float duration)
    {
        _canTakeDamage++;
        if (_canTakeDamage == 1)
        {
            _spriteRenderer.DOFade(0.7f, 0f);
        }
        yield return new WaitForSeconds(duration);
        if (_canTakeDamage == 1)
        {
            _spriteRenderer.DOFade(1f, 0f);
        }
        yield return new WaitForSeconds(0.1f);
        _canTakeDamage--;
    }
    IEnumerator UpdateStartingIndicators()
    {
        yield return new WaitForFixedUpdate();
        OnPlayerLifeChanged?.Invoke(_health);
    }
    private void OnDestroy()
    {
        PlayerDash.OnDashPerformed -= DashingDodge;
    }
}
