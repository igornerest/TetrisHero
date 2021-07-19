using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuControler : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
  
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
