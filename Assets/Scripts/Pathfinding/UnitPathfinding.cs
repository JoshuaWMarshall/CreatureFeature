using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathfinding : MonoBehaviour
{
    public Transform target;
    private Pathfinding pathfinding;
    //private WorldGeneration worldGeneration;
    private GridGraph gridGraph;
    private List<GridNode> path;
    private int currentPathIndex;
    public float pathUpdateDistance = 1; // Distance threshold to skip pathfinding

    void Start()
    {
        //worldGeneration = FindObjectOfType<WorldGeneration>();
        target = GameObject.FindGameObjectWithTag("Target").transform;
        
        //gridGraph = worldGeneration.gridGraph;
        /* This is where we should use the mesh from our terrain
         * not sure how to shange from a grid to 3d mesh?????
         */

        pathfinding = new Pathfinding(gridGraph);
        InvokeRepeating(nameof(FindPath), 0f, 0.5f); // Find path every 0.5 seconds
    }

    void FindPath()
    {
        // // Check if the unit is within the update distance to the target
        // if (Vector3.Distance(transform.position, target.position) > pathUpdateDistance)
        // {
            path = pathfinding.FindPath(transform.position, target.position);
            currentPathIndex = 0;
        //}
    }

    void Update()
    {
        if (path != null && currentPathIndex < path.Count)
        {
            Vector3 targetPosition = gridGraph.GetWorldPositionFromNode(path[currentPathIndex]);
            targetPosition.y = transform.position.y;

            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);
            }
            else
            {
                currentPathIndex++;
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (path == null || path.Count == 0) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 fromPos = gridGraph.GetWorldPositionFromNode(path[i]);
            Vector3 toPos = gridGraph.GetWorldPositionFromNode(path[i + 1]);
            Gizmos.DrawLine(fromPos, toPos);
        }
    }
}
