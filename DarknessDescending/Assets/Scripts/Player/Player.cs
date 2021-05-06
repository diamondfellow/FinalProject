using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField] private float interactDistance;
    [SerializeField] private Light headlamp;

    public bool canMove = false;
    public bool spectate = false;
    public bool isDead = false;

    private Camera mainCam;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Canvas loadingCanvas;

    private void Awake()
    {
        mainCam = Camera.main;
        
    }
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }
        #region Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray CameraRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            if (Physics.Raycast(CameraRay, out hit))
            {
                Interactables objectInteract;
                if (hit.collider.transform.gameObject.TryGetComponent<Interactables>(out objectInteract))
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
        #endregion
        #region lookcode
        //if (!hasAuthority) { return; }
        //Left to right Camera
        Vector3 playerRotation = gameObject.transform.rotation.eulerAngles;
        playerRotation.y += (lookSpeed * Input.GetAxis("Mouse X") * Time.deltaTime);
        Quaternion transfer = Quaternion.Euler(playerRotation);
        gameObject.transform.rotation = transfer;
        // Up and Down Camera
        Vector3 cameraRotation = mainCam.transform.rotation.eulerAngles;
        cameraRotation.x += -(lookSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime);
        if(cameraRotation.x > 85 && cameraRotation.x < 300)
        {
            cameraRotation.x = 300;
        }
        else if(cameraRotation.x < 295 && cameraRotation.x > 80 )
        {
            cameraRotation.x = 80;
        }      
        Quaternion transferUpDown = Quaternion.Euler(cameraRotation);
        mainCam.transform.rotation = transferUpDown;
        #endregion
        #region Move
        float horiInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        Vector3 lookingAt = gameObject.transform.forward * vertInput;
        Vector3 lookingStrafe = gameObject.transform.right * horiInput;
        if (!spectate)
        {
            lookingStrafe = new Vector3(lookingStrafe.x, 0, lookingStrafe.z);
            lookingAt = new Vector3(lookingAt.x, 0, lookingAt.z);
        }
        if (spectate)
        {
            gameObject.GetComponent<Rigidbody>().velocity = ((lookingStrafe + lookingAt) * moveSpeed);
        }
        else if (canMove)
        {
            gameObject.GetComponent<Rigidbody>().velocity = ((lookingStrafe + lookingAt) * moveSpeed);
        }
        #endregion
    }
    [Client]
    public void SeeMove()
    {
        loadingCanvas.enabled = !loadingCanvas.enabled;
        canMove = !canMove;
    }
    [Command]
    private void Interact(Interactables objectToInteract)
    {
        objectToInteract.Interacted();
    }

    #region DeathCode

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == ("Monster"))
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            isDead = true;
            RpcKillPlayer();

        }
    }
    [ClientRpc]
    private void RpcKillPlayer()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }
    #endregion
    #region FlashLight
    [Command]
    private void CmdFlashligt()
    {
        SvrFlashlight();
    }
    [Server]
    private void SvrFlashlight()
    {
        ClntFlashlight();
    }
    [ClientRpc]
    private void ClntFlashlight()
    {
        headlamp.enabled = !headlamp.enabled;
    }
    #endregion


}
