using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform arenaPrefab;

    private Transform firstArena;
    private Transform secondArena;
    private PlayerController firstPlayer;
    private PlayerController secondPlayer;

    private Transform currentArena;
    private bool isGameSet = false;

    private static GameManager instance = null;

    public GameManager instace {get; private set;}

    private int _jogadoresEmJogo = 0;

    [SerializeField] private string _localizacaoPrefab;

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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
            DestroyImmediate(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        SetArenas();
    }


    //private void Start() {
    //    photonView.RPC("AdicionaJogador", RpcTarget.AllBuffered);
    //}

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

        if (isGameSet)
        {
            if (AudioManager.Instance.Play())
            {
                float momentHighPointCycle = AudioManager.Instance.GetMomentHighPointCycle();
                currentArena.Find("ArenaManager").GetComponent<ArenaManager>().ScheduleTetrominoFall(momentHighPointCycle);

                currentArena = currentArena == firstArena ? secondArena : firstArena;
            }
        }
        
        if (!firstPlayer.playerArenaManager.IsPossibleToSpawn() && SceneManager.GetActiveScene().name != "GameOver") {            
            isGameSet = false;
            SceneManager.LoadScene("GameOver");
        }
        if (!secondPlayer.playerArenaManager.IsPossibleToSpawn() && SceneManager.GetActiveScene().name != "GameOver") {
            isGameSet = false;
            SceneManager.LoadScene("GameOver");            
        }

    }

    public void SetArenas()
    {
        this.firstArena = Instantiate(arenaPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        this.firstArena.name = "FirstArena";

        this.secondArena = Instantiate(arenaPrefab, new Vector3(20, 0, 0), Quaternion.identity);
        this.secondArena.name = "SecondArena";

        this.firstPlayer = new GameObject("FirstPlayer").AddComponent<PlayerController>();
        this.firstPlayer.playerArenaManager = firstArena.Find("ArenaManager").GetComponent<ArenaManager>();
        this.firstPlayer.enemyArenaManager = secondArena.Find("ArenaManager").GetComponent<ArenaManager>();
        this.firstPlayer.playerId = 1;

        this.secondPlayer = new GameObject("SecondPlayer").AddComponent<PlayerController>();
        this.secondPlayer.playerArenaManager = secondArena.Find("ArenaManager").GetComponent<ArenaManager>();
        this.secondPlayer.enemyArenaManager = firstArena.Find("ArenaManager").GetComponent<ArenaManager>();
        this.secondPlayer.playerId = 2;

        this.currentArena = firstArena;
        this.isGameSet = true;
    }

    //[PunRPC]
    //private void AdicionaJogador() {
    //    _jogadoresEmJogo++;
    //    if (_jogadoresEmJogo == PhotonNetwork.PlayerList.Length) {
    //        CriaJogador();
    //    }
    //}

    //private void CriaJogador() {
    //    var jogadorObj = PhotonNetwork.Instantiate(_localizacaoPrefab, new Vector3(20, 0, 0), Quaternion.identity);
    //    var jogador = jogadorObj.GetComponent<ArenaManager>();
    //}
}
