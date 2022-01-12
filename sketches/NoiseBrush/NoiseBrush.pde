

void setup(){
  size(600, 300);
  
}

void draw(){
  //background(0);
  
  float time = millis() / 1000.0;
  
  float d3 = map(noise(time), 0, 1, 50, 200);
  
  ellipse(mouseX, mouseY, d3, d3);
  
}
