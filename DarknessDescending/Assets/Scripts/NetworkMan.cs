using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkMan : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        GameManager.gameMan.playerConnections.Add(conn);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        GameManager.gameMan.playerConnections.Remove(conn);
    }
}
