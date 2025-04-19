#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 view_projection;
float xmult;
float4 base_color;

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
    output.Color = v.Color * base_color;
    output.TexCoord = v.TexCoord;
    return output;
}
float4 SpritePixelShader(PixelInput p) : SV_TARGET{
    float xm = abs(p.Position.x - (128 * xmult)) / (128 * xmult);
    float ym = abs(p.Position.y - 64) / 64;
    float mask = 1;
    if (ym > 0.8125 && ym < 0.9375) { mask = 0; }
    else { mask = max(min((1 - xm) * xmult, 1), 0); }
    return p.Color * float4(mask, mask, mask, mask);
}

technique SpriteBatch {
    pass {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}