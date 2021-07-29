#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 FragPos;
out vec2 TexCoords;
out vec3 Normal;
out vec4 Pos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform int isRaymarch;

void main()
{
    if (isRaymarch==0) {
        vec4 worldPos = model * vec4(aPos, 1.0);
        FragPos = worldPos.xyz;
        TexCoords = aTexCoords;
        
        mat3 normalMatrix = transpose(inverse(mat3(model)));
        Normal = normalMatrix * aNormal;
        
        gl_Position = projection * view * worldPos;
        Pos = gl_Position;
    } else {
        vec4 worldPos = model * vec4(aPos, 1.0);
        FragPos = worldPos.xyz;
        TexCoords = aTexCoords;
        
        mat3 normalMatrix = transpose(inverse(mat3(model)));
        Normal = normalMatrix * aNormal;
        
        gl_Position = vec4(aPos, 1.0);
//        gl_Position = worldPos;
//        gl_Position = projection * view * worldPos;
        Pos = projection * view * worldPos;
    }
}
