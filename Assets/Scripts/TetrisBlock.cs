using UnityEngine;
using MLAPI;

public class TetrisBlock : NetworkBehaviour
{
    public Vector3 rotationPoint;
    public ArenaManager arenaManager;

    private float previousTime = 0;

    private void Update()
    {
        if (!IsServer)
            return;

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
            Vector3 blockLocalPosition = GetBlockLocalPosition(block);

            int xPosition = (Mathf.RoundToInt(blockLocalPosition.x) + direction) % maxWidth;
            int yPosition = Mathf.RoundToInt(blockLocalPosition.z);
            block.position = transform.parent.position + new Vector3(xPosition, 0, yPosition);
        }
    }

    public Vector3 GetBlockLocalPosition(Transform block)
    {
        return transform.localPosition + block.localPosition;
    }
}
