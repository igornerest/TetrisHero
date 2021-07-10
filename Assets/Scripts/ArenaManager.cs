using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public static int height = 15;
    public static int width = 10;

    public GameObject[] Tetrominoes;
    public Vector3 spawnPosition;
    public Material plaaformMaterial;
    
    public float spawnTime = 5f;
    public float fallTime = 0.5f;

    private Transform[,] grid = new Transform[width, height];
    private HashSet<Transform> fallingPieces = new HashSet<Transform>();

    private float previousTime = 0;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TranslateGrid(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            TranslateGrid(+1);
        }

        if (Time.time - previousTime > spawnTime)
        {
            previousTime = Time.time;
            createNewTetromino();
        }
    }

    private void LateUpdate()
    {
        CheckLines();
    }

    public bool DropTetromino(Transform tetromino)
    {
        foreach (Transform block in tetromino)
        {
            int roundedX = Mathf.RoundToInt(block.transform.position.x);
            int roundedY = Mathf.RoundToInt(block.transform.position.z) - 1;
 
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

    private void TranslateGrid(int direction)
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
                    grid[col, row].transform.position = new Vector3(col, 0, row);
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
                int roundedX = (Mathf.RoundToInt(block.transform.position.x) - direction + width) % width;
                int roundedY = Mathf.RoundToInt(block.transform.position.z);

                if (grid[roundedX, roundedY] != null)
                {
                    tetromino.GetComponent<TetrisBlock>().ShiftTetromino(direction, width);
                    break; 
                }
            }

        }
    }

    private void createNewTetromino()
    {
        int randomIndex = Random.Range(0, Tetrominoes.Length);
        GameObject tetromino = Instantiate(Tetrominoes[4], spawnPosition, Quaternion.identity, transform);
        tetromino.GetComponent<TetrisBlock>().arenaManager = this;
        fallingPieces.Add(tetromino.transform);
    }

    private void AddToGrid(Transform tetromino)
    {
        foreach (Transform block in tetromino)
        {
            int roundedX = Mathf.RoundToInt(block.transform.position.x);
            int roundedY = Mathf.RoundToInt(block.transform.position.z);

            grid[roundedX, roundedY] = block;
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
            Transform blockTransform = grid[col, row].gameObject.transform;
            Destroy(blockTransform.childCount <= 1 ? blockTransform.parent.gameObject : blockTransform.gameObject);

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
                grid[col, row - offset].transform.position += Vector3.back * offset;
                grid[col, row] = null;
            }
        }
    }
}
