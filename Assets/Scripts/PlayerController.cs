using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ArenaManager playerArenaManager;
    public ArenaManager enemyArenaManager;
    public int playerId;

    private void Update()
    {
        KeyCode leftMovvement = playerId == 1 ? KeyCode.LeftArrow : KeyCode.A;
        KeyCode rightMovement = playerId == 1 ? KeyCode.RightArrow : KeyCode.D;

        if (Input.GetKeyDown(leftMovvement))
        {
            playerArenaManager.TranslateGrid(-1);
        }
        if (Input.GetKeyDown(rightMovement))
        {
            playerArenaManager.TranslateGrid(+1);
        }

        // TODO: Atack movement
    }

}
