using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    public GameManager gameMangaer;
    [ClientCallback]
    public void Awake()
    {
        gameMangaer = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    [Server]
    public virtual void Interacted()
    {

    }
}
