﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform arenaPrefab;

    private Transform firstArena;
    private Transform secondArena;

    private PlayerController firstPlayer;
    private PlayerController secondPlayer;

    private static GameManager instance = null;

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
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        SetArenas();
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
    }
}