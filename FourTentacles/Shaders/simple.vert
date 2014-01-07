varying vec4 color;

void main(void)
{
    color = gl_Color;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
}
