using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject addressUI;
    [SerializeField] private InputField addressInput;
    [SerializeField] private Button joinButton;

    private void Awake()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            mainMenu.SetActive(false);
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Settings()
    {

    }
    public void HostLobby()
    {
        mainMenu.SetActive(false);
        NetworkManager.singleton.StartHost();
    }
    public void JoinLobby()
    {
        mainMenu.SetActive(false);
        addressUI.SetActive(true);
    }
  
}
