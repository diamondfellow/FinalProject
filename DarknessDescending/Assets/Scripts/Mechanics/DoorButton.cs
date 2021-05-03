using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DoorButton : Interactables
{
    [SerializeField] private GameObject door;
    [SerializeField] private Vector3 doorOpenPos;
    [SerializeField] private Vector3 doorClosePos;

    [SerializeField] private float OpenSpeed;
    [SerializeField] private float CloseSpeed;

    private bool isOpen;
    [Server]
    public override void Interacted()
    {
        base.Interacted();
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
    [Server]
    public void Open()
    {
        while (door.transform.position.y > doorOpenPos.y)
        {
            Vector3 transfer = door.transform.position;
            transfer.y -= (.05f * CloseSpeed);
            door.transform.position = transfer;
        }
        isOpen = true;
    }
    [Server]
    public void Close()
    {
        while (door.transform.position.y < doorClosePos.y)
        {
            Vector3 transfer = door.transform.position;
            transfer.y += (.05f * OpenSpeed);
            door.transform.position = transfer;
        }
        isOpen = false;
    }
}
