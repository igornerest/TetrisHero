using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using System;
using MLAPI.Connection;

public class GameManager : NetworkBehaviour
{
    public Transform arenaPrefab;

    private Transform firstArena;
    private Transform secondArena;
    private Transform currentArena;

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

        if (!IsServer)
            return;

        if (this.isGameOn.Value == false)
        {
            SetArenas();
        }
        else
        {
            HandleAudioSynchedSpawn();
            HandleArenaTranslations();
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

        this.firstArena = Instantiate(arenaPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.firstArena.GetComponent<NetworkObject>().Spawn();
        this.firstArena.name = "FirstArena";
        this.firstArena.Find("ArenaManager").GetComponent<ArenaManager>().isStandardOrientation = true;

        this.secondArena = Instantiate(arenaPrefab, new Vector3(0, 0, 20), Quaternion.identity);
        this.secondArena.GetComponent<NetworkObject>().Spawn();
        this.secondArena.name = "SecondArena";
        this.secondArena.Find("ArenaManager").GetComponent<ArenaManager>().isStandardOrientation = true;

        this.currentArena = firstArena;
        this.isGameOn.Value = true;
    }

    private void HandleAudioSynchedSpawn()
    {
        if (AudioManager.Instance.Play())
        {
            float momentHighPointCycle = AudioManager.Instance.GetMomentHighPointCycle();
            currentArena.Find("ArenaManager").GetComponent<ArenaManager>().ScheduleTetrominoFall(momentHighPointCycle);

            currentArena = currentArena == firstArena ? secondArena : firstArena;
        }
    }

    private void HandleArenaTranslations()
    {
        if (firstArenaGridTranslation.Value != 0)
        {
            firstArena.Find("ArenaManager").GetComponent<ArenaManager>().TranslateGrid(firstArenaGridTranslation.Value);
            firstArenaGridTranslation.Value = 0;
        }

        if (secondArenaGridTranslation.Value != 0)
        {
            secondArena.Find("ArenaManager").GetComponent<ArenaManager>().TranslateGrid(secondArenaGridTranslation.Value);
            secondArenaGridTranslation.Value = 0;
        }
    }

    private void CheckEndgameConditions()
    {
        /*
        if (!firstPlayer.playerArenaManager.IsPossibleToSpawn() && SceneManager.GetActiveScene().name != "GameOver")
        {
            isGameSet = false;
            SceneManager.LoadScene("GameOver");
        }
        if (!secondPlayer.playerArenaManager.IsPossibleToSpawn() && SceneManager.GetActiveScene().name != "GameOver")
        {
            isGameSet = false;
            SceneManager.LoadScene("GameOver");
        }*/
    }
}