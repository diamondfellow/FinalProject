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

    private void Awake()
    {
        mainCam = Camera.main;
    }
    //[ClientCallback]
    // Update is called once per frame
    void Update()
    {
        //if(!hasAuthority) { return; }
        //Left to right Camera
        Debug.Log("e");
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
        if (spectate)
        {

        }
        else if (canMove)
        {
            //movement 
        }

    }
}
