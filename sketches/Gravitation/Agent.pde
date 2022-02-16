
class Agent {

  color colour;
  
  PVector position = new PVector();
  PVector velocity = new PVector();
  PVector force = new PVector();
  
  float size;
  float mass;
  
  boolean doneCalcingGravity = false;
  
  Agent(float massMin, float massMax){
    position.x = random(0, width);
    position.y = random(0, height);
    
    mass = random(massMin, massMax);
    size = sqrt(mass);
    
    colorMode(HSB);
    colour = color(random(0,255), 255, 255);
    
  }
  
  void update(){
    
    for(Agent a : agents){
      
      if(a == this) continue; // skip...
      if(a.doneCalcingGravity) continue; // skip...
      
      PVector f = findGravityForce(a);
      force.add(f);
    }
    doneCalcingGravity = true;
    
    // a = f/m
    PVector acceleration = PVector.div(force, mass);
    
    // clear force:
    force.set(0, 0);
    
    // v += a
    velocity.add(acceleration);
    
    // p += v
    position.add(velocity);
    
  }
  
  void draw(){
    fill(colour);
    ellipse(position.x, position.y, size, size);
  }
  
  PVector findGravityForce(Agent a){
    
    PVector vToAgentA = PVector.sub(a.position, position);
    
    float r = vToAgentA.mag();
    
    float gravForce = G * (a.mass * mass) / (r * r);
    
    if(gravForce > maxForce) gravForce = maxForce;
    
    vToAgentA.normalize();
    vToAgentA.mult(gravForce);
    
    // add force to other object:
    a.force.add(PVector.mult(vToAgentA, -1));
    
    return vToAgentA;
  }
  
  
}
