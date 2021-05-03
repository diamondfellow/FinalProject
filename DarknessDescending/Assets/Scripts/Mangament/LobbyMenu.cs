using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject startGame;

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
