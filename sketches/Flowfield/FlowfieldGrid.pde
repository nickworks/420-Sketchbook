class FlowfieldGrid {

  // properties:
  int res = 50;
  float zoom = 10;
  boolean isHidden = false;

  // current / cached data:
  float[][] data;

  FlowfieldGrid() {

    build();
  }
  void build() {
    data = new float[res][res];

    int thresh = 3;
    float w = getCellWidth();
    float h = getCellHeight();

    float disThresh = pow(width/2,2);

    for (int x = 0; x < data.length; x++) {
      for (int y = 0; y < data[x].length; y++) {
        
        // use perlin noise to generate val
        float noise = noise(x/zoom, y/zoom);
        
        //noise = sin(noise);
        
        // map the perlin noise to -180 to +180 degrees
        float angleNoise = map(noise, .3, .7, -PI, PI);

        // find vector to center of screen:
        float dy = (height/(float)2) - (y * h + h/2);
        float dx = (width/(float)2) - (x * w + w/2);
        
        // find angle to center:
        float angleCenter = atan2(dy, dx);

        // find distance to center:
        float dis = sqrt(dx*dx + dy*dy);
        float relativeDis = (dis)/(width);
        relativeDis = constrain(relativeDis, 0, 1);
        relativeDis = 1 - relativeDis;
        relativeDis *= relativeDis;
        relativeDis = 1 - relativeDis;
        
        if(angleCenter < angleNoise) angleCenter += TWO_PI;
        
        // interpolate final angle:
        //float val = lerp(angleNoise, angleCenter, relativeDis);
        float val = lerp(angleCenter+PI, angleCenter-PI, noise-.8);
        
        while(val > PI) val -= TWO_PI;
        while(val <-PI) val += TWO_PI;
        
        data[x][y] = val;
      }
    }
  }

  void update() {

    boolean rebuild = false;

    if (Keys.onDown(32)) {
      isHidden = !isHidden;
      rebuild = true;
    }

    if (Keys.onDown(37)) {
      res--;
      rebuild = true;
    }
    if (Keys.onDown(39)) {
      res++;
      rebuild = true;
    }

    if (Keys.onDown(38)) {
      zoom += 1;
      rebuild = true;
    }
    if (Keys.onDown(40)) {
      zoom -= 1;
      rebuild = true;
    }

    res = constrain(res, 4, 50);
    zoom = constrain(zoom, 5, 50);

    if (rebuild) build();
  }

  void draw() {

    if (isHidden) return;

    float w = getCellWidth();
    float h = getCellHeight();

    for (int x = 0; x < data.length; x++) {
      for (int y = 0; y < data[x].length; y++) {

        float val = data[x][y];

        float topleftX = x * w;
        float topleftY = y * h;

        pushMatrix();
        translate(topleftX + w/2, topleftY + h/2);
        rotate(val);


        float hue = map(val, -PI, PI, 0, 511);

        int hu = ((int)hue)%255;

        noStroke();
        fill(hu, 255, 255);
        ellipse(0, 0, 15, 15);
        //stroke(255);
        //line(0, 0, 25, 0);

        popMatrix();
      } // end y loop
    } // end x loop
  } // end draw()
  float getCellWidth() {
    return width / res;
  }
  float getCellHeight() {
    return height / res;
  }
  float getDirectionAt(PVector p) {
    return getDirectionAt(p.x, p.y);
  }
  float getDirectionAt(float x, float y) {

    int ix = (int)(x / getCellWidth());
    int iy = (int)(y / getCellHeight());

    if (ix < 0 || iy < 0 || ix >= data.length || iy >= data[0].length) {
      // invalidate coordinate...

      float dy = (height/(float)2) - y;
      float dx = (width/(float)2) - x;

      return atan2(dy, dx);
    }

    return data[ix][iy];
  }
}
