using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TreeBuilder : MonoBehaviour
{
    public Vector3[] points = new Vector3[] {
        new Vector3(0,0,0),
        new Vector3(0,2,0),
        new Vector3(0,6,3)
    };

    public MeshFromLine.Settings settings;

    private void Start() {
        Build();
    }
    private void OnValidate() {
        Build();
    }
    public void Build() {

        GetComponent<MeshFilter>().mesh = MeshFromLine.BuildMesh(points,settings);
    }
}
