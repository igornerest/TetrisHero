using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 rotationPoint;
    public ArenaManager arenaManager;
    
    private float previousTime = 0;

    private void Update()
    {
        if (Time.time - previousTime > arenaManager.fallTime)
        {
            if (arenaManager.DropTetromino(transform) == false)
                this.enabled = false;

            previousTime = Time.time;
        }
    }
}
