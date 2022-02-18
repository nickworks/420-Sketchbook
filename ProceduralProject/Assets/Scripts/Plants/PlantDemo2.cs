using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public enum BranchingStyle {
    Random,
    Opposite,
    AlternateDistichous,
    AlternateFibonacci,
    WhorledDecussate,
    WhorledTricussate
}
public enum NodeType {
    None,
    Branch,
    Leaf,
    Flower
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantDemo2 : MonoBehaviour
{
    
    public PlantSettings plantSettings = new PlantSettings();
    private System.Random randGenerator;


    void Start()
    {
        Build();
    }
    private void OnValidate() {
        Build();
    }
    public void Build(PlantSettings settings) {
        plantSettings = settings;
        Build();
    }
    private void Build() {

        // get ref to component:
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter) return;

        // seed the random number generator:
        randGenerator = new System.Random(plantSettings.seed);

        // 1. making storage for instances:
        MeshTools.MeshBuilder meshBuilder = new MeshTools.MeshBuilder();
        meshBuilder.Poke((int)NodeType.Branch);
        meshBuilder.Poke((int)NodeType.Leaf);
        

        // 2. spawn the instances
        GrowFromSeed(meshBuilder);

        // 3. combining the instances together:
        meshFilter.mesh = meshBuilder.CombineAll();

    }
    class RecursionCounter {
        
        public int maxSteps { get; private set; }
        public int currentStep { get; private set; }    
        public int numOfBranches { get; private set; }

        public RecursionCounter(int iterations) {
            maxSteps = iterations;
            currentStep = 0;
            numOfBranches = 0;

        }
        public void Grow() {
            currentStep++;
        }
        public RecursionCounter Branch() {

            return new RecursionCounter(maxSteps) {
                numOfBranches = numOfBranches + 1,
                currentStep = currentStep
            };

        }
        public float PercentAtBase() {
            return currentStep / (float) maxSteps;
        }

