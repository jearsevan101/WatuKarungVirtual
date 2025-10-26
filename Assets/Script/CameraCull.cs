using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCull : MonoBehaviour
{
    public Camera vrCamera;
    public string targetLayerName = "CullObjects"; // the layer you want to cull
    public float cullDistance = 100f;

    void Start()
    {
        float[] distances = new float[32];

        // Find the layer index from its name
        int targetLayer = LayerMask.NameToLayer(targetLayerName);

        if (targetLayer >= 0 && targetLayer < 32)
        {
            // Apply distance only to that layer
            distances[targetLayer] = cullDistance;
        }
        else
        {
            Debug.LogWarning($"Layer '{targetLayerName}' not found!");
        }

        // Assign distances to camera
        vrCamera.layerCullDistances = distances;

        // Use spherical distance (better for VR)
        vrCamera.layerCullSpherical = true;
    }
}
