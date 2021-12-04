using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private float _attackPower = 1;
    private bool _canDamage = true;
    
    private void OnTriggerEnter(Collider other)
    {
        IDamageable hit = other.GetComponent<IDamageable>();
        if (hit == null) return;
        hit.Damage((int)_attackPower);
    }
}
