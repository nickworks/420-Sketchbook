using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour {

    delegate Pathfinder.Node LookupDelegate(int x, int y);

    public TerrainCube cubePrefab;

    private TerrainCube[,] cubes;
    
    void Start()
    {
        MakeGrid();
    }

    
    void Update()
    {
        
    }
    void MakeGrid(){
        int size = 19;
        cubes = new TerrainCube[size,size];

        for(int x = 0; x < size; x++){
            for(int y = 0; y < size; y++){
                cubes[x,y] = Instantiate(cubePrefab, new Vector3(x, 0, y), Quaternion.identity);
            }
        }
    }
    void MakeNodes(){

        Pathfinder.Node[,] nodes = new Pathfinder.Node[cubes.GetLength(0), cubes.GetLength(1)];

        for(int x = 0; x < cubes.GetLength(0); x++){
            for(int y = 0; y < cubes.GetLength(1); y++){

                Pathfinder.Node n = new Pathfinder.Node();

                n.position = cubes[x,y].transform.position;
                n.moveCost = cubes[x,y].isSolid ? 9999 : 1;
                
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

                if(neighbor1 != null) n.neighbors.Add( neighbor1 );
                if(neighbor2 != null) n.neighbors.Add( neighbor2 );
                if(neighbor3 != null) n.neighbors.Add( neighbor3 );
                if(neighbor4 != null) n.neighbors.Add( neighbor4 );

            }
        }

        Pathfinder.Node start = nodes[
            (int)Random.Range(0, nodes.GetLength(0)),
            (int)Random.Range(0, nodes.GetLength(1))
        ];
        Pathfinder.Node end = nodes[
            (int)Random.Range(0, nodes.GetLength(0)),
            (int)Random.Range(0, nodes.GetLength(1))
        ];

        List<Pathfinder.Node> path = Pathfinder.Solve(start, end);        

    }
}
