using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_SlowBall : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public LayerMask whatIsGround;

    [Header("Ball Settings")]
    [Range(0f, 1f)]
    public float bouncinnes;
    [Header("On Spawn Settings")]
    public bool onSpawnGravity;
    [Header("On Release Settings")]
    public bool onReleaseGravity;


    [Header("On Exsplosion Settings")]
    public GameObject explosion;
    public GameObject onExsplosionObject;
    public float maxCollisions;
    public float maxLifetime;


    private int collisions;
    PhysicMaterial physics_mat;

    public bool explosionPlayed = false;


    private void Start()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();

        SpawnSetup();
    }
    private void Update()
    {
        if (collisions > maxCollisions) Explode();


        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Dissapear();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (collision.collider.CompareTag("Projectile")) return;

        if ((whatIsGround.value & (1 << collision.transform.gameObject.layer)) > 0)
        {
            Debug.Log("Collision, with terrain");
            collisions++;
        }
    }

    private void Explode()
    {
        if (explosionPlayed) return;
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        if (onExsplosionObject != null)
        {
            Instantiate(onExsplosionObject, transform.position, Quaternion.identity);
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

    public void Dissapear()
    {
        Destroy(gameObject);
    }
}
