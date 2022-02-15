using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;


[System.Serializable]
public class PlantSettings : ISerializable {

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

    [Tooltip("Branches that are smaller than this are converted to leaves. Turning this up a little is a good way to prevent run-away growth.")]
    [Range(0f, .5f)]
    public float pruneSmallerThan = .3f;

    #endregion

    public PlantSettings() {

    }
    public PlantSettings(SerializationInfo info, StreamingContext context) {

        bool shouldBeDeprecated = false;

        foreach (SerializationEntry serialized in info) {
            System.Reflection.FieldInfo property = this.GetType().GetField(serialized.Name);


            if (property != null && serialized.ObjectType == property.FieldType) {


                if (serialized.Name == "filename") continue;

                object value = info.GetValue(serialized.Name, serialized.ObjectType);
                property.SetValue(this, value);

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

            info.AddValue(prop.Name, prop.GetValue(this));
        }
    }
}