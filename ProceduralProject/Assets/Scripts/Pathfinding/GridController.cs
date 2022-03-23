using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridController : MonoBehaviour {

    delegate Pathfinder.Node LookupDelegate(int x, int y);

    public static GridController singleton { get; private set; }

    public TerrainCube cubePrefab;

    public Transform helperStart;
    public Transform helperEnd;

    private TerrainCube[,] cubes;
    private Pathfinder.Node[,] nodes;
    
    void Start()
    {
        if(singleton != null){ // we already have a singleton ...
            Destroy(gameObject);
            return;
        }

        singleton = this;
        //DontDestroyOnLoad(gameObject);

        MakeGrid();
    }
    void OnDestroy(){
        if(this == singleton) singleton = null;
    }

    void MakeGrid(){
        int size = 19;
        cubes = new TerrainCube[size,size];


        float zoom = 10;
        float amp = 10;

        for(int x = 0; x < size; x++){
            for(int y = 0; y < size; y++){

                float verticalPosition = Mathf.PerlinNoise(x/zoom, y/zoom) * amp;

                cubes[x,y] = Instantiate(cubePrefab, new Vector3(x, verticalPosition, y), Quaternion.identity);
            }
        }
    }
    public void MakeNodes(){

        nodes = new Pathfinder.Node[cubes.GetLength(0), cubes.GetLength(1)];

        for(int x = 0; x < cubes.GetLength(0); x++){
            for(int y = 0; y < cubes.GetLength(1); y++){

                Pathfinder.Node n = new Pathfinder.Node();

                n.position = cubes[x,y].transform.position;
                n.moveCost = cubes[x,y].MoveCost;
                
                nodes[x,y] = n;
            }
        }

        LookupDelegate lookup = (x, y) => {
            if(x < 0) return null;
            if(y < 0) return null;
            if(x >= nodes.GetLength(0)) return null;
            if(y >= nodes.GetLength(1)) return null;
            return nodes[x,y];
        };

        for(int x = 0; x < nodes.GetLength(0); x++){
            for(int y = 0; y < nodes.GetLength(1); y++){

                Pathfinder.Node n = nodes[x,y];

                Pathfinder.Node neighbor1 = lookup(x + 1, y);
                Pathfinder.Node neighbor2 = lookup(x - 1, y);
                Pathfinder.Node neighbor3 = lookup(x, y + 1);
                Pathfinder.Node neighbor4 = lookup(x, y - 1);

                Pathfinder.Node neighbor5 = lookup(x + 1, y + 1);
                Pathfinder.Node neighbor6 = lookup(x - 1, y + 1);
                Pathfinder.Node neighbor7 = lookup(x - 1, y - 1);
                Pathfinder.Node neighbor8 = lookup(x + 1, y - 1);

                if(neighbor1 != null) n.neighbors.Add( neighbor1 );
                if(neighbor2 != null) n.neighbors.Add( neighbor2 );
                if(neighbor3 != null) n.neighbors.Add( neighbor3 );
                if(neighbor4 != null) n.neighbors.Add( neighbor4 );
                if(neighbor5 != null) n.neighbors.Add( neighbor5 );
                if(neighbor6 != null) n.neighbors.Add( neighbor6 );
                if(neighbor7 != null) n.neighbors.Add( neighbor7 );
                if(neighbor8 != null) n.neighbors.Add( neighbor8 );

            }
        }
    }
    public Pathfinder.Node Lookup(Vector3 pos){

        if(nodes == null){ 
            MakeNodes();
        }

        float w = 1;
        float h = 1;
        
        pos.x += w/2;
        pos.z += h/2;

        int x = (int)(pos.x / w);
        int y = (int)(pos.z / h);

        if(x < 0 || y < 0) return null;
        if(x >= nodes.GetLength(0) || y >= nodes.GetLength(1)) return null;

        return nodes[x,y];
    }
}


[CustomEditor(typeof(GridController))]
class GridControllerEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if( GUILayout.Button("find a path") ){
            (target as GridController).MakeNodes();
        }

    }
}
