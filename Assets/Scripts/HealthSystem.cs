using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    private int health;
    [SerializeField] private int healthMax = 100;

    private void Awake() {
        health = healthMax;
    }

    public void Damage(int damageAmount){
        health -=damageAmount;
        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (health < 0 || health == 0){
            health = 0;

            Die();
        }

    }

    private void Die() => OnDead?.Invoke(this, EventArgs.Empty);

    public float GetHealthNormalized() => (float)health / healthMax;
    
}
