using UnityEngine;

public class TretisBlock : MonoBehaviour
{

    public Vector3 rotationPoint;

    public float fallTime = 0.5f;
    public static int height = 15;
    public static int width = 10;

    private float previousTime;

    private void Update()
    {
        Vector3 oldPosition = transform.position;
        Quaternion oldRotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 1, 0),  90);
        }

        if (Time.time - previousTime > fallTime)
        {
            transform.position += Vector3.back;
            previousTime = Time.time;
        }

        if (!isValidMove())
        {
            transform.SetPositionAndRotation(oldPosition, oldRotation);
        }
    }

    private bool isValidMove()
    {
        foreach (Transform block in transform)
        {
            int roundedX = Mathf.RoundToInt(block.transform.position.x);
            int roundedY = Mathf.RoundToInt(block.transform.position.z);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }
        }

        return true;
    }
}
