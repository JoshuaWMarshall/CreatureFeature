using UnityEngine;

public class Noise
{
    /// <summary>
    /// Get a noise map texture of the specified size using the Perlin noise algorithm
    /// </summary>
    /// <param name="width"></param> the width of the generated noise map
    /// <param name="height"></param> the height of the generated noise map
    /// <param name="noiseScale"></param> the scale of the noise
    /// <param name="seed"></param> the random seed created from the terrain generator
    /// <returns></returns> A Texture with a noise map stored in the green channel
    public static Texture2D GetNoiseMap(int width, int height, float noiseScale, int octaves, float lacunarity, int seed, bool randomiseSeed, out int newSeed)
    {
        if (randomiseSeed)
        {
            seed = Random.Range(0, 100000);
        }

        newSeed = seed;
        
        //create a new texture and set its size
        Texture2D noiseMap = new Texture2D(width, height);
        
        if (noiseScale <= 0)
        {
            noiseScale = 0.0001f;
        }
        
        // create a random offset based on the seed
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        
        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;
        
        //Iterate over each pixel and set its colour
        for (int y = 0;y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / noiseScale * frequency + octaveOffsets[i].x;
                    float sampleY = y / noiseScale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= 0.5f;
                    frequency *= lacunarity;
                }
                
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                
                // Set the pixel colour based on the noise value
                // we will use green channel but any is fine
                noiseMap.SetPixel(x, y, new Color(0, noiseHeight, 0));
            }
        }
        //Apply the changes to the texture
        noiseMap.Apply();

        return noiseMap;
    }
}
