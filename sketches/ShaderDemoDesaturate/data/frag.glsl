#define PROCESSING_TEXTURE_SHADER

// values from Processing:
uniform sampler2D texture; // the texture to use
uniform vec2 texOffset; // size of a "pixel"

// values from Vertex shader:
varying vec4 vertTexCoord; // uv value at this pixel
varying vec4 vertColor; // vertex color at this pixel

// runs once per pixel:
void main(){

    // lookup pixel color at uv coordinate:
    vec4 color = texture2D(texture, vertTexCoord.xy);

    // desaturate
    float v = (color.r + color.g + color.b)/3;
    color.rgb = vec3(v);

    // set the pixel color of gl_FragColor
    gl_FragColor = color;
}

