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
    }
    private void OnDestroy()
    {
        NetworkMan.ClientOnConnected -= HandleCLientConnected;
        NetworkMan.IsHost -= HandleIsHost;
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
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
