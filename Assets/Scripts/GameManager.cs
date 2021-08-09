using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using System;

public class GameManager : NetworkBehaviour
{
    public Transform arenaPrefab;

    private Transform firstArena;
    private Transform secondArena;
    private PlayerController firstPlayer;
    private PlayerController secondPlayer;

    private Transform currentArena;

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

    public void RequestTranslation(int playerId, int direction)
    {
        if (playerId == firstPlayer.playerId)
        {
            firstArenaGridTranslation.Value = direction;
        }

        if (playerId == secondPlayer.playerId)
        {
            secondArenaGridTranslation.Value = direction;
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
        DontDestroyOnLoad(this);
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
            //HandleAudioSynchedSpawn();
            //HandleArenaTranslations();
        }
    }

    private void SetArenas()
    {
        PlayerController[] pcs = FindObjectsOfType<PlayerController>();

        Debug.Log(pcs.Length);
        if (pcs.Length < 2)
            throw new Exception("Unsuficient players");

        pcs[0].playerId = 1;
        pcs[0].name = "FirstPlayer";

        pcs[1].playerId = 2;
        pcs[1].name = "SecondPlayer";

        this.firstArena = Instantiate(arenaPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.firstArena.GetComponent<NetworkObject>().Spawn();
        this.firstArena.name = "FirstArena";
        this.firstArena.Find("ArenaManager").GetComponent<ArenaManager>().isStandardOrientation = true;

        this.secondArena = Instantiate(arenaPrefab, new Vector3(0, 0, 20 ), Quaternion.identity);
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
        firstArena.Find("ArenaManager").GetComponent<ArenaManager>().TranslateGrid(firstArenaGridTranslation.Value);
        firstArenaGridTranslation.Value = 0;

        secondArena.Find("ArenaManager").GetComponent<ArenaManager>().TranslateGrid(secondArenaGridTranslation.Value);
        secondArenaGridTranslation.Value = 0;
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
