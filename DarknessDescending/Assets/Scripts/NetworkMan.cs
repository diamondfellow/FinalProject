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
    public List<PlayerLobby> lobbyPlayers = new List<PlayerLobby>();

    public GameObject lobbyPlayerGO;

    //private string[] orderedPlayerNames = new string[4];
    private bool isGameProgress = false;
   [SerializeField] private bool useSteam = false;

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
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {

            lobbyPlayers.Remove(conn.identity.gameObject.GetComponent<PlayerLobby>());
        }
        Players.Remove(conn);
        base.OnServerDisconnect(conn);
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
        bool isLobby;
        if (SceneManager.GetActiveScene().name == "Multiplayer")
        {
            isLobby = false;
            base.OnServerAddPlayer(conn);
        }
        else
        {
            isLobby = true;
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(lobbyPlayerGO, startPos.position, startPos.rotation)
                : Instantiate(lobbyPlayerGO);

            NetworkServer.AddPlayerForConnection(conn, player);          
            
        }
        if (isLobby)
        {
            lobbyPlayers.Add(conn.identity.gameObject.GetComponent<PlayerLobby>());
        }
        if (isLobby && useSteam)
        {
            conn.identity.gameObject.GetComponent<PlayerLobby>().SetDisplayNameSteam();
        }
        else if (isLobby)
        {
            conn.identity.gameObject.GetComponent<PlayerLobby>().SetDisplayName("Player " + Players.Count);
        }
        
    }
    public override void OnServerChangeScene(string newSceneName)
    {
        if(newSceneName == "MainMenu")
        {
            SceneManager.LoadScene(0);
        }
    }
   
}
