using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class GameManager : NetworkBehaviour
{
    public Transform arenaPrefab;

    public Transform firstArena;
    public Transform secondArena;
    private PlayerController firstPlayer;
    private PlayerController secondPlayer;

    private Transform currentArena;

    private static GameManager instance = null;

    private static NetworkVariableSettings fullWritePermission = new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone };
    private NetworkVariableInt firstPlayerTranslate = new NetworkVariableInt(fullWritePermission, 0);
    private NetworkVariableInt secondPlayerTranslate = new NetworkVariableInt(fullWritePermission, 0);
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

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        this.isGameOn.Value = false;
    }

    public NetworkVariableInt AddPlayer(Transform newPlayer)
    {
        if (firstPlayer == null)
        {
            firstPlayer = newPlayer.GetComponent<PlayerController>();
            firstPlayer.name = "FirstPlayer";
            firstPlayer.playerId = 1;
            return firstPlayerTranslate;
        }
        else if (secondPlayer == null)
        {
            secondPlayer = newPlayer.GetComponent<PlayerController>();
            secondPlayer.name = "SecondPlayer";
            secondPlayer.playerId = 2;
            return secondPlayerTranslate;
        }
        else
        {
            return null;
        }

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
            if (firstPlayer && secondPlayer)
            {
                SetArenas();
            }
        }
        else
        {
            if (AudioManager.Instance.Play())
            {
                float momentHighPointCycle = AudioManager.Instance.GetMomentHighPointCycle();

                currentArena.Find("ArenaManager").GetComponent<ArenaManager>().momentHighPointCycle = momentHighPointCycle;
                currentArena.Find("ArenaManager").GetComponent<ArenaManager>().hasGameManagerScheduled = true;

                currentArena = currentArena == firstArena ? secondArena : firstArena;
            }

            if (firstPlayerTranslate.Value != 0)
            {
                firstArena.Find("ArenaManager").GetComponent<ArenaManager>().TranslateGrid(firstPlayerTranslate.Value);
                firstPlayerTranslate.Value = 0;
            }

            if (secondPlayerTranslate.Value != 0)
            {
                secondArena.Find("ArenaManager").GetComponent<ArenaManager>().TranslateGrid(secondPlayerTranslate.Value);
                secondPlayerTranslate.Value = 0;
            }
        }
    }

    public void SetArenas()
    {
        Debug.Log("Setting Arenas");
        this.firstArena = Instantiate(arenaPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.firstArena.GetComponent<NetworkObject>().Spawn();
        this.firstArena.name = "FirstArena";

        this.secondArena = Instantiate(arenaPrefab, new Vector3(20, 0, 0), Quaternion.identity);
        this.secondArena.GetComponent<NetworkObject>().Spawn();
        this.secondArena.name = "SecondArena";

        this.currentArena = firstArena;
        this.isGameOn.Value = true; 
    }

    public void Translate(int id, int direction)
    {
        if (id == firstPlayer.playerId)
        {
            firstPlayerTranslate.Value = direction;
        }

        if (id == secondPlayer.playerId)
        {
            secondPlayerTranslate.Value = direction;
        }
    }
}
