#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float texScale;

void main()
{
    FragPos = vec3(model * vec4(aPosition, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    
    vec3 absNormal = abs(normalize(Normal));
    
    if (absNormal.y > absNormal.x && absNormal.y > absNormal.z) {
        TexCoord = FragPos.xz * texScale;
    } else if (absNormal.x > absNormal.z) {
        TexCoord = FragPos.zy * texScale;
    } else {
        TexCoord = FragPos.xy * texScale;
    }
    
    gl_Position = projection * view * vec4(FragPos, 1.0);
}