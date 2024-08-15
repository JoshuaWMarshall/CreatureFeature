using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    public LayerMask terrainLayerMask = 13;
    public float waterHeight = 100f; // Set the height of the water plane
    
    public virtual void Start()
    {
        Invoke("FindLand", 0.1f);

        //FindLand();
    }

    public virtual void FindLand()
    {
        Ray rayDown = new Ray(transform.position, Vector3.down);
        Ray rayUp = new Ray(transform.position, Vector3.up);
        RaycastHit hitInfo;

        bool foundLand = false;

        if (Physics.Raycast(rayDown, out hitInfo, Mathf.Infinity, terrainLayerMask))
        {
            transform.position = hitInfo.point;
            foundLand = true;
        }
        else if (Physics.Raycast(rayUp, out hitInfo, Mathf.Infinity, terrainLayerMask))
        {
            transform.position = hitInfo.point;
            foundLand = true;
        }
        
        if (foundLand)
        {
            // Check if the object is below the water height
            if (transform.position.y <= waterHeight)
            {
                //Debug.Log("Object is below water");
                Destroy(gameObject);
            }
        }
        else
        {
            // Didnt find any mesh to place on
            Destroy(gameObject);
        }
    }
}
