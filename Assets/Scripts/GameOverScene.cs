using TMPro;
using UnityEngine;

public class GameOverScene : MonoBehaviour
{
    public TMP_Text resultText;

    public void SetWinText()
    {
        resultText.text = "You win";
    }

    public void SetLoseText()
    {
        resultText.text = "You lost";
    }
}
