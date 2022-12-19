using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_ExplodingBall : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    [Header("Ball Settings")]
    [Range(0f, 1f)]
    public float bouncinnes;
    [Header("On Spawn Settings")]
    public bool onSpawnGravity;
    [Header("On Release Settings")]
    public bool onReleaseGravity;


    public int explosionDamage;
    public float explosionRange;

    public float maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    private int collisions;
    PhysicMaterial physics_mat;

    public bool explosionPlayed = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        SpawnSetup();
    }
    private void Update()
    {
        if (collisions > maxCollisions) Explode();


        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Projectile")) return;

        collisions++;
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionPlayed) return;
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CompareTag("Enemy"))
            {
                src_Enemy script;
                scr_EnemyAi scriptAi;
                if (script = enemies[i].GetComponent<src_Enemy>())
                {
                    script.TakeDamage(explosionDamage);
                }
                else if (script = enemies[i].GetComponentInParent<src_Enemy>())
                {
                    script.TakeDamage(explosionDamage);
                }
                else if (scriptAi = enemies[i].GetComponent<scr_EnemyAi>())
                {
                    scriptAi.TakeDamage(explosionDamage);
                }
                else if ( scriptAi = enemies[i].GetComponentInParent<scr_EnemyAi>())
                {
                    scriptAi.TakeDamage(explosionDamage);
                }
            }
        }
        explosionPlayed = true;




        Invoke("Delay", 0f);
    }
    private void Delay()
    {
        Destroy(gameObject);
    }
    private void SpawnSetup()
    {
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bouncinnes;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = physics_mat;

        rb.useGravity = onSpawnGravity;
    }

    public void WhenReleased()
    {
        rb.useGravity = onReleaseGravity;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
