using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class PlayerController : NetworkBehaviour
{
    private static NetworkVariableSettings fullWritePermission = new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone };

    private NetworkVariableInt playerId = new NetworkVariableInt(fullWritePermission, 0);
    private NetworkVariableBool isPlayable = new NetworkVariableBool(fullWritePermission, false);

    private void Update()
    {
        if (!isPlayable.Value)
            return;

        if (!GameManager.Instance.IsGameOn)
            return;

        if (IsOwner)
        {
            Debug.Log("IsOwner");
            MoveGrid();
        }
    }

    public void SetPlayable()
    {
        isPlayable.Value = true;
    }

    public void SetPlayerId(int playerId)
    {
        this.playerId.Value = playerId;
    }

    private void MoveGrid()
    {
        KeyCode leftMovementKey = KeyCode.LeftArrow;
        KeyCode rightMovementKey = KeyCode.RightArrow;

        if (Input.GetKeyDown(leftMovementKey))
        {
            Debug.Log("Moving Left");
            GameManager.Instance.RequestOwnTranslation(playerId.Value, -1);
            GameManager.Instance.RequestEnemyTranslation(playerId.Value, -1);
        }
        if (Input.GetKeyDown(rightMovementKey))
        {
            Debug.Log("Moving Right");
            GameManager.Instance.RequestOwnTranslation(playerId.Value, +1);
            GameManager.Instance.RequestEnemyTranslation(playerId.Value, +1);
        }
    }

}
