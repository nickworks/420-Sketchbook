using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[CustomEditor(typeof(PlantDemo2))]
public class PlantInspector : Editor {

    string[] filenames = new string[0];
    int currentPresetNum = -1;

    static int toolbarIndex = 0;

    private void OnEnable() {
        GetFileNames();
    }
    public override void OnInspectorGUI() {

        var plant = (target as PlantDemo2);
        var obj = new SerializedObject(plant);
        var prop = obj.FindProperty("plantSettings");

        string[] options = new string[] { "Preset", "Plant Settings", "Branch Growth", "Leaf Growth" };
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, options);
        
        switch (toolbarIndex) {
            case 0: // preset

                GUILayout.BeginVertical();
                EditorGUILayout.Space(20);
                EditorGUILayout.LabelField("Loaded preset:");
                EditorGUI.BeginChangeCheck();
                currentPresetNum = EditorGUILayout.Popup(currentPresetNum, filenames);
                if (EditorGUI.EndChangeCheck()) {
                    LoadPlant(currentPresetNum);
                }
                EditorGUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New")) NewPlant();
                if (GUILayout.Button("Save")) Save();
                if (GUILayout.Button("Save As ...")) SaveAs();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(20);
                GUILayout.EndVertical();
                break;
            case 1: // plant settings                
                EditorGUILayout.Space(20);
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("seed"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("iterations"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("growthStyle"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("trunkSegmentsBeforeNodes"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("segmentsBetweenNodes"));
                EditorGUILayout.Space(20);
                break;
            case 2: // branch growth:
                EditorGUILayout.Space(20);
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("chanceOfNewBranch"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("thickness"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("length"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("parentAlign"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("turnUpwards"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("turnDegrees"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("twistDegrees"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("convertSmallBranchesToLeaves"));
                EditorGUILayout.Space(20);
                break;
            case 3: // leaf growth:
                EditorGUILayout.Space(20);
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("leafSizeMult"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("leafSizeLimit"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("chanceOfLeaf"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("leafAlignParent"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("leafCurl"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("leafResolution"));
                EditorGUILayout.Space(20);
                break;
        }
        //GUILayout.EndArea();
        obj.ApplyModifiedProperties();
    }
    private void ShowPropertyField(SerializedProperty prop, string name) {

        
    }

    private void LoadPlant(int index) {
        if (index < 0) return;
        if (index >= filenames.Length) return;

        try {
            Stream s = File.OpenRead(Application.persistentDataPath + Path.DirectorySeparatorChar + filenames[index]);
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(s);
            s.Close();

            PlantDemo2.PlantSettings settings = (PlantDemo2.PlantSettings)obj;
            settings.filename = filenames[index];

            (target as PlantDemo2).Build(settings);


            //Debug.Log(filenames[index] + " loaded");

        } catch(System.Exception e) {
            currentPresetNum = -1;
            Debug.LogError("File couldn't be opened.");
        }
    }
    private void NewPlant() {
        currentPresetNum = -1;
        (target as PlantDemo2).Build(new PlantDemo2.PlantSettings());
    }
    private void Save() {
        if(currentPresetNum < 0 || currentPresetNum >= filenames.Length) {
            SaveAs();
        } else {
            string path = Application.persistentDataPath + Path.DirectorySeparatorChar + filenames[currentPresetNum];
            SaveTo(path);
        }
    }
    private void SaveAs() {
        string path = EditorUtility.SaveFilePanel("Save Tree", Application.persistentDataPath, "", "tree");
        if(path.Length != 0) SaveTo(path);
    }
    private void SaveTo(string path) {
        string filename = Path.GetFileName(path);
        if (path.Length != 0) {

            var plant = (target as PlantDemo2);
            plant.plantSettings.filename = filename;

            Stream s = File.OpenWrite(path);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, plant.plantSettings);
            s.Close();

            //Debug.Log("saved to " + filename);
        }
        GetFileNames();
    }
    private void GetFileNames() {

        var plant = (target as PlantDemo2);
        string[] longnames = Directory.GetFiles(Application.persistentDataPath, "*.tree");

        filenames = new string[longnames.Length];
        for(int i = 0; i < longnames.Length; i++) {
            filenames[i] = Path.GetFileName(longnames[i]);
            if (filenames[i] == plant.plantSettings.filename) currentPresetNum = i;
        }
    }
}