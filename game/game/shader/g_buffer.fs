#version 330 core
layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;

in vec3 FragPos;
in vec2 TexCoords;
in vec3 Normal;
in vec4 Pos;

uniform vec3 bulletPos;
uniform vec3 cameraPos;
uniform vec3 targetPos;
uniform vec4 bullet;
uniform vec4 target;
uniform bool targetCrashed;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 rot;

uniform vec2 resolution;

uniform int isRaymarch;

float sphereSize = 0.5;
vec3 spherePosition = bulletPos;
vec3 targetPosition = targetPos;
//vec3 cameraPosition = vec3(camera.x,camera.y,camera.z);
vec3 cameraPosition = cameraPos;

float dist_func(vec3 position, float size) {
    return length(position) - size;
}

vec3 normal(vec3 pos, float size) {
    float ep = 0.0001;
    return normalize(vec3(
              dist_func(pos, size) - dist_func(vec3(pos.x - ep, pos.y, pos.z), size),
              dist_func(pos, size) - dist_func(vec3(pos.x, pos.y - ep, pos.z), size),
              dist_func(pos, size) - dist_func(vec3(pos.x, pos.y, pos.z - ep), size)
              ));
}

vec3 color = vec3(0.0);
vec3 norm = vec3(0.0);

bool hit(){
    vec2 pixelPosition2d = (gl_FragCoord.xy * 2.0 - resolution.xy) / min(resolution.x, resolution.y);
    float screenZ = -1.0;
    vec3 pixelPosition = vec3(pixelPosition2d, screenZ);
    pixelPosition = ((rot)*vec4(pixelPosition, 1.0)).xyz;
//    pixelPosition.x -= 0.5;
//    vec3 pixelPosition = FragPos;
    
//    spherePosition = ((rot) * vec4(spherePosition, 1.0)).xyz;
    
    vec3 lightDirection = normalize(vec3(0.0, 0.0, 1.0));
    vec3 rayDirection = normalize(pixelPosition - cameraPosition);
    
    for (int i = 0; i < 99; i++) {
        vec3 cameraToSphere = cameraPosition-spherePosition;
        float bulletDist = dist_func(cameraToSphere, sphereSize);
        
        vec3 cameraToTarget = cameraPosition-targetPosition;
        float targetDist = dist_func(cameraToTarget, sphereSize);
        
//        float dist = bulletDist;
        float dist = bulletDist;
        if (!targetCrashed) {
            dist = min(bulletDist, targetDist);
        }
        
        if (dist < 0.0001) {
//            norm = normal(cameraToSphere, sphereSize);
//            float diff = dot(norm, lightDirection);
//            color = vec3(diff);
//            return true;
            if (dist == bulletDist) {
                norm = normal(cameraToSphere, sphereSize);
                float diff = dot(norm, lightDirection);
                color = vec3(diff);
                return true;
            } else {
                norm = normal(cameraToTarget, sphereSize);
                float diff = dot(norm, lightDirection);
                color = vec3(diff);
                return true;
            }
        }
        cameraPosition += rayDirection * dist;
    }
    
    return false;
}

//uniform sampler2D texture_diffuse1;
//uniform sampler2D texture_specular1;

void main()
{
    if (isRaymarch==0) {
        // store the fragment position vector in the first gbuffer texture
        gPosition = FragPos;
        // also store the per-fragment normals into the gbuffer
        gNormal = normalize(Normal);
        // and the diffuse per-fragment color
        //    gAlbedoSpec.rgb = texture(texture_diffuse1, TexCoords).rgb;
        gAlbedoSpec.rgb = vec3(1.0);
        // store specular intensity in gAlbedoSpec's alpha component
        gAlbedoSpec.a = 1.0;
//        float far=gl_DepthRange.far;
//        float near=gl_DepthRange.near;
//        float depth = (((far-near) * FragPos.z) + near + far) / 2.0;
        gl_FragDepth = Pos.z/Pos.w*0.5+0.5;
    } else {
        if (hit()) {
            gPosition = cameraPosition;
            gNormal = norm;
            gAlbedoSpec.rgb = vec3(1.0, 0.0, 0.0);
            gAlbedoSpec.a = 1.0;
            gl_FragDepth = -cameraPosition.z/Pos.w*0.5+0.5;
        } else {
//            gPosition = vec3(0.0, 0.0, 0.0);
//            gNormal = vec3(0.0, 0.0, -1.0);
//            gAlbedoSpec.rgb = vec3(1.0);
//            gAlbedoSpec.a = 0.0;
            gl_FragDepth = 1.0;
        }
    }
}
