class Dungeon {

  int roomSize = 10;
  int res = 50;
  int[][] rooms;

  Dungeon() {
    generate();
  }
  void generate() {
    rooms = new int[res][res];

    // TODO: fill with data
  }
  void draw() {
    noStroke();
    for (int x = 0; x < rooms.length; x++) {
      for (int y = 0; y < rooms[x].length; y++) {
        int val = rooms[x][y];
        if (val > 0) {
          switch(val){
            case 1: fill(255);       break;
            case 2: fill(0, 255, 0); break;
            case 3: fill(255, 0, 0); break;
            case 4: fill(0, 0, 255); break;
          }
          rect(x *roomSize, y * roomSize, roomSize, roomSize);
        }
      }
    }
  }
}
