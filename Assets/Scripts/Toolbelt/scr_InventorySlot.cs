using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scr_InventorySlot : MonoBehaviour
{
    public InputActionProperty leftHandGrab;
    public InputActionProperty rightHandGrab;

    public GameObject itemInSlot;

    GameObject currentColldingObject;
    private void Awake()
    {
        leftHandGrab.action.canceled += InsertItem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemInSlot != null) return;
        if (!IsItem(other.gameObject)) return;
        currentColldingObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other == currentColldingObject)
        {
            currentColldingObject = null;
        }
    }

    bool IsItem(GameObject obj)
    {
        return obj.GetComponent<scr_Item>();
    }
    void InsertItem(InputAction.CallbackContext context)
    {
        if (currentColldingObject == null) return;

        GameObject obj = currentColldingObject;
        scr_Item item = obj.GetComponent<scr_Item>();
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.SetParent(gameObject.transform, true);
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localPosition = item.slotRotation;
        item.inSlot = true;
        item.currentSlot = this;
        itemInSlot = obj;
    }
    
}
