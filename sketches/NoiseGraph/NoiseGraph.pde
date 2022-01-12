
ArrayList<PVector> vals = new ArrayList<PVector>();

void setup(){
  size(500, 600);
  stroke(255);
}

void draw(){
  background(0);
  
  // add new values to our array:
  vals.add(0, new PVector(0, 0, 0));
  
  // remove last item, if too many:
  if(vals.size() > width) vals.remove(vals.size() - 1);
  
  // draw three arrays to screen:
  for(int x = 0; x < vals.size(); x++){
    float y1 = height/3 - vals.get(x).x * height/3;
    
    point(x, y1);
  }
  
}
