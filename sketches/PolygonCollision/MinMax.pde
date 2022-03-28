
class MinMax {
  
  float min = 0;
  float max = 0;
  
  MinMax(){
    this(0);    
  }
  
  MinMax(float v){
    this(v, v); 
  }
  
  MinMax(float min, float max){
    this.min = min;
    this.max = max;
  }
  
}
