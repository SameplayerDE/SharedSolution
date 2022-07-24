#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix View;
matrix Projection;

bool AlphaTest = true;
bool AlphaTestGreater = true;
float AlphaTestValue = 0.5f;

Texture2D Texture : register(t0);
sampler Sampler : register(s0)
{
	Texture = (Texture);
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 position = input.Position;
    float4 color = float4(1, 1, 1, 1);
    float2 textureCoordinate = input.TextureCoordinate;
    
    float4 worldPosition = mul(position, World);
    float4 viewPosition = mul(worldPosition, View);

	output.Position = mul(viewPosition, Projection);
    output.Color = color;
    output.TextureCoordinate = textureCoordinate;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(Sampler, input.TextureCoordinate) * input.Color;
	if (AlphaTest) {
	    clip((color.a - AlphaTestValue) * (AlphaTestGreater ? 1 : -1));
    }
	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};