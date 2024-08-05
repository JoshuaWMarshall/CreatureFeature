using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static Texture2D GetNoiseMap(int width, int height, float scale)
    {
        // create a new texture and set its size
        Texture2D noiseMapTexture = new Texture2D(width, height);
        
        // Iterate over each pixel in the texture
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Generate a noise value using Mathf.PerlinNoise
                float noiseValue = Mathf.PerlinNoise((float)x / width * scale, (float)y / height * scale);
                
                // Set the pixel color based on the noise value
                noiseMapTexture.SetPixel(x, y, new Color(0, noiseValue, 0));
                
            }
        }
        
        // Apply the changes to the texture
        noiseMapTexture.Apply();

        return noiseMapTexture;
    }
}
