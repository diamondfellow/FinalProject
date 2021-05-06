using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DoorButton : Interactables
{
    [SerializeField] private GameObject door;
    [SerializeField] private float doorOpenPos;
    [SerializeField] private float doorClosePos;

    [SerializeField] private float OpenSpeed;
    [SerializeField] private float CloseSpeed;

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
        if (isOpen)
        {
            StartCoroutine(nameof(Close));
        }
        else
        {
            StartCoroutine(nameof(Open));
        }
    }
    [Server]
    public IEnumerator Open()
    {
        Debug.Log("yes");
        while (door.transform.position.y > doorOpenPos)
        {
            
                Vector3 transfer = door.transform.position;
                transfer.y -= (.05f * OpenSpeed);
                door.transform.position = transfer;
                yield return new WaitForSeconds(.01f);
  
        }
        isOpen = true;
    }
    [Server]
    public IEnumerator Close()
    {
        Debug.Log("yes");
        while (door.transform.position.y < doorClosePos)
        {
   
            
                Vector3 transfer = door.transform.position;
                transfer.y += (.05f * CloseSpeed);
                door.transform.position = transfer;
            yield return new WaitForSeconds(.05f);

        }
        isOpen = false;
    }
}
