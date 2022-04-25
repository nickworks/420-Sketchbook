using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DemoHUD : MonoBehaviour
{
    private Slider slider;
    void Start()
    {
        slider = GetComponentInChildren<Slider>();

        if(PlayerPrefs.HasKey("the-volume")){
            slider.value = PlayerPrefs.GetFloat("the-volume", 0);
        }
        slider.onValueChanged.AddListener(OnSliderChange);
    }

    public void OnSliderChange(float value){
        PlayerPrefs.SetFloat("the-volume", value);
        print($"value saved: {value}");
    }

    public void OnButtonSave(){
        
        SaveData data = new SaveData();
        data.playerHealth = 42;

        FileStream stream = File.OpenWrite("savegame.dagd420");

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(stream, data);

        stream.Close();

    }
    public void OnButtonLoad(){
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = null;
        try {
            stream = File.OpenRead("savegame.dagd420");
        } catch (System.Exception e){
            return;
        }

        SaveData data = null;
        try {
            data = (SaveData) bf.Deserialize(stream);
        } catch (System.Exception e){
            
        }
        stream.Close();

        if(data != null) print(data.playerHealth);

    }

}
