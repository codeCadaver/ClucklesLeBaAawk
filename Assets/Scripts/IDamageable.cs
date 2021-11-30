using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool IsAlive { get; set; }
    int Health { get; set; }

    void Damage(int damageAmount);
}
