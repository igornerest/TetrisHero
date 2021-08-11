using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class PlayerController : NetworkBehaviour
{
    public Vector3 camera1Position = new Vector3(4.5f, 10f, 28f);
    public Vector3 camera2Position = new Vector3(4.5f, 10f, -28f);
    public Vector3 centerPosition = new Vector3(4.5f, 0, 0);

    private const string IS_PLAYABLE = "IS_PLAYABLE";
    private const string IS_GAMEOVER = "IS_GAMEOVER";
    private const string NONE = "NONE";

    private static NetworkVariableSettings fullWritePermission = new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone };

    private NetworkVariableInt playerId = new NetworkVariableInt(fullWritePermission, 0);
    private NetworkVariableString playerState = new NetworkVariableString(fullWritePermission, NONE);
    private NetworkVariableBool hasWon = new NetworkVariableBool(fullWritePermission, false);

    private bool setCamera = false;

    private void Update()
    {
        if (!IsOwner)
            return;

        switch (playerState.Value)
        {
            case IS_PLAYABLE:
                if (!GameManager.Instance.IsGameOn)
                    return;

                if (!setCamera)
                {
                    SetCamera();
                }
                MoveGrid();
                
                break;

            case IS_GAMEOVER:
                UpdateResultText();
                playerState.Value = NONE;
                return;

        }
    }

    // SetPlayable(), SetGameOver() and SetPlayerId are methods called from the server side
    public void SetPlayable()
    {
        playerState.Value = IS_PLAYABLE;
    }

    public void SetGameOver(bool hasWon)
    {
        playerState.Value = IS_GAMEOVER;
        this.hasWon.Value = hasWon;
    }

    public void SetPlayerId(int playerId)
    {
        this.playerId.Value = playerId;
    }

    private void UpdateResultText()
    {
        GameOverScene gmScene = GameObject.Find("GameOverScene").GetComponent<GameOverScene>();
        
        if (hasWon.Value)
        {
            gmScene.SetWinText();
        }
        else
        {
            gmScene.SetLoseText();
        }
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
