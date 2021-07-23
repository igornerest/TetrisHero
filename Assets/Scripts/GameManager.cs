using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class GameManager : MonoBehaviour
{
    public Transform arenaPrefab;

    private Transform firstArena;
    private Transform secondArena;
    private PlayerController firstPlayer;
    private PlayerController secondPlayer;

    private Transform currentArena;

    private static GameManager instance = null;

    public NetworkVariableUInt count = new NetworkVariableUInt(0);

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

    public bool IsGameOn { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        IsGameOn = false;
    }

    public void AddPlayer(Transform newPlayer)
    {
        if (firstPlayer == null)
        {
            firstPlayer = newPlayer.GetComponent<PlayerController>();
            firstPlayer.name = "FirstPlayer";
            firstPlayer.playerId = 1;
        }
        else if (secondPlayer == null)
        {
            secondPlayer = newPlayer.GetComponent<PlayerController>();
            secondPlayer.name = "SecondPlayer";
            secondPlayer.playerId = 2;
        }

        if (firstPlayer && secondPlayer)
        {
            SetArenas();
        }
    }

    [ServerRpc]
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

        if (IsGameOn)
        {
            if (AudioManager.Instance.Play())
            {
                float momentHighPointCycle = AudioManager.Instance.GetMomentHighPointCycle();
                currentArena.Find("ArenaManager").GetComponent<ArenaManager>().ScheduleTetrominoFall(momentHighPointCycle);

                currentArena = currentArena == firstArena ? secondArena : firstArena;
            }
        }
    }

    public void SetArenas()
    {
        this.firstArena = Instantiate(arenaPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.firstArena.name = "FirstArena";

        this.secondArena = Instantiate(arenaPrefab, new Vector3(20, 0, 0), Quaternion.identity);
        this.secondArena.name = "SecondArena";

        this.firstPlayer.playerArenaManager = firstArena.Find("ArenaManager").GetComponent<ArenaManager>();
        this.firstPlayer.enemyArenaManager = secondArena.Find("ArenaManager").GetComponent<ArenaManager>();
    
        this.secondPlayer.playerArenaManager = secondArena.Find("ArenaManager").GetComponent<ArenaManager>();
        this.secondPlayer.enemyArenaManager = firstArena.Find("ArenaManager").GetComponent<ArenaManager>();

        this.currentArena = firstArena;
        this.IsGameOn = true; 
    }
}
