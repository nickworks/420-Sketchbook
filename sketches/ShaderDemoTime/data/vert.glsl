#define PROCESSING_TEXTURE_SHADER

// values from Processing:
uniform mat4 transform;
uniform mat4 texMatrix;

attribute vec4 vertex; // position in local-space
attribute vec4 color; // vertex color
attribute vec2 texCoord; // uv

varying vec4 vertColor;
varying vec4 vertTexCoord;

// runs once per vertex:
void main(){

    // gl_Position to the final vertex screen-space position:
    gl_Position = transform * vertex;

    vertColor = color;

    vertTexCoord = texMatrix * vec4(texCoord, 1, 1);
}

