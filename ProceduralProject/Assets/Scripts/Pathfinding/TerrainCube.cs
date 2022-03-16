using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCube : MonoBehaviour {


    public Transform wall;
    BoxCollider box;
    public bool isSolid;

    void Start() {
        box = GetComponent<BoxCollider>();
        UpdateArt();
    }

    void OnMouseDown() {
        isSolid = !isSolid;
        UpdateArt();
    }
    void UpdateArt(){
        if(wall){
            wall.gameObject.SetActive(isSolid);

            float y = isSolid ? .44f : 0f;
            float h = isSolid ? 1.1f : .2f;

            box.size = new Vector3(1, h, 1);
            box.center = new Vector3(0, y, 0);
        }
    }
}
