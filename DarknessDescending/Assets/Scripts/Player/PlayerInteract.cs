using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField ]private float interactDistance;
    [SerializeField] private Light headlamp;
    [ClientCallback]
    void Update()
    {
        if(!hasAuthority) { return; }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Ray CameraRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)); 
            RaycastHit hit;
            if(Physics.Raycast(CameraRay, out hit))
            {
                Interactable objectInteract;
                if (hit.collider.transform.gameObject.TryGetComponent<Interactable>(out objectInteract))
                {
                    Interact(objectInteract);
                }
            }       
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            headlamp.enabled = !headlamp.enabled;
            CmdFlashligt();
        }
    }
    #region FlashLight
    [Command]
    private void CmdFlashligt()
    {
        SvrFlashlight();
    }
    [Server] private void SvrFlashlight()
    {
        ClntFlashlight();
    }
    [ClientRpc]
    private void ClntFlashlight()
    {
        headlamp.enabled = !headlamp.enabled;
    }
    #endregion

    [Command]
    private void Interact(Interactable objectToInteract)
    {
        objectToInteract.Interacted();
    }
}
