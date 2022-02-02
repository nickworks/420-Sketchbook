using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    Void,       // 0
    Normal,     // 1
    RandomPOI,  // 2
    Merchant,   // 3
    Shrine,     // 4
    QuestGiver, // 5
    Loot,       // 6
    FloorEnter, // 7
    FloorExit   // 8
}


public class DungeonLayout {
    private int lilsPerBig = 5;
    private int res = 0;
    private int hires = 0;
    private int[,] lilrooms;
    private int[,] bigrooms;

    public void Generate(int size) {
        res = size;
        hires = res * lilsPerBig;

        bigrooms = new int[res, res];
        lilrooms = new int[hires, hires];

        WalkRooms(RoomType.FloorEnter, RoomType.FloorExit);
        WalkRooms(RoomType.RandomPOI, RoomType.RandomPOI);
        WalkRooms(RoomType.RandomPOI, RoomType.RandomPOI);
        WalkRooms(RoomType.RandomPOI, RoomType.RandomPOI);

        MakeBigRooms();

        // PunchHoles()
    }

    private void WalkRooms(RoomType a, RoomType b) {

        // starting room:
        int x = Random.Range(0, hires);
        int y = Random.Range(0, hires);

        int half = hires / 2;

        // end room:
        int tx = Random.Range(0, half);
        int ty = Random.Range(0, half);

        if (x < half) tx += half;
        if (y < half) ty += half;

        // insert two rooms into dungeon:
        SetLilRoom(x, y, (int)a);
        SetLilRoom(tx, ty, (int)b);

        // walk to target room:
        while (x != tx || y != ty) {

            int dir = Random.Range(0, 4);
            int dis = Random.Range(2, 6);

            // get distances to target:
            int dx = tx - x;
            int dy = ty - y;

            // sometimes ...
            if(Random.Range(0, 100) < 50) {
                // pick best direction:
                if(Mathf.Abs(dx) > Mathf.Abs(dy)) {
                    dir = (dx > 0) ? 3 : 2;
                } else {
                    dir = (dy > 0) ? 1 : 0;
                }
            }

            // step into next room:
            for (int i = 0; i < dis; i++) {

                if (dir == 0) y--;
                if (dir == 1) y++;
                if (dir == 2) x--;
                if (dir == 3) x++;

                x = Mathf.Clamp(x, 0, hires - 1);
                y = Mathf.Clamp(y, 0, hires - 1);

                if (GetLilRoom(x, y) == 0) {
                    SetLilRoom(x, y, 1);
                }

            } // ends for

        } // ends while

    } // ends WalkRooms()


    private void MakeBigRooms() {

        for(int x = 0; x < lilrooms.GetLength(0); x++) {
            for (int y = 0; y < lilrooms.GetLength(1); y++) {

                int val = GetLilRoom(x, y);

                int xb = x / lilsPerBig;
                int yb = y / lilsPerBig;
                // if bigroom val < lilroom val
                if(GetBigRoom(xb, yb) < val) {
                    SetBigRoom(xb, yb, val); // set bigroom = lilroom
                }

            }
        }

    }


    public int[,] GetRooms() {

        if (bigrooms == null) {
            Debug.LogError("DungeonLayout: must call Generate() before calling GetRooms()");
            return new int[0, 0];
        }

        // make an empty array, same size:
        int[,] copy = new int[bigrooms.GetLength(0), bigrooms.GetLength(1)];

        // copy data to new array:
        System.Array.Copy(bigrooms, 0, copy, 0, bigrooms.Length);

        // return the copy:
        return copy;
    }


    private int GetLilRoom(int x, int y) {

        if (lilrooms == null) return 0;
        if (x < 0) return 0;
        if (y < 0) return 0;
        if (x >= lilrooms.GetLength(0)) return 0;
        if (y >= lilrooms.GetLength(1)) return 0;

        return lilrooms[x, y];
    }
    private void SetLilRoom(int x, int y, int val) {

        if (lilrooms == null) return;
        if (x < 0) return;
        if (y < 0) return;
        if (x >= lilrooms.GetLength(0)) return;
        if (y >= lilrooms.GetLength(1)) return;

        lilrooms[x, y] = val;
    }

    private int GetBigRoom(int x, int y) {

        if (bigrooms == null) return 0;
        if (x < 0) return 0;
        if (y < 0) return 0;
        if (x >= bigrooms.GetLength(0)) return 0;
        if (y >= bigrooms.GetLength(1)) return 0;

        return bigrooms[x, y];
    }
    private void SetBigRoom(int x, int y, int val) {

        if (bigrooms == null) return;
        if (x < 0) return;
        if (y < 0) return;
        if (x >= bigrooms.GetLength(0)) return;
        if (y >= bigrooms.GetLength(1)) return;

        bigrooms[x, y] = val;
    }

}
