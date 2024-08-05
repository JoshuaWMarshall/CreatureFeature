using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public List<Vector2Int> Path = new List<Vector2Int>();

    void Start()
    {
    }

    void Update()
    {
        YRayCast();
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        for(int i=0;i<Path.Count-1;++i)
        {
            Debug.DrawLine(
                new Vector3(Path[i].x + 0.5f, 0.1f, Path[i].y + 0.5f),
                new Vector3(Path[i + 1].x + 0.5f, 0.1f, Path[i + 1].y + 0.5f),
                Color.green);
        }

        if (Path.Count == 0)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Vector2Int newtarget = new Vector2Int();
            do
            {
                newtarget.x = Random.Range(1, AstarPathFind.gridWidth - 1);
                newtarget.y = Random.Range(1, AstarPathFind.gridHeight - 1);
            } while (AstarPathFind.GetNode(newtarget).Wall);

            Path = AstarPathFind.FindPath(
                new Vector2Int((int)transform.position.x, (int)transform.position.z),
                newtarget);
        }
        if (Path.Count != 0)
        {
            Vector3 target = new Vector3(
                Path[Path.Count - 1].x + AstarPathFind.cellSize * 0.5f, 
                0.5f, 
                Path[Path.Count - 1].y + AstarPathFind.cellSize * 0.5f);
            GetComponent<Rigidbody>().velocity = (target - transform.position).normalized * 80.0f;
            // Calculate the direction to the target
            Vector3 direction = (target - transform.position).normalized;

            // Calculate the rotation angle in degrees
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Rotate the game object to face the target direction
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

// Smoothly rotate the game object towards the target direction using slerp
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);

            if (Mathf.Abs(transform.position.x - target.x) < 0.6f && Mathf.Abs(transform.position.z - target.z) < 0.6f)
            {
                Path.RemoveAt(Path.Count - 1);
            }
        }
    }

    public void YRayCast()
    {
        RaycastHit hit;
        Debug.Log("ray");
        Vector3 raySpot = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        float rayY = 100;
        raySpot.y = rayY;
        
        if(Physics.Raycast(raySpot, Vector3.down, out hit, Mathf.Infinity))
        {
            Debug.Log("Raycast hit " + hit.point);
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        else
        {
            Debug.Log("not work");
        }
    }
    
}
