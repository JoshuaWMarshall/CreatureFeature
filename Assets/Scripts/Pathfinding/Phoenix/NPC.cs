using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class NPC : MonoBehaviour
{
    public List<Vector2Int> Path = new List<Vector2Int>();  // List of waypoints for the NPC to follow
    public int speed = 80;     // Speed of the NPC
    public Transform objTarget; // Target object the NPC is moving towards
    private Vector2Int givenTarget; // Target position in grid coordinates
    public float currentDistance;  // Current distance to the target
    public int maxRange = 100; // Maximum range the NPC can move in one step
    private bool isRandom = false; // Flag to indicate if the NPC is moving randomly
    
    // Update is called once per frame
    void Update()
    {
        // Perform a raycast to adjust the NPC's Y position
        YRayCast();
        
        // Draw the path the NPC is following
        for(int i = 0; i < Path.Count - 1; ++i)
        {
            Debug.DrawLine(
                new Vector3(Path[i].x + 0.5f, 0.1f, Path[i].y + 0.5f),
                new Vector3(Path[i + 1].x + 0.5f, 0.1f, Path[i + 1].y + 0.5f),
                Color.green);
        }

        // If there is no path and a target is set, calculate a new path
        if (Path.Count == 0 && objTarget != null && isRandom == false)
        {
            // Set the target position based on the target object's position
            givenTarget.x = (int)objTarget.position.x;
            givenTarget.y = (int)objTarget.position.z;
            
            // Stop the NPC's movement
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            
            Vector2Int newtarget = new Vector2Int();
            currentDistance = Vector2Int.Distance(new Vector2Int((int)transform.position.x, (int)transform.position.z), givenTarget);
            
            if (currentDistance > 1)
            {
                do
                {
                    if (currentDistance <= maxRange)
                    {
                        newtarget = givenTarget;
                    }
                    else if(currentDistance > maxRange)
                    {
                        int deltaX = givenTarget.x - (int)transform.position.x;
                        int deltaY = givenTarget.y - (int)transform.position.z;

                        int newTargetX = (Math.Abs(deltaX) > maxRange) ? (int)transform.position.x + Math.Sign(deltaX) * maxRange : givenTarget.x;
                        int newTargetY = (Math.Abs(deltaY) > maxRange) ? (int)transform.position.z + Math.Sign(deltaY) * maxRange : givenTarget.y;

                        newtarget = new Vector2Int(newTargetX, newTargetY);
                    }
                    
                } while (AstarPathFind.GetNode(newtarget).Wall);

                // Find a path to the new target
                Path = AstarPathFind.FindPath(
                    new Vector2Int((int)transform.position.x, (int)transform.position.z),
                    newtarget);
            }
        }
        // If there is no target and no path, move randomly
        else if (objTarget == null && Path.Count == 0)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Vector2Int newtarget = new Vector2Int();
            isRandom = true;
            
            do
            {
                // Calculate valid range for x
                int minX = Mathf.Max(0, (int)transform.position.x - maxRange);
                int maxX = Mathf.Min(AstarPathFind.gridWidth, (int)transform.position.x + maxRange);
                Math.Clamp(newtarget.x, 0, AstarPathFind.gridWidth);
                newtarget.x = Random.Range(minX, maxX);

                // Calculate valid range for y
                int minY = Mathf.Max(0, (int)transform.position.z - maxRange);
                int maxY = Mathf.Min(AstarPathFind.gridHeight, (int)transform.position.z + maxRange);
                Math.Clamp(newtarget.y, 0, AstarPathFind.gridWidth);
                newtarget.y = Random.Range(minY, maxY);

            } while (AstarPathFind.GetNode(newtarget).Wall);
            
            // Find a path to the new random target
            Path = AstarPathFind.FindPath(
                new Vector2Int((int)transform.position.x, (int)transform.position.z),
                newtarget);
        }
        
        // If there is a path, move along it
        if (Path.Count != 0)
        {
            // If moving randomly and a target is set, clear the path
            if (isRandom == true && objTarget != null)
            {
                for (int i = Path.Count - 1; i >= 0; i--)
                {
                    Path.RemoveAt(i);
                    if (i == 0) // Stop the loop after removing the last element
                    {
                        break;
                    }
                }

                isRandom = false;
            }
            else
            {
                // Move towards the last waypoint in the path
                Vector3 target = new Vector3(
                    Path[Path.Count - 1].x + AstarPathFind.cellSize * 0.5f, 
                    0.5f, 
                    Path[Path.Count - 1].y + AstarPathFind.cellSize * 0.5f);
                
                // Calculate the direction to the target
                Vector3 direction = (target - transform.position).normalized;
                direction.y = 0; // Set the y component to 0 to only affect x and z axes
                GetComponent<Rigidbody>().velocity = direction * speed * Time.deltaTime;

                // Calculate the rotation angle in degrees
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                // Rotate the game object to face the target direction
                Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

                // Smoothly rotate the game object towards the target direction using slerp
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);

                // If close enough to the target, remove the last waypoint from the path
                if (Mathf.Abs(transform.position.x - target.x) < 0.6f && Mathf.Abs(transform.position.z - target.z) < 0.6f)
                {
                    Path.RemoveAt(Path.Count - 1);
                }
            }
        }
    }

    // Perform a raycast to adjust the NPC's Y position
    public void YRayCast()
    {
        RaycastHit hit;
        int layerMask = ~(1 << 14) & ~(1 << 15); // Ignore layer 14 and 15
        Vector3 raySpot = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        float rayY = transform.position.y + 1000;
        raySpot.y = rayY;

        if (Physics.Raycast(raySpot, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            // Adjust the NPC's Y position to match the hit point
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        else
        {
            Debug.Log("Raycast did not hit or hit a layer that should be ignored.");
        }
    }

    // Set the destination target for the NPC
    public void SetDestination(Transform destination)
    {
        objTarget = destination;
    }
}