using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Mirror;
public class DoorButton : Interactables
{
    [SerializeField] private GameObject door;
    [SerializeField] private float doorOpenPos;
    [SerializeField] private float doorClosePos;

    [SerializeField] private float OpenSpeed;
    [SerializeField] private float CloseSpeed;

    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource doorSound;
    [SerializeField] private bool isCoolDoor;

    [SerializeField] private Color ButtonOn;
    [SerializeField] private Color ButtonOff;
    [SerializeField] private GameObject lightObject;


    private bool isMoving = false;
    private bool isOpen = true;
    private float timer = 0;
    private Coroutine coroutine;
    [ServerCallback]
    private void Update()
    {
        timer += Time.deltaTime;
    }
    [Server]
    public override void Interacted()
    {
        base.Interacted();
        GameManager.gameMan.RpcPlaySound(buttonClick.gameObject, "ButtonClick");
        if (isOpen && !isMoving)
        {
            RpcSetColor(ButtonOn);
            StartCoroutine(nameof(Close));
        }
        else if (!isMoving)
        {
            RpcSetColor(ButtonOff);
            StartCoroutine(nameof(Open));
        }
        return;
    }
    [ClientRpc]
    private void RpcSetColor(Color color)
    {
        lightObject.GetComponent<MeshRenderer>().material.color = color;
    }

    [Server]
    public IEnumerator Open()
    {
        isMoving = true;
        while (door.transform.position.y > doorOpenPos)
        {

            Vector3 transfer = door.transform.position;
            transfer.y -= (.05f * OpenSpeed);
            door.transform.position = transfer;
            yield return new WaitForSeconds(.01f);

        }
        GameManager.gameMan.RpcPlaySound(doorSound.gameObject, "DoorOpenSlam");
        isOpen = true;
        isMoving = false;
    }
    [Server]
    public IEnumerator Close()
    {
        isMoving = true;
        GameManager.gameMan.RpcPlaySound(doorSound.gameObject,"DoorClosing");
        while (door.transform.position.y < doorClosePos)
        {


            Vector3 transfer = door.transform.position;
            transfer.y += (.05f * CloseSpeed);
            door.transform.position = transfer;
            yield return new WaitForSeconds(.05f);

        }
        GameManager.gameMan.RpcPlaySound(doorSound.gameObject, "DoorClose");
        isOpen = false;
        isMoving = false;
        if (isCoolDoor && GameManager.gameMan.endingDoors)
        {
            GameManager.gameMan.DoorsClosed();
        }
    }
}
