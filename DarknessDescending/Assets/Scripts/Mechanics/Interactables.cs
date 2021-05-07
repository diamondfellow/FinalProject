using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Interactables : NetworkBehaviour
{
    [HideInInspector] public GameManager gameMangaer;
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
