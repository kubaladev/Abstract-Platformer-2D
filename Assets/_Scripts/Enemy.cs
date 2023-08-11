using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Enemy : MonoBehaviour
{
    public static event Action<int> OnEnemyKilled;
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int Score { get; private set; }
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
        Destroy(this.gameObject);
    }
}
