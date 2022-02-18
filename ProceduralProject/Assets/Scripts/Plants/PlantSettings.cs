using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;


[System.Serializable]
public class PlantSettings : ISerializable {

    [System.Serializable]
    class SerializedKeyFrame : ISerializable {

        float time;
        float value;
        float inTangent;
        float outTangent;
        float inWeight;
        float outWeight;
        int weightedMode;


        public SerializedKeyFrame(Keyframe frame) {
            time = frame.time;
            value = frame.value;
            inTangent = frame.inTangent;
            outTangent = frame.outTangent;
            inWeight = frame.inWeight;
            outWeight = frame.outWeight;
            weightedMode = (int)frame.weightedMode;
        }
        public Keyframe MakeKeyframe() {
            var kf = new Keyframe(time, value, inTangent, outTangent, inWeight, outWeight);
            kf.weightedMode = (WeightedMode)weightedMode;
            return kf;
        }
        public SerializedKeyFrame(SerializationInfo info, StreamingContext context) {
            time = (float)info.GetValue("time", typeof(float));
            value = (float)info.GetValue("value", typeof(float));
            inTangent = (float)info.GetValue("inTangent", typeof(float));
            inWeight = (float)info.GetValue("inWeight", typeof(float));
            outTangent = (float)info.GetValue("outTangent", typeof(float));
            outWeight = (float)info.GetValue("outWeight", typeof(float));
            weightedMode = (int)info.GetValue("weightedMode", typeof(int));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("time", time);
            info.AddValue("value", value);
            info.AddValue("inTangent", inTangent);
            info.AddValue("inWeight", inWeight);
            info.AddValue("outTangent", outTangent);
            info.AddValue("outWeight", outWeight);
            info.AddValue("weightedMode", weightedMode);
        }
    }
    [System.Serializable]
    class SerializedCurve : ISerializable {

        SerializedKeyFrame[] frames;
        int postWrapMode;
        int preWrapMode;


        public SerializedCurve(AnimationCurve curve) {
            postWrapMode = (int)curve.postWrapMode;
            preWrapMode = (int)curve.preWrapMode;
            frames = new SerializedKeyFrame[curve.keys.Length];

            for (int i = 0; i < curve.keys.Length; i++) {
                frames[i] = new SerializedKeyFrame(curve.keys[i]);
            }
        }
        public AnimationCurve MakeCurve() {


            Keyframe[] keys = new Keyframe[frames.Length];
            for (int i = 0; i < keys.Length; i++) {
                keys[i] = frames[i].MakeKeyframe();
            }
            AnimationCurve curve = new AnimationCurve(keys);
            curve.preWrapMode = (WrapMode)preWrapMode;
            curve.postWrapMode = (WrapMode)postWrapMode;

            return curve;
        }
        public SerializedCurve(SerializationInfo info, StreamingContext context) {
            frames = (SerializedKeyFrame[])info.GetValue("frames", typeof(SerializedKeyFrame[]));
            postWrapMode = (int)info.GetValue("postWrapMode", typeof(int));
            preWrapMode = (int)info.GetValue("preWrapMode", typeof(int));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("postWrapMode", postWrapMode);
            info.AddValue("preWrapMode", preWrapMode);
            info.AddValue("frames", frames);
        }
    }


    [HideInInspector]
    public string filename = "";

    public bool isDeprecated = false;

    #region Plant Settings
    [Range(0, 100000000)]
    public int seed = 0;

    [Range(2, 30)]
    public int iterations = 5;

    public BranchingStyle growthStyle = BranchingStyle.AlternateFibonacci;

    [Range(0, 10)]
    public int trunkSegmentsBeforeNodes = 2;

    [Range(1, 10)]
    public int segmentsBetweenNodes = 2;

    #endregion
    #region Leaf Settings

    public bool hideLeaves = false;

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

    #endregion

    #region Branch Settings

    [Range(3,12)]
    public int sides = 4;

    [TreeRange(0, 1)]
    public TreeFloat chanceOfNewBranch = new TreeFloat(.5f, 0);

    [TreeRange(0, 1)]
    public TreeFloat thickness = new TreeFloat(.2f, 0);

    public AnimationCurve thicknessCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });

    [TreeRange(.01f, 2)]
    public TreeFloat length = new TreeFloat(1f, .5f);

    [TreeRange(0f, 1f)]
    public TreeFloat parentAlign = new TreeFloat(0.25f, 0.75f);

    [TreeRange(-1f, 1f)]
    public TreeFloat turnUpwards = new TreeFloat(0, 0.5f);

    [TreeRange(-45, 45)]
    public TreeFloat turnDegrees = new TreeFloat(0, 0);

    [TreeRange(-45, 45)]
    public TreeFloat twistDegrees = new TreeFloat(0, 0);

    [Tooltip("Branches that are smaller than this are removed. Increase the value to prevent run-away growth.")]
    [Range(.03f, .5f)]
    public float pruneSmallerThan = .3f;

    [Tooltip("A % chance of replacing pruned branches with leaves.")]
    [TreeRange(0, 1)]
    public TreeFloat graftLeafChance = new TreeFloat(0, 1);

    #endregion

    public PlantSettings() {

    }
    public PlantSettings(SerializationInfo info, StreamingContext context) {

        bool shouldBeDeprecated = false;

        // foreach serialized value
        foreach (SerializationEntry serialized in info) {

            // find the corresponding Field within PlantSettings
            System.Reflection.FieldInfo property = this.GetType().GetField(serialized.Name);

            if (property != null){

                // if the serialized object is a curve
                if (serialized.ObjectType == typeof(SerializedCurve)) {
                    // get the serialized value
                    SerializedCurve curve = (SerializedCurve)info.GetValue(serialized.Name, serialized.ObjectType);
                    property.SetValue(this, curve.MakeCurve());
                }
                // if a field exists with mataching type
                else if (serialized.ObjectType == property.FieldType) {

                    // skip some values
                    if (serialized.Name == "filename") continue;
                    if (serialized.Name == "deprecated") continue;

                    // get the serialized value
                    object value = info.GetValue(serialized.Name, serialized.ObjectType);

                    // set the corresponding Field to the serialized value
                    property.SetValue(this, value);

                } 

            } else {



                // deprecated names for backwards compatibility

                if(serialized.Name == "convertSmallBranchesToLeaves") {
                    
                    Debug.LogWarning("This file is deprecated...");

                    this.pruneSmallerThan = (float) serialized.Value;
                    shouldBeDeprecated = true;
                }

            }
        }
        this.isDeprecated = shouldBeDeprecated;
    }
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
        foreach (System.Reflection.FieldInfo prop in this.GetType().GetFields()) {

            // don't serialized the following values:
            if (prop.Name == "filename") continue;
            if (prop.Name == "deprecated") continue;

            if (prop.FieldType == typeof(AnimationCurve)) {
                AnimationCurve curve = (AnimationCurve)prop.GetValue(this);

                SerializedCurve scurve = new SerializedCurve(curve);
                info.AddValue(prop.Name, scurve);


            } else {
                info.AddValue(prop.Name, prop.GetValue(this));
            }
        }
    }
}