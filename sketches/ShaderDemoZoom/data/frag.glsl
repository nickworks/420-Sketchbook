#define PROCESSING_TEXTURE_SHADER

// values from Processing:
uniform sampler2D texture; // the texture to use
uniform vec2 texOffset; // size of a "pixel"

// values from Vertex shader:
varying vec4 vertTexCoord; // uv value at this pixel
varying vec4 vertColor; // vertex color at this pixel

// runs once per pixel:
void main(){

    float ratio = texOffset.x / texOffset.y;

    vec2 uv = vertTexCoord.xy;
    
    float mag = length(uv); // dis from (0,0)
    float rad = atan(uv.y, uv.x); // angle from (0,0)

    mag += .01;

    uv.x = mag * cos(rad);
    uv.y = mag * sin(rad);

    // lookup pixel color at uv coordinate:
    vec4 color = texture2D(texture, uv);


    // set the pixel color of gl_FragColor
    gl_FragColor = color;
}

