using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class PlayerController : NetworkBehaviour
{
    public int playerId;

    private void Awake()
    {
        GameManager.Instance.AddPlayer(transform);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameOn)
            return;

        if (IsOwner) {
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
            GameManager.Instance.Translate(playerId, -1);
        }
        if (Input.GetKeyDown(rightMovement))
        {

            GameManager.Instance.Translate(playerId, -1);
        }
    }

}
