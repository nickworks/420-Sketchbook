using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BranchingStyle {
    Random,
    Opposite,
    AlternateDistichous,
    AlternateFibonacci,
    WhorledDecussate,
    WhorledTricussate
}
public enum NodeType {
    Branch,
    Leaf,
    Flower
}
public class InstanceCollection {
    List<CombineInstance> branchInstances = new List<CombineInstance>();
    List<CombineInstance> leafInstances = new List<CombineInstance>();
    public void AddBranch(Mesh mesh, Matrix4x4 xform) {
        branchInstances.Add(new CombineInstance() { mesh = mesh, transform = xform });
    }
    public void AddLeaf(Mesh mesh, Matrix4x4 xform) {
        leafInstances.Add(new CombineInstance() { mesh = mesh, transform = xform });
    }
    /// <summary>
    /// Why go through all of the trouble? To have multiple material references, but no more than we need.
    /// </summary>
    /// <returns></returns>
    public Mesh MakeMultiMesh() {
        
        Mesh branchMesh = new Mesh();
        branchMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        branchMesh.CombineMeshes(branchInstances.ToArray());


        Mesh leafMesh = new Mesh();
        leafMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        leafMesh.CombineMeshes(leafInstances.ToArray());


        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        finalMesh.CombineMeshes(new CombineInstance[] {
            new CombineInstance(){ mesh = branchMesh, transform = Matrix4x4.identity },
            new CombineInstance(){ mesh = leafMesh, transform = Matrix4x4.identity }
        }, false);

        return finalMesh;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantDemo2 : MonoBehaviour
{

    [Range(0, 100000000)]
    public int seed = 0;

    [Range(2, 30)]
    public int iterations = 5;

    [Range(0, 45)]
    public float turnDegrees = 10;

    [Range(0, 45)]
    public float twistDegrees = 10;

    [Range(.1f, .5f)]
    public float limitSmallGeometry = .3f;

    [Range(1, 10)]
    public float leafSize = 1;

    public BranchingStyle branching;

    [Range(1, 10)]
    public int nodeDistance = 2;

    [Range(0, 10)]
    public int trunkNodes = 2;

    [Range(0, 1)]
    public float chanceOfLeaf;

    public AnimationCurve curveThickness;
    public AnimationCurve curveAlignWithParent;
    public AnimationCurve curveTurnUpwards;

    private System.Random randGenerator;

    void Start()
    {
        Build();
    }
    private void OnValidate() {
        Build();
    }

    void Build() {

        randGenerator = new System.Random(seed);

        // 1. making storage for instances:
        InstanceCollection instances = new InstanceCollection();

        // 2. spawn the instances

        Grow(NodeType.Branch, instances, Vector3.zero, Quaternion.identity, Vector3.one, iterations);

        // 3. combining the instances together:

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter) meshFilter.mesh = instances.MakeMultiMesh();

    }
    void Grow(NodeType type, InstanceCollection instances, Vector3 pos, Quaternion rot, Vector3 scale, int max, int num = 0, float nodeSpin = 0, bool allowedSplit = true) {

        if (num < 0) num = 0;
        if (num >= max) return; // stop recursion!

        if (type == NodeType.Branch) {

            // add to num, calc %:
            float percentAtBase = num / (float) max;
            bool nodeHere = (allowedSplit && num >= trunkNodes && (num-trunkNodes) % nodeDistance == 0);

            ++num;

            float thickness = curveThickness.Evaluate(percentAtBase);

            // make a cube mesh, add to list:
            Matrix4x4 xform = Matrix4x4.TRS(pos, rot, scale);
            instances.AddBranch(MeshTools.MakeCube(thickness, 1, thickness), xform);

        
            // find the end of the branch:
            Vector3 endPoint = xform.MultiplyPoint(new Vector3(0, 1, 0));

            if ((pos - endPoint).sqrMagnitude < limitSmallGeometry * limitSmallGeometry) return; // too small, stop recursion!

            // continue calculating this branch's rotation:
            
            // if it continued, which way would this branch naturally grow?
            // it inherits its parent's rot, and then turns / twists: 
            Quaternion randRot = rot * Quaternion.Euler(turnDegrees, twistDegrees, 0);

            // what rotator represents "up"?
            //Quaternion upRot = Quaternion.RotateTowards(rot, Quaternion.identity);

            // blend between the continued growth and the upwards growth:
            //Quaternion newRot = Quaternion.Lerp(randRot, upRot, turnUpwards);
            float amnt = Mathf.Lerp(0, 90, curveTurnUpwards.Evaluate(percentAtBase));
            Quaternion newRot = Quaternion.RotateTowards(randRot, Quaternion.identity, amnt);

            
            if(nodeHere) nodeSpin += GetSpin();

            Grow(
                NodeType.Branch,
                instances,
                endPoint,
                newRot,
                scale * RandBell(.85f, .95f),
                max,
                num,
                nodeSpin);


            if (nodeHere) {

                NodeType whatGrowNext = NodeType.Branch;
                int howMany = 0;
                float degreesBetweenSiblings = 0;
                float spinChildren = 0;

                switch (branching) {
                    case BranchingStyle.Random:
                        howMany = 1;
                        break;
                    case BranchingStyle.Opposite:
                        howMany = 2;
                        degreesBetweenSiblings = 180;
                        break;
                    case BranchingStyle.AlternateDistichous:
                        howMany = 1;
                        break;
                    case BranchingStyle.AlternateFibonacci:
                        howMany = 1;
                        degreesBetweenSiblings = 180;
                        break;
                    case BranchingStyle.WhorledDecussate:
                        howMany = 2;
                        degreesBetweenSiblings = 180;
                        spinChildren = 90;
                        break;
                    case BranchingStyle.WhorledTricussate:
                        howMany = 3;
                        degreesBetweenSiblings = 120;
                        break;
                }
                float split = Mathf.Lerp(90, 0, curveAlignWithParent.Evaluate(percentAtBase));
                for (int i = 0; i < howMany; i++) {
                    float spin = nodeSpin + degreesBetweenSiblings * i;
                    Quaternion branchDir = rot * Quaternion.Euler(split, spin, 0);
                    float s = RandBell(.5f, .95f);
                    if (RandBool()) {
                        if (Rand() < chanceOfLeaf) {
                            whatGrowNext = NodeType.Leaf;
                        } else {
                            whatGrowNext = NodeType.Flower;
                        }
                    }

                    Grow(whatGrowNext, instances, pos, branchDir, scale * s, max, num - 1, nodeSpin + spinChildren, false);
                }
            }
        }


        if (type == NodeType.Leaf) {


            instances.AddLeaf(MeshTools.MakeLeaf(leafSize), Matrix4x4.TRS(pos, rot, scale));
        }

    }
    
    private float RandBell(float min, float max) {
        min /= 2;
        max /= 2;
        return Rand(min, max) + Rand(min, max);
    }
    private float Rand() {
        return (float)randGenerator.NextDouble();
    }
    private float Rand(float min, float max) {
        return Rand() * (max - min) + min;
    }
    private bool RandBool() {
        return randGenerator.NextDouble() >= .5;
    }
    private float GetSpin() {

        switch (branching) {
            case BranchingStyle.Random:
                return Rand(0, 360);
            case BranchingStyle.Opposite:
                return 0;
            case BranchingStyle.AlternateDistichous:
                return 180;
            case BranchingStyle.AlternateFibonacci:
                return 137.5f;
            case BranchingStyle.WhorledDecussate:
                return 90;
            case BranchingStyle.WhorledTricussate:
                return 60;
        }

        return 0;
    }
}
