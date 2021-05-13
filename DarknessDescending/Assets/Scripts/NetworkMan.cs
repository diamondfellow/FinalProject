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
    public static event Action IsHost;
    public static List<NetworkConnection> Players  = new List<NetworkConnection>();

    public GameObject lobbyPlayer;

    private string[] orderedPlayerNames = new string[4];
    private bool isGameProgress = false;


    public override void OnServerConnect(NetworkConnection conn)
    {
        Players.Add(conn);
        if (Players.Count == 1)
        {
            IsHost?.Invoke();
        }
        if (isGameProgress)
        {
            conn.Disconnect();
        }
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(isGameProgress) { GameManager.gameMan.NumberofPlayers--; }
        base.OnServerDisconnect(conn);
        Players.Remove(conn);
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
        if (isGameProgress) { GameManager.gameMan.NumberofPlayers++; }
        if (SceneManager.GetActiveScene().name == "Multiplayer")
        {
            base.OnServerAddPlayer(conn);
        }
        else
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(lobbyPlayer, startPos.position, startPos.rotation)
                : Instantiate(lobbyPlayer);

            NetworkServer.AddPlayerForConnection(conn, player);
        }

        
    }
    public override void OnServerChangeScene(string newSceneName)
    {

    }
}
