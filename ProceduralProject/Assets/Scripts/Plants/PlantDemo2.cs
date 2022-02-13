using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
    [System.Serializable]
    public class PlantSettings : ISerializable{

        [HideInInspector]
        public string filename = "";

        [Range(0, 100000000)]
        public int seed = 0;

        [Range(2, 30)]
        public int iterations = 5;

        public BranchingStyle growthStyle = BranchingStyle.AlternateFibonacci;

        [Range(0, 10)]
        public int trunkSegmentsBeforeNodes = 2;

        [Range(1, 10)]
        public int segmentsBetweenNodes = 2;

        [TreeRange(1, 10)]
        public TreeFloat leafSizeMult = new TreeFloat(1, 1);

        [TreeRange(.25f, 5)]
        public TreeFloat leafSizeLimit = new TreeFloat(.25f, .25f);

        [TreeRange(0, 1)]
        public TreeFloat chanceOfLeaf = new TreeFloat(1, 1);

        [TreeRange(0, 1)]
        public TreeFloat leafAlignParent = new TreeFloat(0.5f, 0.5f);

        [TreeRange(-90, 90)]
        public TreeFloat leafCurl = new TreeFloat(0, 20);

        [Range(1, 10)]
        public int leafResolution = 2;

        [TreeRange(0, 1)]
        public TreeFloat chanceOfNewBranch = new TreeFloat(.5f, 0);

        [TreeRange(0, 1)]
        public TreeFloat thickness = new TreeFloat(.2f, 0);

        [TreeRange(0, 2)]
        public TreeFloat length = new TreeFloat(1f, .5f);

        [TreeRange(0f, 1f)]
        public TreeFloat parentAlign = new TreeFloat(0.25f, 0.75f);

        [TreeRange(-1f, 1f)]
        public TreeFloat turnUpwards = new TreeFloat(0, 0.5f);

        [TreeRange(-45, 45)]
        public TreeFloat turnDegrees = new TreeFloat(0, 0);

        [TreeRange(-45, 45)]
        public TreeFloat twistDegrees = new TreeFloat(0, 0);

        [Range(0f, .5f)]
        public float convertSmallBranchesToLeaves = .3f;

        public PlantSettings() {

        }
        public PlantSettings(SerializationInfo info, StreamingContext context) {

            foreach(SerializationEntry serialized in info) {
                System.Reflection.FieldInfo property = this.GetType().GetField(serialized.Name);

                if(property != null && serialized.ObjectType == property.FieldType) {
                    object value = info.GetValue(serialized.Name, serialized.ObjectType);
                    property.SetValue(this, value);
                }
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            foreach (System.Reflection.FieldInfo prop in this.GetType().GetFields()) {
                info.AddValue(prop.Name, prop.GetValue(this));
            }
        }
    }
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

        // 2. spawn the instances
        Grow(NodeType.Branch, meshBuilder, Vector3.zero, Quaternion.identity, 1, plantSettings.iterations);

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

            bool nodeHere = (allowedSplit && num >= plantSettings.trunkSegmentsBeforeNodes && (num - plantSettings.trunkSegmentsBeforeNodes) % plantSettings.segmentsBetweenNodes == 0);
            if (nodeHere) nodeSpin += GetSpin();

            ++num;

            float thicc = plantSettings.thickness.Lerp(percentAtBase);
            float lngth = plantSettings.length.Lerp(percentAtBase);

            // find the end of the branch:
            Vector3 endPoint = xform.MultiplyPoint(new Vector3(0, lngth, 0));

            //if ((pos - endPoint).sqrMagnitude < plantSettings.convertSmallBranchesToLeaves * plantSettings.convertSmallBranchesToLeaves) {

            // make a cube mesh, add to list:
            meshBuilder.AddMesh(MeshTools.MakeCube(thicc, lngth, thicc), xform, (int)NodeType.Branch);

            // calculate next branch's rotation:

            // which way would this branch naturally grow?
            // it inherits its parent's rot, and then turns / twists: 
            Quaternion newRot = rot * Quaternion.Euler(plantSettings.turnDegrees.Lerp(percentAtBase), plantSettings.twistDegrees.Lerp(percentAtBase), 0);

            // create a version of "up" that is spun to match the current branch growth
            Quaternion spunUp = Quaternion.Euler(0, newRot.eulerAngles.y, 0);

            // angle the growth up or down:
            newRot = Quaternion.LerpUnclamped(newRot, spunUp, plantSettings.turnUpwards.Lerp(percentAtBase));

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
                NodeType whatGrowNext = NodeType.Leaf;
                float lean = Mathf.Lerp(90, 0, plantSettings.parentAlign.Lerp(percentAtBase));
                for (int i = 0; i < howMany; i++) {

                    float s = RandBell(.5f, .95f);
                    if (Rand() <= plantSettings.chanceOfNewBranch.Lerp(percentAtBase)) {
                        whatGrowNext = NodeType.Branch;
                    }
                    if (scale * s < plantSettings.convertSmallBranchesToLeaves) {
                        whatGrowNext = NodeType.Leaf;
                    }

                    if (whatGrowNext == NodeType.Leaf) lean = 0;
                    float spin = nodeSpin + degreesBetweenSiblings * i;
                    Quaternion branchDir = rot * Quaternion.Euler(lean, spin, 0);



                    Grow(whatGrowNext, meshBuilder, pos, branchDir, scale * s, max, num - 1, nodeSpin + spinChildren, false);
                }
            }
        }
        

        if (type == NodeType.Leaf) {

            if (Rand() < plantSettings.chanceOfLeaf.Lerp(percentAtBase)) {

                // rotate the leaf:
                float lean = Mathf.Lerp(90, 0, plantSettings.leafAlignParent.Lerp(percentAtBase));
                Quaternion leafRot = rot * Quaternion.Euler(lean, 0, 0);

                // offset the leafpos by the thickness of the branch:
                Vector3 leafPos = xform.MultiplyPoint(new Vector3(0,0, plantSettings.thickness.Lerp(percentAtBase)));

                float size = plantSettings.leafSizeMult.Lerp(percentAtBase) * scale;
                float maxSize = plantSettings.leafSizeLimit.Lerp(percentAtBase);
                if (size > maxSize) size = maxSize;

                //float align = Vector3.Dot(leafRot * Vector3.back, Vector3.up) < 1 ? 1 : -1;
                Matrix4x4 leafXform = Matrix4x4.TRS(leafPos, leafRot, Vector3.one * size);
                Vector3 worldUp = leafXform.inverse.MultiplyVector(Vector3.up);

                meshBuilder.AddMesh(MeshTools.MakeLeaf(1, plantSettings.leafResolution, plantSettings.leafCurl.Lerp(percentAtBase)), leafXform, (int)NodeType.Leaf);
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
