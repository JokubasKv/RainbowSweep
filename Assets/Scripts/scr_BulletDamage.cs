using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_BulletDamage : MonoBehaviour
{
    [Header("Bullet damage settings")]
    [SerializeField]
    float damage;
    GameObject target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            target.GetComponent<src_CharacterStats>().TakeDamage(damage);
        }
    }
}
