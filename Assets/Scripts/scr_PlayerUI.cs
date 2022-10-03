using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_PlayerUI : MonoBehaviour
{
    public Text healthAmount;

    src_CharacterStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<src_CharacterStats>();

    }
}
