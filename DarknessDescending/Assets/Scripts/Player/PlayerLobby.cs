using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerLobby : NetworkBehaviour
{
    //[SyncVar(hook =nameof(ClentHandleDisplayNAmeUpdated))]
    private string displayName;

    public static event Action ClientOnInfoUpdated;
    /* private static object ClentHandleDisplayNAmeUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
       // return;
    }
    */
    [Server]
    public void SetDisplayName() 
    {

    }

}
