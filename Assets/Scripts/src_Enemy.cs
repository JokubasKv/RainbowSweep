using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class src_Enemy : src_CharacterStats
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    float damage;
    float lastAttackTime = 0;

    [SerializeField]
    float attackCooldown = 2;


    [SerializeField]
    float stoppingDistance;
    NavMeshAgent agent;

    GameObject target;
    Animator anim;


    bool isBeingKnocbacked = false;
    Vector3 knobackDirection;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        target = GameObject.FindGameObjectWithTag("Player");
        if (rb == null) gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Stops following after stoppingDistance amount
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist < stoppingDistance)
        {
            StopEnemy();
            Attack();
        }
        else
        {
            GoToTarget();
        }
    }
    private void FixedUpdate()
    {
        if (isBeingKnocbacked)
        {

        }
    }

    public IEnumerator Knocback(Vector3 direction, float force)
    {
        agent.updatePosition = false;
        rb.AddForce(direction * force, ForceMode.Impulse);

        yield return new WaitForSeconds(2f);

        agent.updatePosition = true;

    }

    private void GoToTarget()
    {
        agent.isStopped = false;
        agent.SetDestination(target.transform.position);
        anim.SetBool("isWalking", true);
    }

    private void StopEnemy()
    {
        agent.isStopped = true;
        anim.SetBool("isWalking", false);
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            target.GetComponent<src_CharacterStats>().TakeDamage(damage);
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
