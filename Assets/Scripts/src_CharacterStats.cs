using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class src_CharacterStats : MonoBehaviour
{

    public float currentHealth;

    public float maxHealth;

    public bool isDead;

    public virtual void CheckHealth()
    {
        if (currentHealth >=maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (currentHealth <=0)
        {
            currentHealth = 0;
            isDead = true;
            Die();
            
        }
    }

    public virtual void Die()
    {
        Debug.Log(this.ToString() + " Has Died");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        CheckHealth();
    }
}
