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
    [SerializeField] private bool useSteam = false;
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
        NetworkMan.Players.Add(NetworkClient.connection);
        Debug.Log(NetworkMan.Players);
        base.OnStartClient();
    }
    public override void OnStopClient()
    {
        NetworkMan.Players.Remove(NetworkClient.connection);
        ClientOnInfoUpdated?.Invoke();
        base.OnStopClient();
    }
}
