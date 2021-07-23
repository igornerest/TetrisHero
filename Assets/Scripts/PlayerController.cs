using UnityEngine;
using MLAPI;

public class PlayerController : NetworkBehaviour
{
    public ArenaManager playerArenaManager;
    public ArenaManager enemyArenaManager;
    public int playerId;

    private void Awake()
    {
        GameManager.Instance.AddPlayer(transform);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameOn)
            return;

        if (IsLocalPlayer) {
            MoveGrid();
        }

        // TODO: Atack movement
    }

    private void MoveGrid()
    {
        KeyCode leftMovvement = KeyCode.LeftArrow;
        KeyCode rightMovement = KeyCode.RightArrow;

        if (Input.GetKeyDown(leftMovvement))
        {
            playerArenaManager.TranslateGrid(-1);
        }
        if (Input.GetKeyDown(rightMovement))
        {
            playerArenaManager.TranslateGrid(+1);
        }
    }

}
