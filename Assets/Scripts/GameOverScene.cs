using TMPro;
using UnityEngine;
using MLAPI.SceneManagement;
using MLAPI;

public class GameOverScene : NetworkBehaviour
{
    [SerializeField] private TMP_Text resultMessage;
    [SerializeField] private TMP_Text returnMessage;

    private bool hasRequestedSceneSwitch = false;

    public void SetWinText()
    {
        resultMessage.text = "You win";
    }

    public void SetLoseText()
    {
        resultMessage.text = "You lose";
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad < 5)
        {
            int countdown = 5 - Mathf.FloorToInt(Time.timeSinceLevelLoad);
            returnMessage.text = "Returning to Lobby in " + countdown + " ...";
        }
        else
        {
            if (IsServer && !hasRequestedSceneSwitch)
            {
                NetworkSceneManager.SwitchScene("LobbyScene");
                hasRequestedSceneSwitch = true;
            }
        }
    }
}