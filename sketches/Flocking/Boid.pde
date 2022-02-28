

class Boid {
  
  int TYPE = 1;
  
  PVector position = new PVector();
  PVector velocity = new PVector();
  PVector force = new PVector();
  
  PVector _dir = new PVector();
  
  float mass = 20;
  float speed = 5;
  
  float radiusCohesion = 200;
  float radiusAlignment = 100;
  float radiusSeparation = 50;
  
  float forceCohesion = 1;
  float forceAlignment = .25;
  
  float forceSeparation = 10;
  
  Boid(float x, float y){
    position.x = x;
    position.y = y;
    
    velocity.x = random(-3, 3);
    velocity.y = random(-3, 3);
    
  }
  
  void calcForces(Flock f){
    
    // calculate forces!!
    
    // 1. cohesion // pull towards group center
    // 2. separation // pushes boids apart
    // 3. alignment // turn boid to nearby avg direction

    // force that pushes boids towards center
    // force that pushes boids away from sides
    // ...
    
    PVector centerOfGroup = new PVector();
    PVector avgAlignment = new PVector();
    int numCohesion = 0;
    int numAlignment = 0;
    
    for(Boid b : f.boids){
      
      if(b == this) continue; // if same boid, skip calculations
      
      float dx = b.position.x - position.x;
      float dy = b.position.y - position.y;
      float dis = sqrt(dx*dx + dy*dy); // pythagorean theorem
      
      if(TYPE == 1 && b.TYPE == 1){
        if(dis < radiusCohesion){
          centerOfGroup.add(b.position);
          numCohesion++;
        }
        if(dis < radiusSeparation){
          PVector awayFromB = new PVector(-dx/dis, -dy/dis);
          awayFromB.mult(forceSeparation / dis);
          
          force.add(awayFromB);
        }
        if(dis < radiusAlignment){
          avgAlignment.add(b._dir);
          numAlignment++;
        }
      }
      
      
    } // ends for loop
    
    if(numCohesion > 0){
      centerOfGroup.div(numCohesion);

      PVector dirToCenter = PVector.sub(centerOfGroup, position);
      dirToCenter.setMag(speed);
      
      PVector cohesionForce = PVector.sub(dirToCenter, velocity);
      
      cohesionForce.limit(forceCohesion);
      force.add(cohesionForce);
    }
    
    if(numAlignment > 0){
      avgAlignment.div(numAlignment); // get avg direction
      avgAlignment.mult(speed); // get desired vel = dir * max speed
      
      PVector alignmentForce = PVector.sub(avgAlignment, velocity);
      alignmentForce.limit(forceAlignment);
      force.add(alignmentForce);
    }
    
    
  }
  void updateAndDraw(){
    
    // euler step:
    PVector acceleration = PVector.div(force, mass); 
    velocity.add(acceleration);
    position.add(velocity);
    force = new PVector(0,0,0);
    
    // loop on sides of screen:
    if(position.x < 0) position.x += width;
    else if(position.x > width) position.x -= width;
    
    if(position.y < 0) position.y += height;
    else if(position.y > height) position.y -= height;
    
    // cache the direction vector:
    _dir = PVector.div(velocity, velocity.mag());
    
    ellipse(position.x, position.y, 10, 10);
  }
}
