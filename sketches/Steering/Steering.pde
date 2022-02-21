
ArrayList<Agent> agents = new ArrayList<Agent>();
BigFish big;

void setup(){
  size(1000, 800);
  big = new BigFish();
  background(0);
  noStroke();
}

void draw(){
  
  fill(0,0,0,10);
  rect(0,0,width,height);
  
  if(mousePressed){
    agents.add( new Agent() );    
  }
  
  fill(255, 0, 0);
  big.update();
  big.draw();
  
  fill(255);
  for(Agent a : agents){
    a.update();
    a.draw();
  }
  
  
}
