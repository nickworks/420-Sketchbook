PImage img;
PShader shader;

void setup(){
  size(800, 600, P2D);
  
  img = loadImage("panda.jpeg");
  imageMode(CENTER);
  
  shader = loadShader("frag.glsl", "vert.glsl");
}

void draw(){
  background(100);
  image(img, mouseX, mouseY); 
  filter(shader);
}
