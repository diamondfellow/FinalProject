using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLobby : NetworkBehaviour
{
    private string username = null;
    [TargetRpc]
    private void GetUsernamePref()
    {
        username = PlayerPrefs.GetString("username");
    }
}