        internal bool HasExpired() {
            return (currentStep > maxSteps);
        }
    }
    void GrowFromSeed(MeshTools.MeshBuilder meshBuiler) {

        GrowBranch(new RecursionCounter(plantSettings.iterations), meshBuiler, Vector3.zero, Quaternion.identity, 1);

    }
    void GrowBranch(RecursionCounter counter, MeshTools.MeshBuilder meshBuilder, Vector3 pos, Quaternion rot, float scale) {

        // set the branch thickness:
        
        MeshFromSpline.Settings geometrySettings = new MeshFromSpline.Settings();
        geometrySettings.sides = plantSettings.sides;
        geometrySettings.radiusStart = plantSettings.thickness.Lerp(counter.PercentAtBase()) * scale;
        geometrySettings.radiusEnd = plantSettings.thickness.atTop * scale;
        geometrySettings.radiusCurve = plantSettings.thicknessCurve;

        // make the spline using recursion:
        Vector3[] spline = GrowBranchSpline(counter, meshBuilder, pos, rot, scale, 1);

        // make a mesh from the spline:
        Mesh mesh = MeshFromSpline.BuildMesh(spline, geometrySettings);
        meshBuilder.AddMesh(mesh, Matrix4x4.identity, (int)NodeType.Branch);
        
    }
    Vector3[] GrowBranchSpline(RecursionCounter counter, MeshTools.MeshBuilder meshBuilder, Vector3 pos, Quaternion rot, float scale, float nodeSpin = 0, bool allowedSplit = true) {

        Matrix4x4 xform = Matrix4x4.TRS(pos, rot, Vector3.one * scale);

        List<Vector3> spline = new List<Vector3>() {
            xform.MultiplyPoint(Vector3.zero)
        };
        
        for (;;) {

            if (counter.HasExpired()) break;

            bool aboveTrunk = counter.currentStep >= plantSettings.trunkSegmentsBeforeNodes;
            bool isTimeForNode = (counter.currentStep - plantSettings.trunkSegmentsBeforeNodes) % plantSettings.segmentsBetweenNodes == 0;

            bool nodeHere = (allowedSplit && aboveTrunk && isTimeForNode);
            if (nodeHere) nodeSpin += GetSpin();

            float percentAtBase = counter.PercentAtBase();
            counter.Grow();

            float thicc = plantSettings.thickness.Lerp(percentAtBase);
            float lngth = plantSettings.length.Lerp(percentAtBase);
            
            xform = Matrix4x4.TRS(pos, rot, Vector3.one * scale);
            
            // find the end of the branch:
            Vector3 endPoint = xform.MultiplyPoint(new Vector3(0, lngth, 0));

            spline.Add(endPoint);

            // make node

            
            if (nodeHere) {

                int howMany = 0;
                float degreesBetweenSiblings = 0;
                float spinChildren = 0;

                switch (plantSettings.growthStyle) {
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
                NodeType whatGrowNext = NodeType.None;

                if (Rand() <= plantSettings.chanceOfNewBranch.Lerp(percentAtBase)) {
                    whatGrowNext = NodeType.Branch;
                } else if(Rand() <= plantSettings.chanceOfLeaf.Lerp(percentAtBase)) {
                    whatGrowNext = NodeType.Leaf;
                }


                float lean = Mathf.Lerp(90, 0, plantSettings.parentAlign.Lerp(percentAtBase));
                for (int i = 0; i < howMany; i++) {

                    float s = RandBell(.5f, .95f);

                    if (scale * s < plantSettings.pruneSmallerThan) {
                        if (Rand() <= plantSettings.graftLeafChance.Lerp(percentAtBase)) {
                            whatGrowNext = NodeType.Leaf;
                        } else {
                            whatGrowNext = NodeType.None;
                        }
                    }

                    if (whatGrowNext == NodeType.Leaf) lean = 0;
                    float spin = nodeSpin + degreesBetweenSiblings * i;
                    Quaternion branchDir = rot * Quaternion.Euler(lean, spin, 0);

                    if (whatGrowNext == NodeType.Branch) {
                        GrowBranch(counter.Branch(), meshBuilder, endPoint, branchDir, scale * s);
                    }
                    else if(whatGrowNext == NodeType.Leaf) {
                        GrowLeaf(counter, meshBuilder, endPoint, branchDir, scale * s);
                    }

                }
            }

            // continue branch:

            float pitch = plantSettings.turnDegrees.Lerp(percentAtBase);
            float yaw = plantSettings.twistDegrees.Lerp(percentAtBase);

            Quaternion newRot = rot * Quaternion.Euler(pitch, yaw, 0);
            Quaternion spunUp = Quaternion.Euler(0, newRot.eulerAngles.y, 0);
            rot = Quaternion.LerpUnclamped(newRot, spunUp, plantSettings.turnUpwards.Lerp(percentAtBase));
            pos = endPoint;
            scale *= RandBell(.85f, .95f);
        }


        // calculate next branch's rotation:

        // which way would this branch naturally grow?
        // it inherits its parent's rot, and then turns / twists: 

        /*
        */
        return spline.ToArray();
    }
    void GrowLeaf(RecursionCounter counter, MeshTools.MeshBuilder meshBuilder, Vector3 pos, Quaternion rot, float scale = 1) {

        float percentAtBase = counter.PercentAtBase();
        
        if (Rand() < plantSettings.chanceOfLeaf.Lerp(percentAtBase)) {

            // this line should be called after the last Rand()
            if (plantSettings.hideLeaves) return;

            // rotate the leaf:
            float lean = Mathf.Lerp(90, 0, plantSettings.leafAlignParent.Lerp(percentAtBase));
            Quaternion leafRot = rot * Quaternion.Euler(lean, 0, 0);

            Matrix4x4 xform = Matrix4x4.TRS(pos, rot, Vector3.one * scale);
            // offset the leafpos by the thickness of the branch:
            Vector3 leafPos = xform.MultiplyPoint(new Vector3(0, 0, plantSettings.thickness.Lerp(percentAtBase)));

            float size = plantSettings.leafSizeMult.Lerp(percentAtBase) * scale;
            float maxSize = plantSettings.leafSizeLimit.Lerp(percentAtBase);
            if (size > maxSize) size = maxSize;

            Matrix4x4 leafXform = Matrix4x4.TRS(leafPos, leafRot, Vector3.one * size);
            Vector3 worldUp = leafXform.inverse.MultiplyVector(Vector3.up);

            Mesh mesh = MeshTools.MakeLeaf(1, plantSettings.leafResolution, plantSettings.leafCurl.Lerp(percentAtBase));

            meshBuilder.AddMesh(mesh, leafXform, (int)NodeType.Leaf);
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

        switch (plantSettings.growthStyle) {
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
