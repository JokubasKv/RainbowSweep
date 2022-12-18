using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Knockack_SpellSplash : MonoBehaviour
{
    
    public float lifetime=1f;
    public Transform playerTransform;

    public float knocbackForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;



        transform.rotation =Quaternion.LookRotation( Vector3.RotateTowards(transform.forward, transform.position - playerTransform.position,360f,0f));

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 direction = other.transform.position - playerTransform.position;
            src_Enemy script;
            if (script = other.GetComponent<src_Enemy>())
            {
                script.Knocback(direction, knocbackForce);
            }
            else if (script = other.GetComponentInParent<src_Enemy>())
            {
                script.Knocback(direction, knocbackForce);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime <= 0f)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
