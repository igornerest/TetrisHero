using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    [SerializeField] private Vector3 rotationPoint;

    public ArenaManager arenaManager;

    private bool isReadyToFall = false;

    private static float tetrominoFallTimeScale = 2f;

    private void Awake()
    {
        ChangeTranparency(0.5f);
    }

    private void Update()
    {
        if (isReadyToFall)
        {
            if (arenaManager.DropTetromino(transform, Time.smoothDeltaTime * tetrominoFallTimeScale) == false)
            {
                //this.enabled = false;
            }
        }
    }

    public void ShiftTetromino(int direction, int maxWidth)
    {
        foreach (Transform block in transform)
        {
            Vector3 blockLocalPosition = GetBlockLocalPosition(block);

            float yPosition = blockLocalPosition.z;
            float xPosition = (Mathf.RoundToInt(blockLocalPosition.x) + direction) % maxWidth;
            if (xPosition < 0)
            {
                xPosition = maxWidth + xPosition;
            }
            
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
