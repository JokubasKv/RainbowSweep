using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class scr_ActivateTeleportationRay : MonoBehaviour
{
    //public GameObject baseControllerGameObject;
    //public GameObject teleportationGameObject;

    public InputActionReference teleportActivationReference;
    public InputActionReference teleportCancelReference;

    public UnityEvent onTeleportActivate;
    public UnityEvent onTeleportCancel;


    private void Update()
    {
        //Debug.Log(teleportCancelReference.action.ReadValue<float>());
        if(teleportActivationReference.action.ReadValue<Vector2>().y > 0.1f && teleportCancelReference.action.ReadValue<float>() == 0)
        {
            onTeleportActivate.Invoke();
        }
        else
        {
            Invoke("DeactivateTeleport", 0.1f);
        }

    }

    void DeactivateTeleport()
    {
        onTeleportCancel.Invoke();
        Destroy(GameObject.Find("Reticle(Clone)"));
    }

}
