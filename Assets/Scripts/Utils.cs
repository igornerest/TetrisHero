using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Utils
{
    public static void TranslateSmoothlyX(Transform transformTarget, Vector3 positionSource, int col, int direction, int width)
    {
        float positionX = positionSource.x + col;
        bool teletranportOnRight = Mathf.CeilToInt(positionX) == 0 && direction == 1;
        bool teletranportOnLeft = Mathf.CeilToInt(positionX) == width - 1 && direction == -1;
        if (teletranportOnLeft || teletranportOnRight)
        {
            transformTarget.position = new Vector3(positionX, 0, transformTarget.position.z);
        }
        else
        {
            transformTarget.DOMoveX(positionX, 0.05f);
        }
    }

    public static void TranslateSmoothlyZ(Transform transformTarget, Vector3 positionSource, int row)
    {
        float positionZ = positionSource.z + row;
        transformTarget.DOMoveZ(positionZ, 0.2f);
    }

}
