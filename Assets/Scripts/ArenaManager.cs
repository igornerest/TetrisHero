using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public static int height = 15;
    public static int width = 10;

    public GameObject[] Tetrominoes;
    public Vector3 spawnPosition;
    public Material plaaformMaterial;

    public float fallTime = 0.5f;

    public bool isStandardOrientation;

    private Transform[,] grid = new Transform[width, height];
    private HashSet<Transform> fallingPieces = new HashSet<Transform>();

    private float previousTime = 0;
    private float nextTetrominoTime = 0;
    private Transform nextTetromino;
    private bool hasTetrominoFallScheduled = false;

    private bool uncapableOfSpawn = false;

    private void Update()
    {
        if (hasTetrominoFallScheduled && Time.time - previousTime > nextTetrominoTime)
        {
            hasTetrominoFallScheduled = false;
        }
    }

    private void LateUpdate()
    {
        CheckLines();
    }

    public void ScheduleTetrominoFall(float nextTetrominoTime) {
        if (nextTetromino)
        {
            nextTetromino.GetComponent<TetrisBlock>().SetReady();
            fallingPieces.Add(nextTetromino);

            hasTetrominoFallScheduled = true;
            previousTime = Time.time;
        }

        nextTetromino = CreateNewTetromino().transform;
        this.nextTetrominoTime = nextTetrominoTime;
    }

    public bool DropTetromino(Transform tetromino)
    {
        foreach (Transform block in tetromino)
        {
            Vector3 blockLocalPosition = tetromino.GetComponent<TetrisBlock>().GetBlockLocalPosition(block);
            int roundedX = Mathf.RoundToInt(blockLocalPosition.x);
            int roundedY = Mathf.RoundToInt(blockLocalPosition.z) - 1;
 
            if (roundedY < 0 || grid[roundedX, roundedY] != null)
            {
                AddToGrid(tetromino);
                fallingPieces.Remove(tetromino);
                return false;
            }
        }
        
        tetromino.position += Vector3.back;
        return true;
    }

    public void TranslateSpawnPosition(int direction)
    {
        int newPosition = Mathf.RoundToInt(spawnPosition.x) + direction;

        // Avoid borders so tetrominoes can be rotated
        if (newPosition <= 0)
        {
            newPosition = width - 2;
        }
        else if (newPosition >= width - 1)
        {
            newPosition = 1;
        }

        spawnPosition.x = newPosition;
        if (nextTetromino) nextTetromino.position = transform.position + spawnPosition;
    }

    public void TranslateGrid(int direction)
    {
        ShiftFallingConflictingTetrominoes(direction); 

        for (int row = 0; row < height; row++)
        {
            int col = direction > 0 ? 0 : width - 1;
            int lastCol = direction > 0 ? width - 1 : 0;
            Transform lastTransform = grid[lastCol, row];
            
            while (col >= 0 && col < width)
            {
                Transform tempTransf = grid[col, row];
                grid[col, row] = lastTransform;
                lastTransform = tempTransf;

                if (grid[col, row] != null)
                {
                    grid[col, row].transform.position = transform.position + new Vector3(col, 0, row);
                }

                col += direction;
            }
        }
    }

    private void ShiftFallingConflictingTetrominoes(int direction)
    {
        foreach (Transform tetromino in fallingPieces)
        {
            foreach (Transform block in tetromino)
            {
                Vector3 blockLocalPosition = tetromino.GetComponent<TetrisBlock>().GetBlockLocalPosition(block);
                int roundedX = (Mathf.RoundToInt(blockLocalPosition.x) - direction + width) % width;
                int roundedY = Mathf.RoundToInt(blockLocalPosition.z);

                if (grid[roundedX, roundedY] != null)
                {
                    tetromino.GetComponent<TetrisBlock>().ShiftTetromino(direction, width);
                    break; 
                }
            }

        }
    }

    private GameObject CreateNewTetromino()
    {
        int randomIndex = Random.Range(0, Tetrominoes.Length);
        Vector3 realSpawnPosition = isStandardOrientation ? spawnPosition : -spawnPosition;
        GameObject tetromino = Instantiate(Tetrominoes[randomIndex], transform.position + realSpawnPosition, this.transform.rotation, transform);
        
        tetromino.GetComponent<TetrisBlock>().arenaManager = this;

        return tetromino;
    }

    private void AddToGrid(Transform tetromino)
    {
        foreach (Transform block in tetromino)
        {
            Vector3 blockLocalPosition = tetromino.GetComponent<TetrisBlock>().GetBlockLocalPosition(block);
            int roundedX = Mathf.RoundToInt(blockLocalPosition.x);
            int roundedY = Mathf.RoundToInt(blockLocalPosition.z);

            if (grid[roundedX, roundedY] == null)
            {
                grid[roundedX, roundedY] = block;
            }
            else
            {
                uncapableOfSpawn = true;
            }
            block.GetComponent<MeshRenderer>().material = plaaformMaterial;
        }
    }

    private void CheckLines()
    {
        int offset = 0;

        for (int row = 0; row < height; row++)
        {
            if (HasLine(row))
            {
                DeleteRow(row);
                offset++;
            }
            else if (offset > 0)
            {
                FixRowPosition(row, offset);
            }
        }
    }

    private bool HasLine(int row)
    {
        for (int col = 0; col < width; col++)
        {
            if (grid[col, row] == null)
            {
                return false;
            }
        }

        return true;
    }

    private void DeleteRow(int row)
    {
        for (int col = 0; col < width; col++)
        {
            Transform transform = grid[col, row].gameObject.transform;
            Transform parentTransform = transform.parent;

            Destroy(parentTransform.childCount <= 1 ? parentTransform.gameObject : transform.gameObject);

            grid[col, row] = null;
        }
    }

    private void FixRowPosition(int row, int offset)
    {
        for (int col = 0; col < width; col++)
        {
            if (grid[col, row] != null)
            {
                grid[col, row - offset] = grid[col, row];
                grid[col, row - offset].transform.localPosition += Vector3.back * offset;
                grid[col, row] = null;
            }
        }
    }

    public bool IsPossibleToSpawn()
    {
        return !uncapableOfSpawn;
    }
}
