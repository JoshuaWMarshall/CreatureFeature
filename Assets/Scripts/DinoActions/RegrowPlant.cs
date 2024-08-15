using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegrowPlant : MonoBehaviour
{
    public float deactivationTime = 2f; // Time before scaling up
    public float scaleUpTime = 2f; // Time to scale back up to original size
    public Vector3 smallScale = new Vector3(0.1f, 0.1f, 0.1f); // Small scale value
    private Vector3 originalScale;
    public bool isRegrowing = false;
    private void Start()
    {
        // Store the original scale of the plant
        originalScale = transform.localScale;
    }

    public void Eat()
    {
        if (!isRegrowing)
        {
            StartCoroutine(PlantRegrowthRoutine());
        }
    }

    private IEnumerator PlantRegrowthRoutine()
    {
        isRegrowing = true;
        
        // Scale down the plant to a small size
        transform.localScale = smallScale;

        // Wait for a bit before scaling back up (optional delay, e.g., simulating regrowth time)
        yield return new WaitForSeconds(deactivationTime);

        // Scale up the plant back to its original size
        yield return ScaleOverTime(transform, transform.localScale, originalScale, scaleUpTime);
        
        isRegrowing = false;

        yield return null;
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scale is set
        target.localScale = endScale;
    }
}
