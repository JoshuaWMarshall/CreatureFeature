using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Pathfinding : MonoBehaviour
{
    public GridGraph gridGraph;
    
    public Transform startPoint;
    public Transform endPoint;

    public Pathfinding(GridGraph gridGraph)
    {
        this.gridGraph = gridGraph;
    }
    
    private void Start()
    {
        // List<GridNode> path = FindPath(startPoint.position, endPoint.position);
        // if (path != null)
        // {
        //     foreach (var node in path)
        //     {
        //         Debug.Log("Path node: " + node.Position);
        //     }
        // }
        // else
        // {
        //     Debug.Log("No path found");
        // }
    }

    public List<GridNode> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        if (!gridGraph.IsValidPosition(new Vector2Int((int)startWorldPos.x, (int)startWorldPos.z)) 
            ||!gridGraph.IsValidPosition(new Vector2Int((int)endWorldPos.x, (int)endWorldPos.z)))
        {
            Debug.Log("Invalid Path: incorrect start or end position");
            return null;
        }
        
        Vector2Int startGridPos = WorldToGrid(startWorldPos);
        Vector2Int endGridPos = WorldToGrid(endWorldPos);

        GridNode startNode = gridGraph.grid[startGridPos.x, startGridPos.y];
        GridNode endNode = gridGraph.grid[endGridPos.x, endGridPos.y];

        if (!startNode.Walkable || !endNode.Walkable)
        {
            return null;
        }

        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closedList = new HashSet<GridNode>();

        openList.Add(startNode);

        Dictionary<GridNode, GridNode> cameFrom = new Dictionary<GridNode, GridNode>();
        Dictionary<GridNode, float> gScore = new Dictionary<GridNode, float>();
        Dictionary<GridNode, float> fScore = new Dictionary<GridNode, float>();

        gScore[startNode] = 0;
        fScore[startNode] = Heuristic(startNode, endNode);

        while (openList.Count > 0)
        {
            GridNode currentNode = GetLowestFScoreNode(openList, fScore);

            if (currentNode.Equals(endNode))
            {
                return ReconstructPath(cameFrom, currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<Connection> connections = gridGraph.GetConnections(currentNode);

            foreach (var connection in connections)
            {
                GridNode neighbor = connection.ToNode;

                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[currentNode] + connection.Cost;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = currentNode;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, endNode);
            }
        }

        return null; // No path found
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / gridGraph.nodeSize);
        int z = Mathf.RoundToInt(worldPosition.z / gridGraph.nodeSize);
        return new Vector2Int(x, z);
    }

    private GridNode GetLowestFScoreNode(List<GridNode> openList, Dictionary<GridNode, float> fScore)
    {
        GridNode lowestFScoreNode = openList[0];
        float lowestFScore = fScore.ContainsKey(lowestFScoreNode) ? fScore[lowestFScoreNode] : float.MaxValue;

        foreach (var node in openList)
        {
            float score = fScore.ContainsKey(node) ? fScore[node] : float.MaxValue;
            if (score < lowestFScore)
            {
                lowestFScore = score;
                lowestFScoreNode = node;
            }
        }

        return lowestFScoreNode;
    }

    private float Heuristic(GridNode a, GridNode b)
    {
        return Vector2Int.Distance(a.Position, b.Position);
    }

    private List<GridNode> ReconstructPath(Dictionary<GridNode, GridNode> cameFrom, GridNode currentNode)
    {
        List<GridNode> path = new List<GridNode>();
        path.Add(currentNode);

        while (cameFrom.ContainsKey(currentNode))
        {
            currentNode = cameFrom[currentNode];
            path.Add(currentNode);
        }

        path.Reverse();
        return path;
    }
}
