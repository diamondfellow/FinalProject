using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;

    private void Start()
    {
        NetworkMan.ClientOnConnected += HandleCLientConnected;
    }

    private void HandleCLientConnected()
    {
        lobbyUI.SetActive(true);
    }

    private void OnDestroy()
    {
        
    }
    public void LeaveLobby()
    {
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
