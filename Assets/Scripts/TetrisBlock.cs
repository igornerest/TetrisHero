using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 rotationPoint;
    public ArenaManager arenaManager;

    private float previousTime;
    private bool isReadyToFall = false;

    private void Awake()
    {
        ChangeTranparency(0.5f);
    }

    private void Update()
    {
        if (isReadyToFall && Time.time - previousTime > arenaManager.fallTime)
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
            if(xPosition < 0)
            {
                xPosition = maxWidth + xPosition;
            }
            int yPosition = Mathf.RoundToInt(blockLocalPosition.z);
            block.position = transform.parent.position + new Vector3(xPosition, 0, yPosition);
        }
    }

    public Vector3 GetBlockLocalPosition(Transform block)
    {
        return transform.localPosition + block.localPosition;
    }

    public void SetReady()
    {
        isReadyToFall = true;
        previousTime = Time.time;
        ChangeTranparency(1f);
    }

    private void ChangeTranparency(float transp)
    {
        foreach (Transform block in transform)
        {
            Renderer meshRenderer = block.GetComponent<Renderer>();
            Color textureColor = meshRenderer.material.color;
            textureColor.a = transp;
            meshRenderer.material.color = textureColor;
        }
    }
}
