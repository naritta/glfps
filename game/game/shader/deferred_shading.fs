#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedoSpec;
uniform vec2 resolution;

struct Light {
    vec3 Position;
    vec3 Color;
    
    float Linear;
    float Quadratic;
    float Radius;
};
const int NR_LIGHTS = 32;
uniform Light lights[NR_LIGHTS];
uniform vec3 viewPos;

uniform vec4 bullet;
uniform vec4 target;
uniform vec3 camera;

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

bool hit(){
    vec2 pixelPosition2d = (gl_FragCoord.xy * 2.0 - resolution.xy) / min(resolution.x, resolution.y);
    float screenZ = 0.0;
    vec3 pixelPosition = vec3(pixelPosition2d, screenZ);
    
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
                vec3 norm = normal(cameraToSphere, sphereSize);
                float diff = dot(norm, lightDirection);
                color = vec3(diff);
                return true;
            } else {
                vec3 norm = normal(cameraToTarget, sphereSize);
                float diff = dot(norm, lightDirection);
                color = vec3(diff);
                return true;
            }
        }
        cameraPosition += rayDirection * dist;
    }
    
    return false;
}

void main()
{
    vec2 pixelPosition2d = (gl_FragCoord.xy * 2.0 - resolution.xy) / min(resolution.x, resolution.y);
    // retrieve data from gbuffer
    vec3 FragPos = texture(gPosition, TexCoords).rgb;
    vec3 Normal = texture(gNormal, TexCoords).rgb;
    vec3 Diffuse = texture(gAlbedoSpec, TexCoords).rgb;
    float Specular = texture(gAlbedoSpec, TexCoords).a;
    
    // then calculate lighting as usual
    vec3 lighting  = Diffuse * 0.1; // hard-coded ambient component
    vec3 viewDir  = normalize(viewPos - FragPos);
    for(int i = 0; i < NR_LIGHTS; ++i)
    {
        // calculate distance between light source and current fragment
        float distance = length(lights[i].Position - FragPos);
        if(distance < lights[i].Radius)
        {
            // diffuse
            vec3 lightDir = normalize(lights[i].Position - FragPos);
            vec3 diffuse = max(dot(Normal, lightDir), 0.0) * Diffuse * lights[i].Color;
            // specular
            vec3 halfwayDir = normalize(lightDir + viewDir);
            float spec = pow(max(dot(Normal, halfwayDir), 0.0), 16.0);
            vec3 specular = lights[i].Color * spec * Specular;
            // attenuation
            float attenuation = 1.0 / (1.0 + lights[i].Linear * distance + lights[i].Quadratic * distance * distance);
            diffuse *= attenuation;
            specular *= attenuation;
            lighting += diffuse + specular;
        }
    }
    
    if (!hit()) {
        FragColor = vec4(lighting, 1.0);
    } else {
        FragColor = vec4(color, 1.0);
    }
}
