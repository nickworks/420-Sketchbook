using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawner : MonoBehaviour
{

    public Room prefabRoom;

    [Range(4, 15)]
    public int dungeonSize = 10;

    [Range(1, 10)]
    public int spaceBetweenRooms = 5;

    void Start()
    {

        // spawn a DungeonLayout
        DungeonLayout dungeon = new DungeonLayout();
        dungeon.Generate(dungeonSize);

        int[,] rooms = dungeon.GetRooms();

        // loop through rooms, spawn prefabs
        for(int x = 0; x < rooms.GetLength(0); x++) {
            for (int z = 0; z < rooms.GetLength(1); z++) {

                if (rooms[x, z] == 0) continue; // skip room

                Vector3 pos = new Vector3(x, 0, z) * spaceBetweenRooms;
                Room newRoom = Instantiate(prefabRoom, pos, Quaternion.identity);

                newRoom.InitRoom((RoomType)rooms[x, z]);

            }
        }

    } // ends Start()

} // ends DungeonSpawner
