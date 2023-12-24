using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid
{
    private int gridWidth, gridHeight;
    private float screenWidth, screenHeight;
    public NodeGrid(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
        screenWidth = Screen.safeArea.width;
        screenHeight = Screen.safeArea.height;
    }

    /// <summary>
    /// Takes in a grid x and y coordinate and gives the world coords. [0, xMax), [0, yMax)
    /// </summary>
    /// <param name="gridX">x position between 0 (inclusive) and gridWidth (exclusive)</param>
    /// <param name="gridY"></param>
    /// <returns>Vector2 containing the world space x and y coords</returns>
    public Vector2 getGridPointScreenSpace(int gridX, int gridY)
    {
        float x = screenWidth / (gridWidth + 1) * (gridX + 1);
        float y = screenHeight / (gridHeight + 1) * (gridY + 1);
        return new Vector2(x, y);
    }

    public Vector3 getGridPointWorldSpace(Camera cam, int gridX, int gridY)
    {
        Vector3 result = cam.ScreenToWorldPoint(getGridPointScreenSpace(gridX, gridY));
        result.z = 0;
        return result;
    }

    public Vector3 getGridPointWorldSpace(int gridX, int gridY)
    {
        Camera cam = Camera.main;
        Vector3 result = cam.ScreenToWorldPoint(getGridPointScreenSpace(gridX, gridY));
        result.z = 0;
        return result;
    }

    public void DrawDebugGrid()
    {
        // Gizmos.color = Color.yellow;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                // Gizmos.DrawSphere(getGridPointPos(i, j), 0.05f);
                if (i > 0) Debug.DrawLine(getGridPointWorldSpace(i - 1, j), getGridPointWorldSpace(i, j), Color.green, float.PositiveInfinity, false);
                if (i < gridWidth - 1) Debug.DrawLine(getGridPointWorldSpace(i, j), getGridPointWorldSpace(i + 1, j), Color.green, float.PositiveInfinity, false);
                if (j > 0) Debug.DrawLine(getGridPointWorldSpace(i, j - 1), getGridPointWorldSpace(i, j), Color.green, float.PositiveInfinity, false);
                if (j < gridHeight - 1) Debug.DrawLine(getGridPointWorldSpace(i, j), getGridPointWorldSpace(i, j + 1), Color.green, float.PositiveInfinity, false);
            }
        }
    }
}
