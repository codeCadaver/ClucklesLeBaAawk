using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private int _numHits = 0;
    public bool IsAlive { get; private set; }
    public int Health { get; private set; }

    [SerializeField] private int _maxHealth;
    [SerializeField] private int _attackPower = 2;

    private void Start()
    {
        Health = _maxHealth;
    }

    public void Damage(int damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        IsAlive = false;
        Debug.Log($"{transform.name} is Dead");
    }

    private void OnTriggerEnter(Collider other)
    {
        _numHits++;
        IDamageable playerHit = other.GetComponent<IDamageable>();
        if (playerHit == null) return;
        playerHit.Damage(_attackPower);
    }

    private void OnCollisionEnter(Collision other)
    {
        IDamageable playerHit = other.collider.GetComponent<IDamageable>();
        playerHit.Damage(_attackPower);
        Debug.Log($"Player Damaged by: {_attackPower}");
    }
}
