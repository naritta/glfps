#version 330 core
precision mediump float;
out vec4 FragColor;
uniform vec2 resolution;
uniform vec4 bullet;
uniform vec4 target;
uniform vec3 camera;

float sphereSize = bullet.w;
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

void main(void) {
    vec2 pixelPosition2d = (gl_FragCoord.xy * 2.0 - resolution.xy) / min(resolution.x, resolution.y);
    float screenZ = 0.0;
    vec3 pixelPosition = vec3(pixelPosition2d, screenZ);
    
    vec3 lightDirection = normalize(vec3(0.0, 0.0, 1.0));
    vec3 rayDirection = normalize(pixelPosition - cameraPosition);
    vec3 color = vec3(0.0);
    
    for (int i = 0; i < 99; i++) {
        vec3 cameraToSphere = cameraPosition-spherePosition;
        float bulletDist = dist_func(cameraToSphere, sphereSize);
        
        vec3 cameraToTarget = cameraPosition-targetPosition;
        float targetDist = dist_func(cameraToTarget, sphereSize);
        
        float dist = min(bulletDist, targetDist);
        
        if (dist < 0.0001) {
            if (dist == bulletDist) {
                vec3 norm = normal(cameraToSphere, sphereSize);
                float diff = dot(norm, lightDirection);
                color = vec3(diff);
                break;
            } else {
                vec3 norm = normal(cameraToTarget, sphereSize);
                float diff = dot(norm, lightDirection);
                color = vec3(diff);
                break;
            }
        }
        cameraPosition += rayDirection * dist;
    }
    
    FragColor = vec4(color, 1.0);
}
