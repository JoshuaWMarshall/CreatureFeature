using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    // Layer mask to identify the terrain layer
    public LayerMask terrainLayerMask = 13;
    
    // Height of the water plane
    public float waterHeight = 100f; 

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Invoke the FindLand method after a delay of 0.1 seconds
        Invoke("FindLand", 0.1f);
    }

    // Method to find the land and position the object accordingly
    public virtual void FindLand()
    {
        // Temporarily move the object up by 100 units to ensure raycast works
        transform.position += new Vector3(0, 100, 0);
        
        // Create rays pointing downwards and upwards from the current position
        Ray rayDown = new Ray(transform.position, Vector3.down);
        Ray rayUp = new Ray(transform.position, Vector3.up);
        RaycastHit hitInfo;

        bool foundLand = false;

        // Cast a ray downwards to find the terrain
        if (Physics.Raycast(rayDown, out hitInfo, Mathf.Infinity, terrainLayerMask))
        {
            // If terrain is found, position the object at the hit point
            transform.position = hitInfo.point;
            foundLand = true;
        }
        // If no terrain is found downwards, cast a ray upwards
        else if (Physics.Raycast(rayUp, out hitInfo, Mathf.Infinity, terrainLayerMask))
        {
            // If terrain is found, position the object at the hit point
            transform.position = hitInfo.point;
            foundLand = true;
        }
        
        // Uncomment the following block if you want to destroy the object if it's below water height or if no land is found
        /*
        if (foundLand)
        {
            // Check if the object is below the water height
            if (transform.position.y <= waterHeight)
            {
                // Destroy the object if it is below the water height
                Destroy(gameObject);
            }
        }
        else
        {
            // Destroy the object if no land was found
            Destroy(gameObject);
        }
        */
    }
}
