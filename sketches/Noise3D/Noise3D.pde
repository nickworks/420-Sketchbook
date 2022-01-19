
ArrayList<PVector> blox = new ArrayList<PVector>();

float threshold = .5;
float zoom = 10;

float sizeOfBlocks = 10;
int dimOfBlocks = 30;

void setup() {
  size(800, 500, P3D);
  noStroke();
  generateTerrainData();
}

void generateTerrainData() {
  
  // get rid of any existing vectors
  blox.clear();
  
  // clamp values:
  zoom = constrain(zoom, 1, 50);
  
  // make an array to hold the density data
  float[][][] data = new float[dimOfBlocks][dimOfBlocks][dimOfBlocks];

  // set data to perlin noise, using zoomed position as an input
  for (int x = 0; x < dimOfBlocks; x++) {
    for (int y = 0; y < dimOfBlocks; y++) {
      for (int z = 0; z < dimOfBlocks; z++) {
        data[x][y][z] = noise(x/zoom, y/zoom, z/zoom) + y / 100.0;
      }
    }
  }
  
  // TODO: check for occlusion...
  
  // spawn blocks where density > threshold:
  for (int x = 0; x < dimOfBlocks; x++) {
    for (int y = 0; y < dimOfBlocks; y++) {
      for (int z = 0; z < dimOfBlocks; z++) {
        if(data[x][y][z] > threshold){ 
          blox.add(new PVector(x, y, z));
        }
      }
    }
  }
}
void checkInput(){
  boolean shouldRegen = false;
  
  if(Keys.PLUS()){
    threshold += .01;
    shouldRegen = true;
  }
  if(Keys.MINUS()){
    threshold -= .01;
    shouldRegen = true;
  }
  
  if(Keys.BRACKET_LEFT()){
    zoom += .1;
    shouldRegen = true;
  }
  if(Keys.BRACKET_RIGHT()){
    zoom -= .1;
    shouldRegen = true;
  }
  
  if(shouldRegen) generateTerrainData();  
}

void draw() {
  
  checkInput();
  
  background(0);

  lights();
  pushMatrix();
 
 // reposition the camera
 // from the corner of the window to the center of the window:
  translate(width/2, height/2);
  
  // rotate the camera
  // controlled by the mouse
  rotateX(map(mouseY, 0, height, -1, 1));
  rotateY(map(mouseX, 0, width, -PI, PI));
  
  // reposition the camera
  // find the offset from the center of the cube to the corner:
  float d = -dimOfBlocks * sizeOfBlocks / 2;
  translate(d,d,d);
  
  // render each cube
  for(PVector pos : blox){
    pushMatrix();
    // move the origin to the position:
    translate(pos.x * sizeOfBlocks, pos.y * sizeOfBlocks, pos.z * sizeOfBlocks);
    // render a cube:
    box(sizeOfBlocks, sizeOfBlocks, sizeOfBlocks);
    popMatrix();
  }
  
  popMatrix();
}
