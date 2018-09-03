#include "Uniforms.glsl"
#include "Samplers.glsl"
#include "Transform.glsl"
#include "ScreenPos.glsl"

#ifdef COMPILEPS
uniform vec2 cCloudsOffset;
uniform vec4 cSpecColor;
#endif

varying vec4 vTexCoord;
varying vec4 vTangent;
varying vec3 vNormal;
varying vec4 vWorldPos;
varying vec4 vScreenPos;

void VS()
{
    mat4 modelMatrix = iModelMatrix;
    vec3 worldPos = GetWorldPos(modelMatrix);
    vec4 pos = GetClipPos(worldPos);
    vec3 normal = GetWorldNormal(modelMatrix);
    vec3 tangent = GetWorldTangent(modelMatrix).xyz;
    vec3 bitangent = cross(tangent, normal) * iTangent.w;
    
    gl_Position = pos;
    
    vWorldPos = vec4(worldPos, GetDepth(pos));
    vNormal = normal;
    vTexCoord = vec4(GetTexCoord(iTexCoord), bitangent.xy);
    vTangent = vec4(tangent, bitangent.z);
}

void PS()
{
    vec4 earthDiff = texture2D(sDiffMap, vTexCoord.xy);
    vec4 clouds = texture2D(sEnvMap, vTexCoord.xy + cCloudsOffset);
    vec4 night = texture2D(sEmissiveMap, vTexCoord.xy); 
    vec3 spec = texture2D(sSpecMap, vTexCoord.xy).xyz;
    
    mat3 tbn = mat3(vTangent.xyz, vec3(vTexCoord.zw, vTangent.w), vNormal);
    vec3 normal = normalize(DecodeNormal(texture2D(sNormalMap, vTexCoord.xy)) * tbn);

    vec3 finalColor = earthDiff.rgb;
    finalColor += cSpecColor.rgb * spec.rgb * cLightColor.a;
    finalColor *= clamp(dot(normal, cLightDirPS), 0.0, 1.0);
    finalColor += clouds.rgb * (dot(vNormal, cLightDirPS) + 0.5);
    finalColor += night.rgb * (1.0 - dot(vNormal, cLightDirPS));

    gl_FragColor = vec4(finalColor, 1.0);
}