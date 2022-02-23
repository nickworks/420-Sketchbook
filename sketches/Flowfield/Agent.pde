class Agent {
  
  PVector position = new PVector();
  PVector velocity = new PVector();
  PVector force = new PVector();
  float mass = 1;
  float maxSpeed = 10;
  float maxForce = 10;  
  
  PVector target = new PVector();
  float targetAngle = 0;
  float targetRadius = 100;
  float targetSpeed = 0;
  
  Agent(){
    position = new PVector(mouseX, mouseY);
    velocity = new PVector(random(-5, 5), random(-5, 5));
    mass = random(50, 100);
    maxForce = random(5, 15);
    targetAngle = random(-PI, PI);
    targetRadius = random(50, 150);
    maxSpeed = random(2, 15);
    targetSpeed = map(maxForce, 5, 15, .005, .05);
  }
  void update(){
    
    float angle = grid.getDirectionAt(position);
    
    PVector offset = PVector.fromAngle(angle);
    offset.mult(100);
    
    target = PVector.add(position, offset);
    
    doSteeringForce();
    doEuler();
  }
  void doSteeringForce(){
    
    // find desired velocity
    // desired velocity = clamp(target position - current position)
    
    PVector desiredVelocity = PVector.sub(target, position);
    desiredVelocity.setMag(maxSpeed);
    
    // find steering force
    // steering force = desired velocity - current velocity
    
    PVector steeringForce = PVector.sub(desiredVelocity, velocity);
    steeringForce.limit(maxForce);
    
    force.add(steeringForce);
  }
  
  void doEuler(){
    // euler integration:
    PVector acceleration = PVector.div(force, mass);
    velocity.add(acceleration); // velocity += acceleration
    position.add(velocity); // position += velocity
    force.mult(0);
  }
  void draw(){
    
    //ellipse(target.x, target.y, 10, 10);
    
    float a = velocity.heading();
    pushMatrix(); // begin transform...
    translate(position.x, position.y);
    rotate(a);
    triangle(5, 0, -10, 5, -10, -5);
    popMatrix(); // end transform..
  }
  
}
