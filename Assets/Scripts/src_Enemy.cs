using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class src_Enemy : MonoBehaviour
{
    [SerializeField]
    float stoppingDistance;
    NavMeshAgent agent;

    GameObject target;
    Animator anim;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // Stops following after stoppingDistance amount
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist < stoppingDistance)
        {
            StopEnemy();
        }
        else
        {
            GoToTarget();
        }
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
}
