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
    public enum State
    {
        None,
        Open,
        Closed
    }
    public int F = 0;   // Total estimated path length. F = G + h
    public int G = 0;   // Distance travelled so far.
    public int H = 0;   // Estimated distance remaining to target.
    public int C = 1;   // Cost of walking over this node.
    public bool Wall = false;   // Walls block movement.
    public Vector2Int Parent = new Vector2Int(-1,-1);   // The node before this one.
    public State state = State.None;    // Current node state. Could be none (not reached yet), Open (possible next node) and Closed (reached the node).
}
public class AstarPathFind : MonoBehaviour
{
    public static int gridWidth = 5000;
    public static int gridHeight = 5000;
    public static float cellSize = 1.0f;
    public static Vector2Int maxCorner = new Vector2Int(gridWidth-1, gridHeight-1);
    public static Vector2Int minCorner = new Vector2Int(0, 0);
    public static Node2[,] Nodes;
    List<GameObject> rocks = new List<GameObject>();
    List<GameObject> plants = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
        Nodes = new Node2[gridHeight, gridWidth];
        for (int z = 0; z < gridHeight; z++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Nodes[gridHeight - z - 1, x] = new Node2();
                Nodes[gridHeight - z - 1, x].Wall = false;

            }
        }
        // Find all game objects with the tag "rocks"
       StartCoroutine(Wait5());

    }

    
    // Update is called once per frame
    void Update()
    {
        // for (int y = 0; y <= gridHeight; y++)
        // {
        //     Debug.DrawLine(new Vector3(0, 0.1f, y), new Vector3(gridWidth, 0.1f, y));
        // }
        // for (int x = 0; x <= gridWidth; ++x)
        // {
        //     Debug.DrawLine(new Vector3(x, 0.1f, 0), new Vector3(x, 0.1f, gridHeight));
        // }
    }

    public static Node2 GetNode(Vector2Int pos)
    {
        return Nodes[pos.x, pos.y];
    }

    static void Merge(int x, int y)
    {
        if (x < minCorner.x) minCorner.x = x;
        if (x > maxCorner.x) maxCorner.x = x;
        if (y < minCorner.y) minCorner.y = y;
        if (y > maxCorner.y) maxCorner.y = y;
    }
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

        minCorner.x--;
        maxCorner.x++;
        minCorner.y--;
        maxCorner.y++;
        minCorner.x = Math.Clamp(minCorner.x, 0, gridWidth - 1);
        minCorner.y = Math.Clamp(minCorner.y, 0, gridHeight - 1);
        maxCorner.x = Math.Clamp(maxCorner.x, 0, gridWidth - 1);
        maxCorner.y = Math.Clamp(maxCorner.y, 0, gridHeight - 1);
        for (int y = minCorner.y; y <= maxCorner.y; ++y)
        {
            for (int x = minCorner.x; x < maxCorner.x; ++x)
            {
                Nodes[x, y].state = Node2.State.None;
                Nodes[x, y].Parent = new Vector2Int(-1, -1);
                Nodes[x, y].G = 0;
            }
        }
    minCorner = new Vector2Int(gridWidth-1, gridHeight-1);
    maxCorner = new Vector2Int(0, 0);

    
        // A* goes here
        List<Vector2Int> openList = new List<Vector2Int>();

        Vector2Int currentCoord;

        openList.Add(start);

        GetNode(start).state = Node2.State.Open;
        GetNode(start).G = 0;
        Merge(start.x,start.y);
        while (openList.Count > 0)
        {
            Vector2Int lowestFCoord = openList[0];
            int lowestF = GetNode(lowestFCoord).F;
            int lowestFIndex = 0;
            Profiler.BeginSample("LowestF");
            for(int i=1; i<openList.Count; ++i)
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
//            openList.RemoveAt(lowestFIndex);
            openList[lowestFIndex] = openList[openList.Count - 1];
            openList.RemoveAt(openList.Count-1);
            for(int i=0;i<directions.Length; ++i)
            {
                Vector2Int adjCoord = currentCoord + directions[i];
                Node2 adjCoordNode = GetNode(adjCoord);
                Merge(adjCoord.x,adjCoord.y);
                int cost = adjCoordNode.C;
                if (adjCoordNode.Wall)
                {
                    // do nothing
                }
                else if(adjCoordNode.state == Node2.State.Closed) 
                { 
                    // do nothing
                }
                else if(adjCoordNode.state == Node2.State.Open)
                {
                    if(adjCoordNode.G > currentNode.G + cost)
                    {
                        adjCoordNode.G = currentNode.G + cost;
                        adjCoordNode.H = Math.Abs(adjCoord.x - end.x)+ Math.Abs(adjCoord.y - end.y);
                        adjCoordNode.F = adjCoordNode.G + adjCoordNode.H;
                        adjCoordNode.Parent = currentCoord;
                    }
                }
                else if(adjCoordNode.state == Node2.State.None)
                {

                    adjCoordNode.G = currentNode.G + cost;
                    adjCoordNode.H = Math.Abs(adjCoord.x - end.x) + Math.Abs(adjCoord.y - end.y); ;
                    adjCoordNode.F = adjCoordNode.G + adjCoordNode.H;
                    adjCoordNode.Parent = currentCoord;
                    adjCoordNode.state = Node2.State.Open;
                    openList.Add(adjCoord);
                }
            }
            if(GetNode(end).state == Node2.State.Closed)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                Vector2Int backtrsckCoord = end;
                while(backtrsckCoord.x != -1)
                {
                    path.Add(backtrsckCoord);
                    backtrsckCoord = GetNode(backtrsckCoord).Parent;
                }
                
                return path;
            }
        }
        // Return empty path
        Debug.Log("No path found");
        return new List<Vector2Int>();

    }

    IEnumerator Wait5()
    {
        yield return new WaitForSeconds(5f);
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Rocks"))
        {
            rocks.Add(i); 
        }
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Plant"))
        {
            plants.Add(i);
        }
        foreach (GameObject i in rocks)
        {
            int xIndex = Mathf.RoundToInt(i.transform.position.x);
            
            int zIndex = Mathf.RoundToInt(i.transform.position.z);

            // Check if the indices are within the bounds of the Nodes array
            if (zIndex > 1 && zIndex < 4999 && xIndex > 1 && xIndex < 4999)
            {
                GetNode(new Vector2Int(xIndex, zIndex)).Wall = true;
                GetNode(new Vector2Int(xIndex + 1, zIndex)).Wall = true;
                GetNode(new Vector2Int(xIndex + -1, zIndex)).Wall = true;
                GetNode(new Vector2Int(xIndex, zIndex + 1)).Wall = true;
                GetNode(new Vector2Int(xIndex, zIndex - 1)).Wall = true;
                GetNode(new Vector2Int(xIndex + 1, zIndex + 1)).Wall = true;
                GetNode(new Vector2Int(xIndex - 1, zIndex - 1)).Wall = true;
                //Debug.Log(xIndex +" ," + zIndex );
            }
            else
            {
                //Debug.LogError("Index out of bounds Rock: (" + xIndex + ", " + zIndex + ") Phoenix Made Error");
                Destroy(i);
            }
        }
        foreach (GameObject i in plants)
        {
            int xIndex = Mathf.RoundToInt(i.transform.position.x);
            int zIndex = Mathf.RoundToInt(i.transform.position.z);

            // Check if the indices are within the bounds of the Nodes array
            if (zIndex > 1 && zIndex < 4999 && xIndex > 1 && xIndex < 4999)
            {
                GetNode(new Vector2Int(xIndex, zIndex)).C = 5;
                GetNode(new Vector2Int(xIndex + 1, zIndex)).C = 5;
                GetNode(new Vector2Int(xIndex + -1, zIndex)).C = 5;
                GetNode(new Vector2Int(xIndex, zIndex + 1)).C = 5;
                GetNode(new Vector2Int(xIndex, zIndex - 1)).C = 5;
                GetNode(new Vector2Int(xIndex + 1, zIndex + 1)).C = 5;
                GetNode(new Vector2Int(xIndex - 1, zIndex - 1)).C = 5;
                // Debug.Log(xIndex +" ," + zIndex );
            }
            else
            {
                //Debug.LogError("Index out of bounds Plant: (" + xIndex + ", " + zIndex + ") Phoenix Made Error");
                Destroy(i);
            }
        }

        for (int i = rocks.Count - 1; i >= 0; i--)
        {
            rocks.RemoveAt(i);
        }

        for (int i = plants.Count -1; i >= 0; i--)
        {
            plants.RemoveAt(i);
        }
        Debug.Log("Done");
    }
}

