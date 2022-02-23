class FlowfieldGrid {

  // properties:
  int res = 10;
  float zoom = 10;
  boolean isHidden = false;

  // current / cached data:
  float[][] data;

  FlowfieldGrid() {

    build();
  }
  void build() {
    data = new float[res][res];
    for (int x = 0; x < data.length; x++) {
      for (int y = 0; y < data[x].length; y++) {
        float val = noise(x/zoom, y/zoom);
        val = map(val, 0, 1, -PI * 2, PI * 2);
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
    
    if(isHidden) return;
    
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


        float hue = map(val, -PI, PI, 0, 255);

        stroke(255);
        fill(hue, 255, 255);
        ellipse(0, 0, 20, 20);
        line(0, 0, 25, 0);

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
