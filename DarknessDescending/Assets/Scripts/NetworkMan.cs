using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class NetworkMan : NetworkManager
{
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisConnected;
    public List<PlayerMVE> Players  = new List<PlayerMVE>();

    private bool isGameProgress = false;
    public override void OnServerConnect(NetworkConnection conn)
    {

        if (isGameProgress || numPlayers > 4)
        {
            conn.Disconnect();
        }
    }
    public override void OnStopServer()
    {
        Players.Clear();

        isGameProgress = false;
    }
    public void StartGame()
    {
        isGameProgress = true;

        ServerChangeScene("Multiplayer");
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisConnected?.Invoke(); 
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        PlayerMVE player = conn.identity.GetComponent<PlayerMVE>();

        Players.Add(player);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        PlayerMVE player = conn.identity.GetComponent<PlayerMVE>();
        Players.Remove(player);
    }
    public override void OnServerChangeScene(string newSceneName)
    {
        if(SceneManager.GetActiveScene().name == "Multiplayer")
        {

        }
    }
}
