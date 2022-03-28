class Polygon {
  
  // transform:
  
  PVector position = new PVector(); // 0,0
  float rotation = 0;
  float scale = 1;
  
  // local-space points:
  ArrayList<PVector> pts = new ArrayList<PVector>();
  
  // cached values:
  
  // world-space points:
  ArrayList<PVector> wpts = new ArrayList<PVector>();
  
  // world-space edge normals:
  ArrayList<PVector> dirs = new ArrayList<PVector>();
  
  // aabb corners:
  PVector min = new PVector();
  PVector max = new PVector();
  
  boolean isColliding = false;
  
  
  Polygon(int steps){

    position.x = width/2;
    position.y = height/2;
    
    // add points to array:
    for(int i = 0; i < steps; i++){
      float radians = TWO_PI * i / (float)steps;      
      float mag = 50;
      pts.add( new PVector(
        mag * cos(radians),
        mag * sin(radians)
      ) );
    }
    
  }
  
  void update(){
    cacheValues();
  }
  void cacheValues(){
    
    // make the 3x3 transform matrix:
    PMatrix2D xform = new PMatrix2D();
    xform.translate(position.x, position.y);
    xform.rotate(rotation);
    xform.scale(scale);
    
    // calculates the world-position of each point:
    wpts.clear();
    for(PVector p : pts){
       PVector temp = new PVector();
       xform.mult(p, temp);
       wpts.add(temp);
    }
    
    dirs.clear();
    // find min/max:
    int i = 0;
    for(PVector p : wpts){
      if(i == 0 || p.x > max.x) max.x = p.x;
      if(i == 0 || p.x < min.x) min.x = p.x;
      if(i == 0 || p.y > max.y) max.y = p.y;
      if(i == 0 || p.y < min.y) min.y = p.y;
      
      {
        // get prev (or last) point:
        PVector p0 = wpts.get(i == 0 ? wpts.size() - 1 : i - 1);
        PVector p1 = wpts.get(i);
        
        PVector d = PVector.sub(p1, p0); // p1 - p0
        d.normalize();
        d = new PVector(d.y, -d.x);
        //d = new PVector(-d.y, d.x);
        dirs.add(d);
      }
      i++;
    }
  }
  
  boolean doesOverlap(Polygon gon){
    
    // AABB:
    if(this.min.x > gon.max.x) return false; // this is to the RIGHT of the other
    if(this.max.x < gon.min.x) return false; // this is to the LEFT of the other
    
    if(this.min.y > gon.max.y) return false; // this is BELOW the other
    if(this.max.y < gon.min.y) return false; // this is ABOVE the other
    
    ArrayList<PVector> axes = new ArrayList<PVector>();
    axes.addAll(this.dirs);
    axes.addAll(gon.dirs);
    
    for(PVector axis : axes){
      
      MinMax a = this.projectOn(axis);
      MinMax b = gon.projectOn(axis);
      
      if(a.min > b.max) return false;
      if(a.max < b.min) return false;
    }
    
    return true;
  }
  
  MinMax projectOn(PVector axis){
    MinMax mm = new MinMax();
    
    int i = 0;
    for(PVector p : wpts){
      float value = PVector.dot(axis, p);
      
      if(i == 0 || value < mm.min) mm.min = value;
      if(i == 0 || value > mm.max) mm.max = value;
      
      i++;
    }
    
    return mm;
  }
  
  void draw(){

    noFill();
    stroke(128);
    rect(min.x, min.y, max.x - min.x, max.y - min.y);
    
    stroke(0);
    
    if(isColliding) fill(255, 0, 0);
    else fill(255);
    
    beginShape();
    int i = 0;
    for(PVector p : wpts) {
      
      vertex(p.x, p.y);
   
      PVector p0 = wpts.get(i == 0 ? wpts.size() - 1 : i - 1);
      PVector mid = PVector.div(PVector.add(p, p0), 2);
      PVector d = dirs.get(i);
      line(mid.x, mid.y, mid.x + d.x * 20, mid.y + d.y * 20);
      
      i++;
    }
    endShape(CLOSE);

  }
  
}
