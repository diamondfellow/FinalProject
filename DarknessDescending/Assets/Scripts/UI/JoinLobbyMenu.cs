using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenyUI;
    [SerializeField] private InputField addressInput;
    [SerializeField] private Button joinButton;

    public static bool joinMenuOpen = false;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && joinMenuOpen)
        {
            Join();
        }
    }
    private void OnEnable()
    {
        NetworkMan.ClientOnConnected += HandleClientConnected;
        NetworkMan.ClientOnDisConnected+= HandleClientDisconnected;
    }
    private void OnDisable()
    {
        joinMenuOpen = false;
        NetworkMan.ClientOnConnected -= HandleClientConnected;
        NetworkMan.ClientOnDisConnected -= HandleClientDisconnected;
    }
    
    public void Join()
    {
        string address = addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        mainMenyUI.SetActive(false);
    }
    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
