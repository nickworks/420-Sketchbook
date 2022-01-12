using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour
{

    public GameObject voxelPrefab;
    public int chunkSize = 16;

    // a single noise field:
    public Vector2 offset;
    public float zoom = 20;
    public float amp = 10;

    void Start()
    {
        BuildChunk();
    }

    void BuildChunk() {

        if (!voxelPrefab) return;

        for(int x = 0; x < chunkSize; x++) {
            for (int z = 0; z < chunkSize; z++) {

                float val = Mathf.PerlinNoise(x/zoom + offset.x, z/zoom + offset.y);

                Vector3 pos = new Vector3(x, val * amp, z);

                Instantiate(voxelPrefab, pos, Quaternion.identity, transform);
            }
        }


    }

}
