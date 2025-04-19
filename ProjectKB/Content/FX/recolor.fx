#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 view_projection;
float4 rep_color_r;
float4 rep_color_g;
float light_pos_a;
float light_fac_a;
float light_pos_b;
float light_fac_b;

sampler TextureSampler : register(s0);

struct VertexInput {
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};
struct PixelInput {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

PixelInput SpriteVertexShader(VertexInput v) {
    PixelInput output;

    output.Position = mul(v.Position, view_projection);
    output.Color = v.Color;
    output.TexCoord = v.TexCoord;
    return output;
}
float4 SpritePixelShader(PixelInput p) : SV_TARGET{
    float4 diffuse = tex2D(TextureSampler, p.TexCoord.xy) * p.Color;
    float light_fac = lerp(light_fac_a, light_fac_b, clamp((p.Position.y - light_pos_a) / (light_pos_b - light_pos_a), 0, 1));
    diffuse.r *= light_fac;
    float4 newColor;
    newColor.r = rep_color_r.r * diffuse.r + rep_color_g.r * diffuse.g;
    newColor.g = rep_color_r.g * diffuse.r + rep_color_g.g * diffuse.g;
    newColor.b = rep_color_r.b * diffuse.r + rep_color_g.b * diffuse.g;
    newColor.a = diffuse.a;
    return newColor;
}

technique SpriteBatch {
    pass {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}