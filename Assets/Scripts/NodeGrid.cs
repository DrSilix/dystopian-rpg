using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid
{
    private static Vector3 worldOrigin = Vector3.zero;
    private static float zValue = 0f;

    private int gridWidth, gridHeight;
    private float screenWidth, screenHeight;
    private float distanceBetweenPoints;
    private float worldWidth, worldHeight;

    public NodeGrid(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
        screenWidth = Screen.safeArea.width;
        screenHeight = Screen.safeArea.height;
        distanceBetweenPoints = 0.5f;
    }

    public NodeGrid(int width, int height, float worldDistanceBetweenPoints)
    {
        gridWidth = width;
        gridHeight = height;
        screenWidth = Screen.safeArea.width;
        screenHeight = Screen.safeArea.height;
        distanceBetweenPoints = worldDistanceBetweenPoints;
        worldWidth = distanceBetweenPoints * width;
        worldHeight = distanceBetweenPoints * height;
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

    /// <summary>
    /// Converts an abstract grid coord to a world space coordinate bounded by the screen
    /// </summary>
    /// <param name="cam">Camera to bound the grid</param>
    /// <param name="gridX">Abstract grid x value [0, xMax)</param>
    /// <param name="gridY">Abstract grid y value [0, yMax)</param>
    /// <returns>World space Vector3</returns>
    public Vector3 getGridPointScreenToWorldSpace(Camera cam, int gridX, int gridY)
    {
        Vector3 result = cam.ScreenToWorldPoint(getGridPointScreenSpace(gridX, gridY));
        result.z = 0;
        return result;
    }

    /// <summary>
    /// Converts an abstract grid coord to a world space coordinate bounded by the screen of Camera.main
    /// </summary>
    /// <param name="gridX">Abstract grid x value [0, xMax)</param>
    /// <param name="gridY">Abstract grid y value [0, yMax)</param>
    /// <returns>World space Vector3</returns>
    public Vector3 getGridPointScreenToWorldSpace(int gridX, int gridY)
    {
        Camera cam = Camera.main;
        Vector3 result = cam.ScreenToWorldPoint(getGridPointScreenSpace(gridX, gridY));
        result.z = 0;
        return result;
    }

    /// <summary>
    /// Converts an abstract grid coord to a world space coordinate using the worldDistanceBetweenPoints
    /// provided at construction. Origin is zero and points are int the positive quadrant
    /// </summary>
    /// <param name="gridX">Abstract grid x value [0, xMax)</param>
    /// <param name="gridY">Abstract grid y value [0, yMax)</param>
    /// <returns>World space Vector3</returns>
    public Vector3 getGridPointToWorldSpace(int gridX, int gridY)
    {
        float xCoord = (gridX * distanceBetweenPoints) + worldOrigin.x;
        float yCoord = (gridY * distanceBetweenPoints) + worldOrigin.y;
        return new Vector3(xCoord, yCoord, zValue);
    }

    /// <summary>
    /// Draws the lines of the grid to Debug
    /// </summary>
    public void DrawDebugGrid()
    {
        // Gizmos.color = Color.yellow;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                // Gizmos.DrawSphere(getGridPointPos(i, j), 0.05f);
                if (i > 0) Debug.DrawLine(getGridPointScreenToWorldSpace(i - 1, j), getGridPointScreenToWorldSpace(i, j), Color.green, float.PositiveInfinity, false);
                if (i < gridWidth - 1) Debug.DrawLine(getGridPointScreenToWorldSpace(i, j), getGridPointScreenToWorldSpace(i + 1, j), Color.green, float.PositiveInfinity, false);
                if (j > 0) Debug.DrawLine(getGridPointScreenToWorldSpace(i, j - 1), getGridPointScreenToWorldSpace(i, j), Color.green, float.PositiveInfinity, false);
                if (j < gridHeight - 1) Debug.DrawLine(getGridPointScreenToWorldSpace(i, j), getGridPointScreenToWorldSpace(i, j + 1), Color.green, float.PositiveInfinity, false);
            }
        }
    }
}
