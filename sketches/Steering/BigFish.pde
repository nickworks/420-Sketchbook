class BigFish {
  
  PVector position;
  
  PVector target1 = new PVector();
  PVector target2 = new PVector();
  int cooldown = 0;
  
  BigFish(){
    position = new PVector(random(0, width), random(0, height));
    
  }
  void update(){
    
    if(--cooldown <= 0){
      target1 = new PVector(random(0, width), random(0, height));
      cooldown = (int)random(30, 60);
    }
    
    target2.x += (target1.x - target2.x) * .01;
    target2.y += (target1.y - target2.y) * .01;
    
    position.x += (target2.x - position.x) * .02;
    position.y += (target2.y - position.y) * .02;
  }
  void draw(){
    //ellipse(target2.x, target2.y, 10, 10);
    ellipse(position.x, position.y, 120, 120);    
  }
 
}
