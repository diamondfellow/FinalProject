using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Steamworks;
using UnityEngine.SceneManagement;

public class PlayerLobby : NetworkBehaviour
{
    public List<NetworkConnection> players = new List<NetworkConnection>();
    //[SerializeField] private bool useSteam = false;
    [SyncVar(hook =nameof(ClentHandleDisplayNAmeUpdated))]
    private string displayName;

    public static event Action ClientOnInfoUpdated;

    private void ClentHandleDisplayNAmeUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }
    
    [Server]
    public void SetDisplayNameSteam()
    {
        displayName = SteamFriends.GetPersonaName();
    }
    [Server]
    public void SetDisplayName(string displayName) 
    {
        this.displayName = displayName;
    }
    public string GetDisplayName()
    {
        return displayName;
    }
    public void ForceDisconnect()
    {
        SceneManager.LoadScene(0);
        this.connectionToServer.Disconnect();
    }
    public override void OnStartClient()
    {
        if (!NetworkServer.active)
        {
            ((NetworkMan)NetworkManager.singleton).lobbyPlayers.Add(this);
        }

        base.OnStartClient();
    }
    public override void OnStopClient()
    {
        if (!NetworkServer.active)
        {
            ((NetworkMan)NetworkManager.singleton).lobbyPlayers.Remove(this);
        }
        ClientOnInfoUpdated?.Invoke();
        base.OnStopClient();
    }
}
