class Flock {

  ArrayList<Boid> boids = new ArrayList<Boid>();


  Flock() {
  }
  void addBoid(){
    boids.add( new Boid( mouseX, mouseY ) );
  }
  void calcForces(Flock f) {
    for(Boid b : boids) b.calcForces(f);
  }
  void updateAndDraw() {
    for(Boid b : boids) b.updateAndDraw();
  }
}
