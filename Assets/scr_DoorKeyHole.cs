using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class scr_DoorKeyHole : MonoBehaviour
{
    public UnityEvent doorOpen;

    public Transform keyposition;

    private void OnTriggerEnter(Collider other)
    {



        if(other.CompareTag("Door Key"))
        {
            other.transform.SetParent(transform);
            other.transform.SetPositionAndRotation(keyposition.position, Quaternion.identity);

            Destroy(other.GetComponent<XRGrabInteractable>());
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.GetComponent<Rigidbody>().useGravity = false;



            doorOpen.Invoke();
        }
    }
}
