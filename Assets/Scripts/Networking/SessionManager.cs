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
    public Text roomCodeText;

    private ISession session;

    private async void Awake()
    {
        hostButton.onClick.AddListener(HostGame);
        joinButton.onClick.AddListener(JoinGame);

        var initOptions = new InitializationOptions();
        initOptions.SetProfile(Guid.NewGuid().ToString("N").Substring(0, 30));
        await UnityServices.InitializeAsync(initOptions);
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
    }

    private void OnClientConnected(ulong clientId)
    {
        menuPanel.SetActive(false);
        errorText.text = string.Empty;
        statusText.text += $"Player {clientId} entered\n";
    }

    private void OnTransportFailure()
    {
        menuPanel.SetActive(true);
        SetButtonsInteractable(true);
        joinCodeDisplayText.text = "-";
        roomCodeText.text = string.Empty;
        statusText.text = string.Empty;
        ShowError("Connection lost, please try again");
    }

    private void SetButtonsInteractable(bool interactable)
    {
        hostButton.interactable = interactable;
        joinButton.interactable = interactable;
    }

    private void ShowConnecting()
    {
        SetButtonsInteractable(false);
        errorText.color = Color.white;
        errorText.text = "Connecting...";
    }

    private void ShowError(string message)
    {
        errorText.color = Color.red;
        errorText.text = message;
        SetButtonsInteractable(true);
    }

    public async void HostGame()
    {
        ShowConnecting();
        try
        {
            var options = new SessionOptions { MaxPlayers = 2 }.WithRelayNetwork();
            session = await MultiplayerService.Instance.CreateSessionAsync(options);
            joinCodeDisplayText.text = session.Code;
            roomCodeText.text = $"Code: {session.Code}";
            errorText.text = string.Empty;
        }
        catch (Exception e)
        {
            ShowError(e.Message);
        }
    }

    public async void JoinGame()
    {
        ShowConnecting();
        try
        {
            session = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCodeInputField.text);
            joinCodeDisplayText.text = session.Code;
            roomCodeText.text = $"Code: {session.Code}";
            errorText.text = string.Empty;
        }
        catch (Exception e)
        {
            ShowError(e.Message);
        }
    }
}
