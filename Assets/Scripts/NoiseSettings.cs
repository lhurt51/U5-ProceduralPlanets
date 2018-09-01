using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings {

    [Range(1, 8)]
    public int numLayers = 1;
    public float strength = 1.0f;
    public float baseRoughness = 1.0f;
    public float roughness = 2.0f;
    public float persistance = 0.5f;
    public float minVal;
    public Vector3 center;

}
