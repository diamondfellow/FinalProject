using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMVE : MonoBehaviour
{
    public bool canMove = false;
    public bool spectate = false;
    

    private Camera mainCam;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Canvas loadingCanvas;
    private void Awake()
    {
        /*
        if (!hasAuthority)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Destroy( transform.GetChild(i).gameObject);
                NetworkServer.Destroy(transform.GetChild(i).gameObject);
            }
        }
        */
        mainCam = Camera.main;
    }
    //[ClientCallback]
    // Update is called once per frame
    private void Update()
    {
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
    }
    [Client]
    public void SeeMove()
    {
        loadingCanvas.enabled = !loadingCanvas.enabled;
        canMove = !canMove;
    }
}
