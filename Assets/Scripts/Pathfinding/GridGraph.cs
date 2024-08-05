using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridGraph
{
    private int gridSizeX;
    private int gridSizeZ;
    public float nodeSize = 1f;
    public float offset = 0.5f;
    
    public bool showOutline = true;
    public bool showConnections = true;
    public bool allowDiagonalMovement = true;
    
    public GridNode[,] grid;

    void Start()
    {
        GenerateGrid(true);
    }

    public GridGraph(int gridSizeX, int gridSizeZ)
    {
        this.gridSizeX = gridSizeX;
        this.gridSizeZ = gridSizeZ;
    }
    
    public void GenerateGrid(bool allowDiagonalMovement)
    {
        this.allowDiagonalMovement = allowDiagonalMovement;
        
        grid = new GridNode[gridSizeX, gridSizeZ];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector2Int position = new Vector2Int(x, z);
                bool walkable = true;
                
                // TO DO: add logic to determine if a node is walkable

                grid[x, z] = new GridNode(position, walkable);
            }
        }
    }
    
    // Update the walkable status of a specific node and recalculate connections if needed
    public void UpdateNode(Vector2Int position, bool walkable)
    {
        if (IsValidPosition(position))
        {
            GridNode node = GetNodeFromIndex(position.x, position.y);
            node.Walkable = walkable;
            grid[position.x, position.y] = node;
            
            UpdateConnections(position);
        }
    }
    
    private void UpdateConnections(Vector2Int position)
    {
        // Find the updated node
        GridNode node = grid[position.x, position.y];

        // Update connections for this node
        List<Connection> connections = GetConnections(node);
    }
    
    public List<Connection> GetConnections(GridNode node)
    {
        List<Connection> connections = new List<Connection>();

        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
        
        if (allowDiagonalMovement)
        {
            Vector2Int[] diagonalDirections = {
                new Vector2Int(1, 1),
                new Vector2Int(-1, -1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, 1)
            };
            directions = directions.Concat(diagonalDirections).ToArray();
        }
        
        foreach (var direction in directions)
        {
            Vector2Int neighborPos = node.Position + direction;
            if (IsValidPosition(neighborPos))
            {
                GridNode neighborNode = grid[neighborPos.x, neighborPos.y];
                if (neighborNode.Walkable)
                {
                    float cost = Vector2Int.Distance(node.Position, neighborPos);
                    connections.Add(new Connection(node, neighborNode, cost));
                }
            }
        }

        return connections;
    }

    public bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSizeX && position.y >= 0 && position.y < gridSizeZ;
    }
    
    public Vector3 GetWorldPositionFromNode(GridNode node) {
        // Add the offset to center the nodes 
        return new Vector3(node.Position.x + offset, 0, node.Position.y + offset) * nodeSize;
    }

    public GridNode GetNodeFromIndex(int x, int z)
    {
        return grid[x, z];
    }
}
