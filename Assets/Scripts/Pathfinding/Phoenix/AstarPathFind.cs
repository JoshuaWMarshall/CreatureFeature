using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
      public static int gridWidth = 1000;
    public static int gridHeight = 1000;
    public static float cellSize = 1.0f;
    public static Node2[,] Nodes;
    //   static Texture2D tex;
    
    
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

    }
    // Update is called once per frame
    void Update()
    {
        for (int y = 0; y <= gridHeight; ++y)
        {
            Debug.DrawLine(new Vector3(0, 0.1f, y), new Vector3(gridWidth, 0.1f, y));
        }
        for (int x = 0; x <= gridWidth; ++x)
        {
            Debug.DrawLine(new Vector3(x, 0.1f, 0), new Vector3(x, 0.1f, gridHeight));
        }

    }

    public static Node2 GetNode(Vector2Int pos)
    {
        return Nodes[pos.y, pos.x];
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

        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                Nodes[y, x].state = Node2.State.None;
                Nodes[y, x].Parent = new Vector2Int(-1, -1);
                Nodes[y, x].G = 0;
            }
        }
        
        // A* goes here
        List<Vector2Int> openList = new List<Vector2Int>();

        Vector2Int currentCoord;

        openList.Add(start);

        GetNode(start).state = Node2.State.Open;
        GetNode(start).G = 0;

        while (openList.Count > 0)
        {
            Vector2Int lowestFCoord = openList[0];
            int lowestF = GetNode(lowestFCoord).F;
            int lowestFIndex = 0;
            for(int i=1; i<openList.Count; ++i)
            {
                if (GetNode(openList[i]).F < lowestF)
                {
                    lowestFIndex = i;
                    lowestFCoord = openList[i];
                    lowestF = GetNode(lowestFCoord).F;
                }
            }
            currentCoord = lowestFCoord;
            Node2 currentNode = GetNode(currentCoord);
            currentNode.state = Node2.State.Closed;
            openList.RemoveAt(lowestFIndex);
            for(int i=0;i<directions.Length; ++i)
            {
                Vector2Int adjCoord = currentCoord + directions[i];
                Node2 adjCoordNode = GetNode(adjCoord);
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
        return new List<Vector2Int>();

    }
}

