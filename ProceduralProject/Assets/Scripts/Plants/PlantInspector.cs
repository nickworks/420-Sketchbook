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
    private void OnEnable() {
        GetFileNames();
    }
    public override void OnInspectorGUI() {

        var plant = (target as PlantDemo2);

        EditorGUI.BeginChangeCheck();
        currentPresetNum = EditorGUILayout.Popup(currentPresetNum, filenames);
        if (EditorGUI.EndChangeCheck()) {
            LoadPlant(currentPresetNum);
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New")) NewPlant();
        if (GUILayout.Button("Save")) Save();
        if (GUILayout.Button("Save As ...")) SaveAs();
        
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }

    private void LoadPlant(int index) {
        if (index < 0) return;
        if (index >= filenames.Length) return;

        try {
            Stream s = File.OpenRead(Application.persistentDataPath + Path.DirectorySeparatorChar + filenames[index]);
            BinaryFormatter bf = new BinaryFormatter();
            object obj = bf.Deserialize(s);
            s.Close();
            (target as PlantDemo2).Build((PlantDemo2.PlantSettings)obj);

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
            Stream s = File.OpenWrite(path);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, (target as PlantDemo2).plantSettings);
            s.Close();

            //Debug.Log("saved to " + filename);
        }
        GetFileNames(filename);
    }
    private void GetFileNames(string setCurrent = "") {

        string[] longnames = Directory.GetFiles(Application.persistentDataPath, "*.tree");

        filenames = new string[longnames.Length];
        for(int i = 0; i < longnames.Length; i++) {
            filenames[i] = Path.GetFileName(longnames[i]);
            if (filenames[i] == setCurrent) currentPresetNum = i;
        }
    }
}