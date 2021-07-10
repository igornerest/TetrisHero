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

    public void ShiftTetromino(int direction, int maxWidth)
    {
        foreach (Transform block in transform)
        {
            int xPosition = (Mathf.RoundToInt(block.transform.position.x) + direction) % maxWidth;
            int yPosition = Mathf.RoundToInt(block.transform.position.z);
            block.position = new Vector3(xPosition, 0, yPosition);
        }
    }
}
