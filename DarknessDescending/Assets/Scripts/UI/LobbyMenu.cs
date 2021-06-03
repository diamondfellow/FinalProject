using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject startGame;
    [SerializeField] private Text[] playerTexts = new Text[4];

    private void Start()
    {
        NetworkMan.ClientOnConnected += HandleCLientConnected;
        NetworkMan.IsHost += HandleIsHost;
        PlayerLobby.ClientOnInfoUpdated += HandleClientDisplayNameUpdated;
    }
    private void OnDestroy()
    {
        NetworkMan.ClientOnConnected -= HandleCLientConnected;
        NetworkMan.IsHost -= HandleIsHost;
        PlayerLobby.ClientOnInfoUpdated -= HandleClientDisplayNameUpdated;
    }

    private void HandleClientDisplayNameUpdated()
    {
        List<PlayerLobby> players = ((NetworkMan)NetworkManager.singleton).lobbyPlayers;
        for(int i = 0; i < players.Count; i++)
        {
            playerTexts[i].text = players[i].GetDisplayName();
        }
        for(int i = players.Count; i < playerTexts.Length; i++)
        {
            playerTexts[i].text = "Waiting For Player...";
        }
    }
    private void HandleUpdateNameUI(string[] nameList)
    {
        if(nameList[0] != "")
        {
            playerTexts[0].text = nameList[0];
        }
        if (nameList[1] != "")
        {
            playerTexts[1].text = nameList[1];
        }
        if (nameList[2] != "")
        {
            playerTexts[2].text = nameList[2];
        }
        if (nameList[3] != "")
        {
            playerTexts[3].text = nameList[3];
        }
    }
    
    private void HandleIsHost()
    {
        startGame.SetActive(true);
    }
    private void HandleCLientConnected()
    {
        lobbyUI.SetActive(true);
    }
    public void LeaveLobby()
    {
        startGame.SetActive(false);
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            foreach(NetworkConnection conn in NetworkMan.Players)
            {
                if(conn != NetworkClient.connection)
                {
                    conn.identity.gameObject.GetComponent<PlayerLobby>().ForceDisconnect();
                }
            }
            NetworkManager.singleton.StopHost();
            SceneManager.LoadScene(0);
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
