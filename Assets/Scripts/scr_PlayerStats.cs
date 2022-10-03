using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PlayerStats : src_CharacterStats
{
    scr_PlayerUI playerUi;

    // Start is called before the first frame update
    void Start()
    {
        playerUi = GetComponent<scr_PlayerUI>();

        maxHealth = 100;
        currentHealth = maxHealth;

        SetStats();
    }

    public override void Die()
    {
        Debug.Log("GAME OVER!");
    }

    void SetStats()
    {
        playerUi.healthAmount.text = currentHealth.ToString();
    }

    public override void CheckHealth()
    {
        base.CheckHealth();
        SetStats();
    }
}
