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
    private bool isPaused;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject DeathImage;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private float footstepTimerCheck;


    private float footstepTimer;
    [ClientCallback]
    private void Start()
    {
        mainCam = Camera.main;
        if (!hasAuthority)
        {
            GetComponent<AudioListener>().enabled = false;
        }
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            ClosePause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPause();
        }
        if (isPaused) { return; }
        footstepTimer += Time.deltaTime;

        if (footstepTimer > footstepTimerCheck)
        {
            CmdPlayFootstep();
        }

        
        #region Interact
        if (Input.GetKeyUp(KeyCode.E)&& !isDead)
        {
            Ray CameraRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            if (Physics.Raycast(CameraRay, out hit, interactDistance))
            {
                Interactables objectInteract;
                if (hit.collider.transform.gameObject.TryGetComponent<Interactables>(out objectInteract))
                {
                    CmdInteract(objectInteract);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && !isDead)
        {
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
        if (cameraRotation.x > 85 && cameraRotation.x < 300)
        {
            cameraRotation.x = 300;
        }
        else if (cameraRotation.x < 295 && cameraRotation.x > 80)
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
        if (spectate)
        {
            Debug.Log("spec");
            gameObject.GetComponent<Rigidbody>().velocity = ((lookingStrafe + lookingAt) * moveSpeed);
        }
        else if (canMove)
        {
            lookingStrafe = new Vector3(lookingStrafe.x, 0, lookingStrafe.z);
            lookingAt = new Vector3(lookingAt.x, 0, lookingAt.z);
            gameObject.GetComponent<Rigidbody>().velocity = ((lookingStrafe + lookingAt) * moveSpeed);
        }
        if (gameObject.GetComponent<Rigidbody>().velocity.x < .1 && gameObject.GetComponent<Rigidbody>().velocity.z < .1)
        {
            footstepTimer = 0;
        }
        #endregion
    }
    [Command]
    private void CmdPlayFootstep()
    {
        if (gameObject.GetComponent<AudioSource>().isPlaying)
        {
            return;
        }
        string soundID = "Footstep0" + Random.Range(1, 4);
        GameManager.gameMan.RpcPlaySound(gameObject, soundID);
    }
    [Client]
    public void SeeMove()
    {
        if (!spectate)
        {
            loadingCanvas.SetActive(!loadingCanvas.activeSelf);
            canMove = !canMove;
        }
    }
    [Command]
    private void CmdInteract(Interactables objectToInteract)
    {
        objectToInteract.Interacted();
    }

    #region DeathCode

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            spectate = true;
            isDead = true;
            loadingCanvas.SetActive(true);
            DeathImage.SetActive(true);
            if (headlamp.enabled)
            {
                RpcFlashlight();
            }
            RpcKillPlayer();
        }
    }
    [ClientRpc]
    public void RpcKillPlayer()
    {
        Debug.Log("RpcKoll");
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }
    #endregion
    #region FlashLight
    [Command]
    private void CmdFlashligt()
    {
        RpcFlashlight();
    }
    [ClientRpc]
    private void RpcFlashlight()
    {
        if (headlamp.enabled)
        {
            GameManager.gameMan.RpcPlaySound(gameObject, "FlashlightOff");
        }
        else
        {
            GameManager.gameMan.RpcPlaySound(gameObject, "FlashlightOn");
        }

    }
    #endregion
    #region UICode
    [Client]
    public void Spectate()
    {
        moveSpeed *= 3;
        loadingCanvas.SetActive(false);
        DeathImage.SetActive(false);
    }
    [Client]
    public void OpenPause()
    {
        isPaused = true;
        pauseMenu.enabled = true;
    }
    [Client]
    public void ClosePause()
    {
        isPaused = false;
        pauseMenu.enabled = false;
    }
    [Client]
    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkMan.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
    [Client]
    public void Settings()
    {

    }
    #endregion
}
