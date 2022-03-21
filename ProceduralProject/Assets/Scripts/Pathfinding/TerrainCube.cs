using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TerrainType {
    Open,
    Wall,
    Slime
}
public class TerrainCube : MonoBehaviour {


    public Transform wall;
    public Transform slime;

    BoxCollider box;

    public TerrainType type = TerrainType.Open;

    public float MoveCost {
        get {
            if(type == TerrainType.Open) return 1;
            if(type == TerrainType.Wall) return 9999;
            if(type == TerrainType.Slime) return 10;
            return 1;
        }
    }

    void Start() {
        box = GetComponent<BoxCollider>();
        UpdateArt();
    }

    void OnMouseDown() {
        
        type += 1;
        if((int)type > 2) type = 0;

        UpdateArt();
    }
    void UpdateArt(){

        bool isShowingWall = (type == TerrainType.Wall);

        float y = isShowingWall ? .44f : 0f;
        float h = isShowingWall ? 1.1f : .2f;
        box.size = new Vector3(1, h, 1);
        box.center = new Vector3(0, y, 0);

        if(wall) wall.gameObject.SetActive(isShowingWall);
        if(slime) slime.gameObject.SetActive(type == TerrainType.Slime);
        
    }
}
