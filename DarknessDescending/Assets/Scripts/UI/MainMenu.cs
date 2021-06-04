using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject addressUI;
    [SerializeField] private InputField addressInput;
    [SerializeField] private Button joinButton;

    [SerializeField] private bool useSteam = false;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbytJoinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Awake()
    {
        
        if (NetworkManager.singleton.isNetworkActive)
        {
            mainMenu.SetActive(false);
        }
    }
    private void Start()
    {
        if (!useSteam)
        {
            return;
        }
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbytJoinRequest = Callback<GameLobbyJoinRequested_t>.Create(GameLobbyJoin);
        lobbyEntered = Callback<LobbyEnter_t>.Create(LoobyEnter);
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            mainMenu.SetActive(true);
            return;
        }
        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress", SteamUser.GetSteamID().ToString()); 
    }
    private void GameLobbyJoin(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void LoobyEnter(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; }
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        mainMenu.SetActive(false);
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

        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }
        NetworkManager.singleton.StartHost();
    }
    public void JoinLobby()
    {
        mainMenu.SetActive(false);
        addressUI.SetActive(true);
        JoinLobbyMenu.joinMenuOpen = true;
    }
  
}
