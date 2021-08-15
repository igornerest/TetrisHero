using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using System;
using MLAPI.Connection;
using MLAPI.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Transform arenaPrefab;
    [SerializeField] private Vector3 firstArenaPosition = new Vector3(0, 0, 3);
    [SerializeField] private Vector3 secondArenaPosition = new Vector3(0, 0, -17);

    private ArenaManager firstArenaManager;
    private ArenaManager secondArenaManager;
    private ArenaManager currentArenaManager;

    private static int PLAYER1_ID = 1;
    private static int PLAYER2_ID = 2;

    private static GameManager instance = null;

    private static NetworkVariableSettings fullWritePermission = new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone };
    private NetworkVariableInt firstArenaGridTranslation = new NetworkVariableInt(fullWritePermission, 0);
    private NetworkVariableInt firstArenaSpawnTranslation = new NetworkVariableInt(fullWritePermission, 0);
    private NetworkVariableInt secondArenaGridTranslation = new NetworkVariableInt(fullWritePermission, 0);
    private NetworkVariableInt secondArenaSpawnTranslation = new NetworkVariableInt(fullWritePermission, 0);
    private NetworkVariableBool isGameOn = new NetworkVariableBool(fullWritePermission, false);

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return instance;
        }
    }

    public bool IsGameOn
    {
        get
        {
            return isGameOn.Value;
        }
    }

    public void RequestOwnTranslation(int playerId, int direction)
    {
        if (playerId == PLAYER1_ID)
        {
            firstArenaGridTranslation.Value = direction;
        }

        if (playerId == PLAYER2_ID)
        {
            secondArenaGridTranslation.Value = direction;
        }
    }

    public void RequestEnemyTranslation(int playerId, int direction)
    {
        if (playerId == PLAYER1_ID)
        {
            secondArenaSpawnTranslation.Value = direction;
        }

        if (playerId == PLAYER2_ID)
        {
            firstArenaSpawnTranslation.Value = direction;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<PlayerController>().SetPlayable();
        }

        //DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        if (IsServer)
        {
            if (this.isGameOn.Value == false)
            {
                SetArenas();
            }
            else
            {
                HandleAudioSynchedSpawn();
                HandleArenaTranslations();
                CheckEndgameConditions();
            }
        }
    }

    private void SetArenas()
    {
        PlayerController[] pcs = FindObjectsOfType<PlayerController>();

        Debug.Log(pcs.Length);
        if (NetworkManager.Singleton.ConnectedClients.Count < 2)
            throw new Exception("Unsuficient players");

        NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.GetComponent<PlayerController>().SetPlayerId(PLAYER1_ID);
        NetworkManager.Singleton.ConnectedClientsList[1].PlayerObject.GetComponent<PlayerController>().SetPlayerId(PLAYER2_ID);

        Transform firstArena = Instantiate(arenaPrefab, firstArenaPosition, Quaternion.identity);
        firstArena.GetComponent<NetworkObject>().Spawn(null, true);
        firstArena.name = "FirstArena";
        firstArenaManager = firstArena.Find("ArenaManager").GetComponent<ArenaManager>();
        firstArenaManager.isStandardOrientation = false;

        Transform secondArena = Instantiate(arenaPrefab, secondArenaPosition, Quaternion.identity);
        secondArena.GetComponent<NetworkObject>().Spawn(null, true);
        secondArena.name = "SecondArena";
        secondArenaManager = secondArena.Find("ArenaManager").GetComponent<ArenaManager>();
        secondArenaManager.isStandardOrientation = true;

        this.currentArenaManager = firstArenaManager;
        this.isGameOn.Value = true;
    }

    private void HandleAudioSynchedSpawn()
    {
        if (AudioManager.Instance.Play())
        {
            float momentHighPointCycle = AudioManager.Instance.GetMomentHighPointCycle();
            currentArenaManager.ScheduleTetrominoFall(momentHighPointCycle);

            currentArenaManager = (currentArenaManager == firstArenaManager) ? secondArenaManager : firstArenaManager;
        }
    }

    private void HandleArenaTranslations()
    {
        if (firstArenaGridTranslation.Value != 0)
        {
            firstArenaManager.TranslateGrid(firstArenaGridTranslation.Value);
            firstArenaGridTranslation.Value = 0;
        }

        if (secondArenaGridTranslation.Value != 0)
        {
            secondArenaManager.TranslateGrid(secondArenaGridTranslation.Value);
            secondArenaGridTranslation.Value = 0;
        }

        if (firstArenaSpawnTranslation.Value != 0)
        {
            firstArenaManager.TranslateSpawnPosition(firstArenaSpawnTranslation.Value);
            firstArenaSpawnTranslation.Value = 0;
        }

        if (secondArenaSpawnTranslation.Value != 0)
        {
            secondArenaManager.TranslateSpawnPosition(secondArenaSpawnTranslation.Value);
            secondArenaSpawnTranslation.Value = 0;
        }
    }

    private void CheckEndgameConditions()
    {
        bool firstPlayerLost = !firstArenaManager.IsPossibleToSpawn();
        bool secondPlayerLost = !secondArenaManager.IsPossibleToSpawn();

        if (firstPlayerLost || secondPlayerLost)
        {
            NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.GetComponent<PlayerController>().SetGameOver(!firstPlayerLost);
            NetworkManager.Singleton.ConnectedClientsList[1].PlayerObject.GetComponent<PlayerController>().SetGameOver(!secondPlayerLost);
     
            NetworkSceneManager.SwitchScene("GameOver");
        }
    }
}