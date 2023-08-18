using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public abstract class Enemy : MonoBehaviour, IDamageable
{
    public static event Action<int> OnEnemyKilled;
    [field: SerializeField] public int Health { get; private set; } = 1;
    [field: SerializeField] public int Score { get; private set; } = 3;
    protected SpriteRenderer _spriteRenderer;
    protected Animator _animator;
    protected Rigidbody2D _rigidbody2D;

    protected virtual void Awake()
    {
        TryGetComponent<SpriteRenderer>(out _spriteRenderer);
        TryGetComponent<Animator>(out _animator);
        TryGetComponent<Rigidbody2D>(out _rigidbody2D);
        
    }
    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        OnEnemyKilled?.Invoke(Score);
        Debug.Log($"{this.gameObject.name} was killed.");
        SpawnDummy();
        Destroy(this.gameObject);
    }
    void SpawnDummy() 
    {
        GameObject dummy = new GameObject("dummy");
        dummy.transform.position = transform.position;
        dummy.transform.localScale = transform.localScale;
        dummy.transform.rotation = transform.rotation;
        SpriteRenderer dummySpriteRenderer =dummy.AddComponent<SpriteRenderer>();
        dummySpriteRenderer.sprite = _spriteRenderer.sprite;
        dummySpriteRenderer.flipX = _spriteRenderer.flipX;
        dummy.AddComponent<EnemyFadeout>();
    }
}
