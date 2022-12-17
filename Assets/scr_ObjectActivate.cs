using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class scr_ObjectActivate : MonoBehaviour
{
    public GameObject theKey;

    public UnityEvent OnKeyInsertion;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if(other.gameObject == theKey)
        {
            OnKeyInsertion.Invoke();
            Destroy(other.gameObject);
        }
    }
}
