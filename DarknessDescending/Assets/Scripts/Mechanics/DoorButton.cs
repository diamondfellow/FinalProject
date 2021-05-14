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

    [SerializeField] private AudioClip[] doorSounds = new AudioClip[3]; //0 doorCloseMove 1 doorLatch 2 doorSlam

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
        //RpcPlaySound(buttonClick, buttonClick.clip);
        if (isOpen && !isMoving)
        {
            StartCoroutine(nameof(Close));
        }
        else if (!isMoving)
        {
            StartCoroutine(nameof(Open));
        }
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
        //RpcPlaySound(doorSound, doorSounds[2]);
        isOpen = true;
        isMoving = false;
    }
    [Server]
    public IEnumerator Close()
    {
        isMoving = true;
        //RpcPlaySound(doorSound, doorSounds[0]);
        while (door.transform.position.y < doorClosePos)
        {


            Vector3 transfer = door.transform.position;
            transfer.y += (.05f * CloseSpeed);
            door.transform.position = transfer;
            yield return new WaitForSeconds(.05f);

        }
        //RpcPlaySound(doorSound, doorSounds[1]);
        isOpen = false;
        isMoving = false;
    }
    [ClientRpc]
    void RpcPlaySound(GameObject soundObject)
    {
        AudioSource source = soundObject.GetComponent<AudioSource>();
    }
}
