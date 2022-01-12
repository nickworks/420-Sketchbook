
void setup(){
  size(500, 600, P2D);
  stroke(255);
  strokeWeight(2);
}

void draw(){
  background(0);

  float time = millis() / 1000.0;
  
  // draw three arrays to screen:
  float third = height/3;
  
  float zoom = map(mouseX, 0, width, 10, 100);
  
  for(int x = 0; x < width; x++){
    
    float v1 = map(sin(x/zoom + time), -1, 1, 0, 1); // -1 to 1
    float v2 = random(0, 1);
    float v3 = noise(x/zoom + time);
    
    float y1 = third - v1 * third;
    float y2 = third * 2 - v2 * third;
    float y3 = height - v3 * third;
    
    point(x, y1);
    point(x, y2);
    point(x, y3);
  }
  
}
