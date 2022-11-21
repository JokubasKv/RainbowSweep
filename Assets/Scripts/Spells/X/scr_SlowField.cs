using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;


public class scr_SlowField : MonoBehaviour
{

    public float duration = 5f;

    [Range(0f,1f)]
    public float slowStrength;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Appear();
    }
    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            animator.SetTrigger("Close");
        }
    }

    private void Appear()
    {
    }
    public void Dissapear()
    {
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")){
            other.GetComponent<NavMeshAgent>().speed *= slowStrength;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<NavMeshAgent>().speed /= slowStrength;
        }
    }
}
