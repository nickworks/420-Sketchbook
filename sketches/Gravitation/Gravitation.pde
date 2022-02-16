
ArrayList<Agent> agents = new ArrayList<Agent>();

float G = 1;
float maxForce = 1;


void setup(){
  size(800, 600);
  
  // spawn agents
  for(int i = 0; i < 30; i++){
    agents.add( new Agent(10, 100) );
  }
  
  Agent sun = new Agent(1000, 2000);
  sun.position = new PVector(width/2, height/2);
  agents.add(sun);
}

void draw(){
  // section 1: update:
  
  for(Agent a : agents){
    a.update();
  }
  
  // section 2: draw:
  //background(0);
  
  for(Agent a : agents){
    a.doneCalcingGravity = false;
    a.draw();
  }
  
}
