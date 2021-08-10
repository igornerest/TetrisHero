using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class PlayerController : NetworkBehaviour
{
    public Vector3 camera1Position = new Vector3(4.5f, 10f, 28f);
    public Vector3 camera2Position = new Vector3(4.5f, 10f, -28f);
    public Vector3 centerPosition = new Vector3(4.5f, 0, 0);

    private static NetworkVariableSettings fullWritePermission = new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone };

    private NetworkVariableInt playerId = new NetworkVariableInt(fullWritePermission, 0);
    private NetworkVariableBool isPlayable = new NetworkVariableBool(fullWritePermission, false);

    private bool setCamera = false;

    private void Update()
    {
        if (!isPlayable.Value)
            return;

        if (!GameManager.Instance.IsGameOn)
            return;

        if (IsOwner)
        {
            if (!setCamera)
            {
                SetCamera();
            }

            MoveGrid();
        }
    }

    // SetPlayable() and SetPlayerId are methods called from the server side
    public void SetPlayable()
    {
        isPlayable.Value = true;
    }

    public void SetPlayerId(int playerId)
    {
        this.playerId.Value = playerId;
    }

    private void SetCamera()
    {
        Transform mainCamera = GameObject.Find("Main Camera").transform;
        mainCamera.position = playerId.Value == 1 ? camera1Position : camera2Position;
        mainCamera.LookAt(centerPosition);
    }

    private void MoveGrid()
    {
        KeyCode leftMovementKey = playerId.Value == 1 ? KeyCode.RightArrow : KeyCode.LeftArrow;
        KeyCode rightMovementKey = playerId.Value == 1 ? KeyCode.LeftArrow : KeyCode.RightArrow;

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
