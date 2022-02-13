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

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantDemo2 : MonoBehaviour
{
    [Header("Plant Settings")]
    [Range(0, 100000000)]
    public int seed = 0;

    [Range(2, 30)]
    public int iterations = 5;


    public BranchingStyle growthStyle = BranchingStyle.AlternateFibonacci;
    
    [Range(0, 10)]
    public int trunkSegmentsBeforeNodes = 2;

    [Range(1, 10)]
    public int segmentsBetweenNodes = 2;
    
    [Header("Leaf Settings")]
    [TreeRange(1, 10)]
    public TreeFloat leafSizeMult = new TreeFloat(1, 1);

    [TreeRange(.25f, 5)]
    public TreeFloat leafSizeLimit = new TreeFloat(.25f, .25f);

    [TreeRange(0, 1)]
    public TreeFloat chanceOfLeaf = new TreeFloat(1, 1);

    [TreeRange(0, 1)]
    public TreeFloat leafAlignParent;

    [TreeRange(-90, 90)]
    public TreeFloat leafCurl;

    [Range(1, 10)]
    public int leafResolution = 2;

    [Header("Branch Settings")]


    [TreeRange(0, 1)]
    public TreeFloat chanceOfNewBranch = new TreeFloat(.5f, 0);

    [TreeRange(0, 1)]
    public TreeFloat thickness = new TreeFloat(.2f, 0);

    [TreeRange(0, 2)]
    public TreeFloat length = new TreeFloat(1f, .5f);

    [TreeRange(0f, 1f)]
    public TreeFloat parentAlign;

    [TreeRange(-1f, 1f)]
    public TreeFloat turnUpwards;

    [TreeRange(-45, 45)]
    public TreeFloat turnDegrees;

    [TreeRange(-45, 45)]
    public TreeFloat twistDegrees;

    [Range(0f, .5f)]
    public float convertSmallBranchesToLeaves = .3f;

    private System.Random randGenerator;

    void Start()
    {
        Build();
    }
    private void OnValidate() {
        Build();
    }

    void Build() {

        // get ref to component:
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter) return;

        // seed the random number generator:
        randGenerator = new System.Random(seed);

        // 1. making storage for instances:
        MeshTools.MeshBuilder meshBuilder = new MeshTools.MeshBuilder();

        // 2. spawn the instances
        Grow(NodeType.Branch, meshBuilder, Vector3.zero, Quaternion.identity, 1, iterations);

        // 3. combining the instances together:
        meshFilter.mesh = meshBuilder.CombineAll();

    }
    void Grow(NodeType type, MeshTools.MeshBuilder meshBuilder, Vector3 pos, Quaternion rot, float scale = 1, int max = 1, int num = 0, float nodeSpin = 0, bool allowedSplit = true) {

        if (num < 0) num = 0;
        if (num >= max) return; // stop recursion!
        // add to num, calc %:
        float percentAtBase = num / (float) max;
        Matrix4x4 xform = Matrix4x4.TRS(pos, rot, Vector3.one * scale);

        if (type == NodeType.Branch) {

            bool nodeHere = (allowedSplit && num >= trunkSegmentsBeforeNodes && (num - trunkSegmentsBeforeNodes) % segmentsBetweenNodes == 0);
            if (nodeHere) nodeSpin += GetSpin();

            ++num;

            float thicc = thickness.Lerp(percentAtBase);
            float lngth = length.Lerp(percentAtBase);

            // find the end of the branch:
            Vector3 endPoint = xform.MultiplyPoint(new Vector3(0, lngth, 0));

            if ((pos - endPoint).sqrMagnitude < convertSmallBranchesToLeaves * convertSmallBranchesToLeaves) {

                // grow a leaf instead...
                type = NodeType.Leaf;

            } else {


                // make a cube mesh, add to list:
                meshBuilder.AddMesh(MeshTools.MakeCube(thicc, lngth, thicc), xform, (int)NodeType.Branch);

                // calculate next branch's rotation:

                // which way would this branch naturally grow?
                // it inherits its parent's rot, and then turns / twists: 
                Quaternion newRot = rot * Quaternion.Euler(turnDegrees.Lerp(percentAtBase), twistDegrees.Lerp(percentAtBase), 0);


                // create a version of "up" that is spun to match the current branch growth
                Quaternion spunUp = Quaternion.Euler(0, newRot.eulerAngles.y, 0);



                // angle the growth up or down:
                newRot = Quaternion.LerpUnclamped(newRot, spunUp, turnUpwards.Lerp(percentAtBase));

                Grow(
                    NodeType.Branch,
                    meshBuilder,
                    endPoint,
                    newRot,
                    scale * RandBell(.85f, .95f),
                    max,
                    num,
                    nodeSpin);


                if (nodeHere) {

                    int howMany = 0;
                    float degreesBetweenSiblings = 0;
                    float spinChildren = 0;

                    switch (growthStyle) {
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
                    NodeType whatGrowNext = NodeType.Leaf;
                    float lean = Mathf.Lerp(90, 0, parentAlign.Lerp(percentAtBase));
                    for (int i = 0; i < howMany; i++) {

                        if (Rand() <= chanceOfNewBranch.Lerp(percentAtBase)) {
                            whatGrowNext = NodeType.Branch;
                        }

                        if (whatGrowNext == NodeType.Leaf) lean = 0;
                        float spin = nodeSpin + degreesBetweenSiblings * i;
                        Quaternion branchDir = rot * Quaternion.Euler(lean, spin, 0);
                        float s = RandBell(.5f, .95f);
                        Grow(whatGrowNext, meshBuilder, pos, branchDir, scale * s, max, num - 1, nodeSpin + spinChildren, false);
                    }
                }
            }
        }

        if (type == NodeType.Leaf) {

            if (Rand() < chanceOfLeaf.Lerp(percentAtBase)) {

                // rotate the leaf:
                float lean = Mathf.Lerp(90, 0, leafAlignParent.Lerp(percentAtBase));
                Quaternion leafRot = rot * Quaternion.Euler(lean, 0, 0);

                // offset the leafpos by the thickness of the branch:
                Vector3 leafPos = xform.MultiplyPoint(new Vector3(0,0,thickness.Lerp(percentAtBase)));

                float size = leafSizeMult.Lerp(percentAtBase) * scale;
                float maxSize = leafSizeLimit.Lerp(percentAtBase);
                if (size > maxSize) size = maxSize;

                //float align = Vector3.Dot(leafRot * Vector3.back, Vector3.up) < 1 ? 1 : -1;
                Matrix4x4 leafXform = Matrix4x4.TRS(leafPos, leafRot, Vector3.one * size);
                Vector3 worldUp = leafXform.inverse.MultiplyVector(Vector3.up);


                meshBuilder.AddMesh(MeshTools.MakeLeaf(1, leafResolution, leafCurl.Lerp(percentAtBase)), leafXform, (int)NodeType.Leaf);
            }
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

        switch (growthStyle) {
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
