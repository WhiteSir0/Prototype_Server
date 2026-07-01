using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{
    public InputField joinCodeInputField;
    public Text joinCodeDisplayText;
    public Text errorText;
    public Button hostButton;
    public Button joinButton;
    public GameObject menuPanel;
    public Text statusText;

    private ISession session;

    private async void Awake()
    {
        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);

        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        menuPanel.SetActive(false);
        statusText.text += $"Player {clientId} entered\n";
    }

    public async void HostGame()
    {
        errorText.text = string.Empty;
        try
        {
            var options = new SessionOptions { MaxPlayers = 2 }.WithRelayNetwork();
            session = await MultiplayerService.Instance.CreateSessionAsync(options);
            joinCodeDisplayText.text = session.Code;
        }
        catch (Exception e)
        {
            errorText.text = e.Message;
        }
    }

    public async void JoinGame()
    {
        errorText.text = string.Empty;
        try
        {
            session = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCodeInputField.text);
            joinCodeDisplayText.text = session.Code;
        }
        catch (Exception e)
        {
            errorText.text = e.Message;
        }
    }
}
