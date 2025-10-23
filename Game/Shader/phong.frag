#version 330 core
out vec4 FragColor;
 
in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;
 
uniform vec3 lightPos;
uniform vec3 viewPos;
uniform vec3 lightColor;
uniform vec3 objectColor;
uniform vec3 emissiveColor;
uniform float emissiveStrength;

uniform sampler2D textureSampler;
uniform bool useTexture;

#define MAX_POINT_LIGHTS 10
uniform int numPointLights;
uniform vec3 pointLightPositions[MAX_POINT_LIGHTS];
uniform vec3 pointLightColors[MAX_POINT_LIGHTS];
uniform float pointLightIntensities[MAX_POINT_LIGHTS];

vec3 CalcPointLight(vec3 lightPos, vec3 lightColor, float intensity, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(lightPos - fragPos);
    
    float diff = max(dot(normal, lightDir), 0.0);
    
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    
    float distance = length(lightPos - fragPos);
    float attenuation = intensity / (1.0 + 0.03 * distance + 0.032 * (distance * distance));
    
    vec3 diffuse = diff * lightColor * attenuation;
    vec3 specular = 0.5 * spec * lightColor * attenuation;
    
    return (diffuse + specular);
}
 
void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);
    
    // get base color from texture/uniform
    vec3 baseColor = useTexture ? texture(textureSampler, TexCoord).rgb : objectColor;
    
    // ambient
    float ambientStrength = 0.15;
    vec3 ambient = ambientStrength * lightColor;
    
    // main directional light
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;
    
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = 0.5 * spec * lightColor;
    
    vec3 result = (ambient + diffuse + specular) * baseColor;
    
    // add all point lights
    for(int i = 0; i < numPointLights; i++)
    {
        vec3 pointLightContrib = CalcPointLight(
            pointLightPositions[i], 
            pointLightColors[i], 
            pointLightIntensities[i],
            norm, 
            FragPos, 
            viewDir
        );
        result += pointLightContrib * baseColor;
    }
    
    // add glow
    vec3 emissive = emissiveColor * emissiveStrength;
    result += emissive;
    
    FragColor = vec4(result, 1.0);
}