using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public class Node2
{
    // Represents the state of a node in the pathfinding algorithm.
    public enum State
    {
        None,   // Node has not been processed yet.
        Open,   // Node is in the open list (potential nodes to be evaluated).
        Closed  // Node has been evaluated and is in the closed list.
    }

    public int F = 0;   // Total estimated path length. F = G + H
    public int G = 0;   // Distance travelled from the start node to this node.
    public int H = 0;   // Estimated distance from this node to the target node.
    public int C = 1;   // Cost of traversing this node. Default is 1.
    public bool Wall = false;   // Indicates if this node is a wall (impassable).
    public Vector2Int Parent = new Vector2Int(-1, -1);   // Coordinates of the parent node in the path.
    public State state = State.None;    // Current state of the node.
}
public class AstarPathFind : MonoBehaviour
{
    // Grid dimensions and cell size
    public static int gridWidth = 5000;
    public static int gridHeight = 5000;
    public static float cellSize = 1.0f;

    // Corners of the grid
    public static Vector2Int maxCorner = new Vector2Int(gridWidth - 1, gridHeight - 1);
    public static Vector2Int minCorner = new Vector2Int(0, 0);

    // 2D array of nodes representing the grid
    public static Node2[,] Nodes;

    // Lists to store game objects tagged as rocks and plants
    List<GameObject> rocks = new List<GameObject>();
    List<GameObject> plants = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Nodes array
        Nodes = new Node2[gridHeight, gridWidth];
        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Nodes[gridHeight - z - 1, x] = new Node2();
                Nodes[gridHeight - z - 1, x].Wall = false;
            }
        }

        // Start the coroutine to find and process rocks and plants
        StartCoroutine(Wait5());
    }

    // Update is called once per frame
    void Update()
    {
        // Uncomment the following lines to draw grid lines for debugging
        // for (int y = 0; y <= gridHeight; y++)
        // {
        //     Debug.DrawLine(new Vector3(0, 0.1f, y), new Vector3(gridWidth, 0.1f, y));
        // }
        // for (int x = 0; x <= gridWidth; ++x)
        // {
        //     Debug.DrawLine(new Vector3(x, 0.1f, 0), new Vector3(x, 0.1f, gridHeight));
        // }
    }

    // Get the node at the specified position
    public static Node2 GetNode(Vector2Int pos)
    {
        return Nodes[pos.x, pos.y];
    }

    // Update the min and max corners based on the given coordinates
    static void Merge(int x, int y)
    {
        if (x < minCorner.x) minCorner.x = x;
        if (x > maxCorner.x) maxCorner.x = x;
        if (y < minCorner.y) minCorner.y = y;
        if (y > maxCorner.y) maxCorner.y = y;
    }

    // Find a path from start to end using the A* algorithm
    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        Vector2Int[] directions = new Vector2Int[] {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1)
        };

        // Expand the search area by one cell in each direction
        minCorner.x--;
        maxCorner.x++;
        minCorner.y--;
        maxCorner.y++;
        minCorner.x = Math.Clamp(minCorner.x, 0, gridWidth - 1);
        minCorner.y = Math.Clamp(minCorner.y, 0, gridHeight - 1);
        maxCorner.x = Math.Clamp(maxCorner.x, 0, gridWidth - 1);
        maxCorner.y = Math.Clamp(maxCorner.y, 0, gridHeight - 1);

        // Reset the state of nodes within the search area
        for (int y = minCorner.y; y <= maxCorner.y; ++y)
        {
            for (int x = minCorner.x; x < maxCorner.x; ++x)
            {
                Nodes[x, y].state = Node2.State.None;
                Nodes[x, y].Parent = new Vector2Int(-1, -1);
                Nodes[x, y].G = 0;
            }
        }

        // Reset the corners
        minCorner = new Vector2Int(gridWidth - 1, gridHeight - 1);
        maxCorner = new Vector2Int(0, 0);

        // A* algorithm implementation
        List<Vector2Int> openList = new List<Vector2Int>();
        Vector2Int currentCoord;

        openList.Add(start);
        GetNode(start).state = Node2.State.Open;
        GetNode(start).G = 0;
        Merge(start.x, start.y);

        while (openList.Count > 0)
        {
            // Find the node with the lowest F score
            Vector2Int lowestFCoord = openList[0];
            int lowestF = GetNode(lowestFCoord).F;
            int lowestFIndex = 0;
            Profiler.BeginSample("LowestF");
            for (int i = 1; i < openList.Count; ++i)
            {
                if (GetNode(openList[i]).F < lowestF)
                {
                    lowestFIndex = i;
                    lowestFCoord = openList[i];
                    lowestF = GetNode(lowestFCoord).F;
                }
            }
            Profiler.EndSample();

            currentCoord = lowestFCoord;
            Node2 currentNode = GetNode(currentCoord);
            currentNode.state = Node2.State.Closed;

            // Remove the current node from the open list
            openList[lowestFIndex] = openList[openList.Count - 1];
            openList.RemoveAt(openList.Count - 1);

            // Process adjacent nodes
            for (int i = 0; i < directions.Length; ++i)
            {
                Vector2Int adjCoord = currentCoord + directions[i];
                Node2 adjCoordNode = GetNode(adjCoord);
                Merge(adjCoord.x, adjCoord.y);
                int cost = adjCoordNode.C;

                if (adjCoordNode.Wall)
                {
                    // Skip walls
                }
                else if (adjCoordNode.state == Node2.State.Closed)
                {
                    // Skip closed nodes
                }
                else if (adjCoordNode.state == Node2.State.Open)
                {
                    // Update the node if a better path is found
                    if (adjCoordNode.G > currentNode.G + cost)
                    {
                        adjCoordNode.G = currentNode.G + cost;
                        adjCoordNode.H = Math.Abs(adjCoord.x - end.x) + Math.Abs(adjCoord.y - end.y);
                        adjCoordNode.F = adjCoordNode.G + adjCoordNode.H;
                        adjCoordNode.Parent = currentCoord;
                    }
                }
                else if (adjCoordNode.state == Node2.State.None)
                {
                    // Add the node to the open list
                    adjCoordNode.G = currentNode.G + cost;
                    adjCoordNode.H = Math.Abs(adjCoord.x - end.x) + Math.Abs(adjCoord.y - end.y);
                    adjCoordNode.F = adjCoordNode.G + adjCoordNode.H;
                    adjCoordNode.Parent = currentCoord;
                    adjCoordNode.state = Node2.State.Open;
                    openList.Add(adjCoord);
                }
            }

            // Check if the end node has been reached
            if (GetNode(end).state == Node2.State.Closed)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                Vector2Int backtrackCoord = end;
                while (backtrackCoord.x != -1)
                {
                    path.Add(backtrackCoord);
                    backtrackCoord = GetNode(backtrackCoord).Parent;
                }

                return path;
            }
        }

        // Return an empty path if no path is found
        Debug.Log("No path found");
        return new List<Vector2Int>();
    }

    // Coroutine to wait for 5 seconds and then find and process rocks and plants
    IEnumerator Wait5()
    {
        yield return new WaitForSeconds(5f);

        // Find all game objects with the tag "Rocks" and add them to the rocks list
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Rocks"))
        {
            rocks.Add(i);
        }

        // Find all game objects with the tag "Plant" and add them to the plants list
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Plant"))
        {
            plants.Add(i);
        }

        // Process each rock and mark the corresponding nodes as walls
        foreach (GameObject i in rocks)
        {
            int xIndex = Mathf.RoundToInt(i.transform.position.x);
            int zIndex = Mathf.RoundToInt(i.transform.position.z);

            // Check if the indices are within the bounds of the Nodes array
            if (zIndex > 1 && zIndex < 4999 && xIndex > 1 && xIndex < 4999)
            {
                GetNode(new Vector2Int(xIndex, zIndex)).Wall = true;
                GetNode(new Vector2Int(xIndex + 1, zIndex)).Wall = true;
                GetNode(new Vector2Int(xIndex - 1, zIndex)).Wall = true;
                GetNode(new Vector2Int(xIndex, zIndex + 1)).Wall = true;
                GetNode(new Vector2Int(xIndex, zIndex - 1)).Wall = true;
                GetNode(new Vector2Int(xIndex + 1, zIndex + 1)).Wall = true;
                GetNode(new Vector2Int(xIndex - 1, zIndex - 1)).Wall = true;
                // Debug.Log(xIndex + " ," + zIndex);
            }
            else
            {
                // Destroy the rock if it is out of bounds
                // Debug.LogError("Index out of bounds Rock: (" + xIndex + ", " + zIndex + ") Phoenix Made Error");
                Destroy(i);
            }
        }

        // Process each plant and set the cost of the corresponding nodes
        foreach (GameObject i in plants)
        {
            int xIndex = Mathf.RoundToInt(i.transform.position.x);
            int zIndex = Mathf.RoundToInt(i.transform.position.z);

            // Check if the indices are within the bounds of the Nodes array
            if (zIndex > 1 && zIndex < 4999 && xIndex > 1 && xIndex < 4999)
            {
                GetNode(new Vector2Int(xIndex, zIndex)).C = 5;
                GetNode(new Vector2Int(xIndex + 1, zIndex)).C = 5;
                GetNode(new Vector2Int(xIndex - 1, zIndex)).C = 5;
                GetNode(new Vector2Int(xIndex, zIndex + 1)).C = 5;
                GetNode(new Vector2Int(xIndex, zIndex - 1)).C = 5;
                GetNode(new Vector2Int(xIndex + 1, zIndex + 1)).C = 5;
                GetNode(new Vector2Int(xIndex - 1, zIndex - 1)).C = 5;
                // Debug.Log(xIndex + " ," + zIndex);
            }
            else
            {
                // Destroy the plant if it is out of bounds
                // Debug.LogError("Index out of bounds Plant: (" + xIndex + ", " + zIndex + ") Phoenix Made Error");
                Destroy(i);
            }
        }

        // Clear the rocks list
        for (int i = rocks.Count - 1; i >= 0; i--)
        {
            rocks.RemoveAt(i);
        }

        // Clear the plants list
        for (int i = plants.Count - 1; i >= 0; i--)
        {
            plants.RemoveAt(i);
        }

        Debug.Log("Done");
    }
}

