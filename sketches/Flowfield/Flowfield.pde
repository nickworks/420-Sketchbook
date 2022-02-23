
ArrayList<Agent> agents = new ArrayList<Agent>();

FlowfieldGrid grid;

void setup(){
  size(1000, 800);

  grid = new FlowfieldGrid();

  background(0);
  colorMode(HSB);
}

void draw(){
  
  noStroke();
  //fill(0,0,0,10);
  //rect(0,0,width,height);
  background(0);
  
  if(mousePressed){
    agents.add( new Agent() );    
  }
  
  grid.update();
  grid.draw();
  
  fill(255);
  for(Agent a : agents){
    a.update();
    a.draw();
  }
  
  
  Keys.update();
}
