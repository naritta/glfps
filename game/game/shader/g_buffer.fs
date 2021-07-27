#version 330 core
layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;

in vec3 FragPos;
in vec2 TexCoords;
in vec3 Normal;

uniform vec4 bullet;
uniform vec4 target;
uniform vec3 camera;

uniform vec2 resolution;

uniform int isRaymarch;

float sphereSize = 0.1;
vec3 spherePosition = vec3(bullet.x,bullet.y,bullet.z);
vec3 targetPosition = vec3(target.x,target.y,target.z);
vec3 cameraPosition = vec3(camera.x,camera.y,camera.z);

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
    float screenZ = 0.0;
    vec3 pixelPosition = vec3(pixelPosition2d, screenZ);
//    vec3 pixelPosition = FragPos;
    
    vec3 lightDirection = normalize(vec3(0.0, 0.0, 1.0));
    vec3 rayDirection = normalize(pixelPosition - cameraPosition);
    
    for (int i = 0; i < 99; i++) {
        vec3 cameraToSphere = cameraPosition-spherePosition;
        float bulletDist = dist_func(cameraToSphere, sphereSize);
        
        vec3 cameraToTarget = cameraPosition-targetPosition;
        float targetDist = dist_func(cameraToTarget, sphereSize);
        
        float dist = min(bulletDist, targetDist);
        
        if (dist < 0.0001) {
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
    } else {
        if (hit()) {
            gPosition = cameraPosition;
            gNormal = norm;
            gAlbedoSpec.rgb = vec3(1.0, 0.0, 0.0);
            gAlbedoSpec.a = 1.0;
        } else {
            gPosition = vec3(0.0, 0.0, 1000.0);
            gNormal = vec3(0.0, 0.0, -1.0);
            gAlbedoSpec.rgb = vec3(0.0);
            gAlbedoSpec.a = 0.0;
        }
    }
}
