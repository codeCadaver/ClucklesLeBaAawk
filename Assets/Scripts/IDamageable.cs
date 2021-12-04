using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool IsAlive { get; }
    int Health { get; }

    void Damage(int damageAmount);
    
}
