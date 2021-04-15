texture NormalMap;
sampler NormalSampler = { бн }

float4 PixelShader(VertexOuput input) : COLOR0
{
	float3 normal = tex2D(NormalSampler, input.UV).xzy;
	normal = normalize(normal);
	// then do rest of the lighting etc. as before
	...
}
