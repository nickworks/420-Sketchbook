

Flock flock;

void setup(){
 size(900, 600, P2D);
 
 flock = new Flock();
}

void draw(){
  
  if(mousePressed){
    flock.addBoid();
  }
  
  flock.calcForces(flock);
  background(0x333333);
  flock.updateAndDraw();
}
