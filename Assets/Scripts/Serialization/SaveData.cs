using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    // World stats
    public int seed;
    public float waterHeight;
    public int xSize;
    public int zSize;
    public float scale;
    public int octaves;
    public float lacunarity;
    public int meshScale;
    
    // Dino stats
    public int maxDinos;
    public DinoData[] dinosData;

    // Dino state
    [Serializable]
    public class DinoData
    {
        public bool isStego;
        public string name;
        public float hunger;
        public float thirst;
        public float energy;
        public float hungerRate;
        public float thirstRate;
        public float energyRate;
        public Vector3 position;
        public Quaternion rotation;
        public string state;
    }

    // Transformation
    public TransformData[] transformData;

    [Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
