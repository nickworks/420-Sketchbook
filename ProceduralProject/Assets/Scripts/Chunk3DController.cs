using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk3DController : MonoBehaviour
{

    public GameObject voxelPrefab;

    [Tooltip("How many voxels, per dimension?")]
    public int dimensionSize = 5;
    
    [Tooltip("The size of a voxel, in meters.")]
    public float voxelSize = 1;

    [Tooltip("Density threshold (0 - 1)")]
    public float threshold = 0.5f;

    [Tooltip("How much to scale the noise")]
    public float zoom = 10;

    private bool _shouldRegen = false;


    private void Start() {
        GenerateVoxels();

    }

    private void OnValidate() {
        if (!Application.isPlaying) return;
        _shouldRegen = true;   
    }
    private void Update() {
        if(_shouldRegen) {
            _shouldRegen = false;
            GenerateVoxels();
        }
    }

    private void GenerateVoxels() {
        if (!voxelPrefab) return;

        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < dimensionSize; x++) {
            for (int y = 0; y < dimensionSize; y++) {
                for (int z = 0; z < dimensionSize; z++) {

                    Vector3 pos = new Vector3(x, y, z) * voxelSize;

                    float val = Noise.Perlin(pos / zoom);

                    if (val > threshold) {
                        GameObject obj = Instantiate(voxelPrefab, pos, Quaternion.identity, transform);
                        obj.transform.localScale = Vector3.one * voxelSize;
                    }

                }
            }
        }
    }
}
