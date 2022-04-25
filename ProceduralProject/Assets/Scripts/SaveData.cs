using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

enum Location {
    Home,
    Castle,
    Cliffs
}


[Serializable]
public class SaveData : ISerializable
{
    public float playerHealth;
    public int playerLocation;
    public string playerName;

    public SaveData(){

    }

    // Deserializes this object:
    public SaveData(SerializationInfo info, StreamingContext context){
        playerHealth = info.GetSingle("playerHealth");
        playerLocation = info.GetInt32("playerHealth");
        playerName = info.GetString("playerName");
    }

    // Serializes this object:
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("playerHealth", playerHealth);
        info.AddValue("playerLocation", playerLocation);
        info.AddValue("playerName", playerName);
    }
}
