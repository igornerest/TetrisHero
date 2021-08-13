using UnityEngine;
using MLAPI;
using TMPro;
using System.Text;
using MLAPI.SceneManagement;
using MLAPI.Connection;

public class LobbyScene : NetworkBehaviour
{
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject passwordEntryUI;
    [SerializeField] private GameObject leaveButton;

    private bool shouldRestartLobby = false;

    private void Awake()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            shouldRestartLobby = true;
        }    
    }
    
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void Update()
    {
        if (shouldRestartLobby)
        {
            shouldRestartLobby = DisconnectLocalClient();
            return;
        }

        if (!IsServer)
            return;

        if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            NetworkSceneManager.SwitchScene("GameScene");
        }
    }

    private bool DisconnectLocalClient()
    {
        bool canDisconnectAllClients = true;
        foreach (NetworkClient nc in NetworkManager.Singleton.ConnectedClientsList)
        {
            canDisconnectAllClients &= nc.PlayerObject.GetComponent<PlayerController>().CanDisconnectedFromServer();
        }

        if (NetworkManager.Singleton.IsClient && canDisconnectAllClients)
        {
            NetworkManager.Singleton.StopClient();
            return false;
        }

        if (NetworkManager.Singleton.IsHost && canDisconnectAllClients)
        {
            NetworkManager.Singleton.StopHost();
            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (!NetworkManager.Singleton)
            return;

        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public void OnClickHostButton()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(); 
    }

    public void OnClickConnectionButton()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordInputField.text);
        NetworkManager.Singleton.StartClient();
    }

    public void OnClickLeaveButton()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        } 
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }

        passwordEntryUI.SetActive(true);
        leaveButton.SetActive(false);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string password = Encoding.ASCII.GetString(connectionData);

        bool approveConnection = (password == passwordInputField.text);

        callback(true, null, approveConnection, null, null);
    }

    private void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(false);
            leaveButton.SetActive(true);
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(true);
            leaveButton.SetActive(false);
        }
    }


}
