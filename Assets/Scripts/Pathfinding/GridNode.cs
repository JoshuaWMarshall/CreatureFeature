using UnityEngine;

public struct GridNode 
{
    public Vector2Int Position;
    public bool Walkable;

    public GridNode(Vector2Int position, bool walkable)
    {
        Position = position;
        Walkable = walkable;
    }
}

public struct Connection
{
    public GridNode FromNode;
    public GridNode ToNode;
    public float Cost;

    public Connection(GridNode fromNode, GridNode toNode, float cost)
    {
        FromNode = fromNode;
        ToNode = toNode;
        Cost = cost;
    }
}