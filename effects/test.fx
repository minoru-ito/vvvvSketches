// this is a very basic template. use it to start writing your own effects.
// if you want effects with lighting start from one of the GouraudXXXX or PhongXXXX effects

// --------------------------------------------------------------------------------------------------
// PARAMETERS:
// --------------------------------------------------------------------------------------------------

//transforms
float4x4 tWVP: WORLDVIEWPROJECTION;

//texture
texture Tex <string uiname="Texture";>;
sampler Samp = sampler_state    //sampler for doing the texture-lookup
{
    Texture   = (Tex);          //apply a texture to the sampler
};

// --------------------------------------------------------------------------------------------------
// VERTEXSHADERS
// --------------------------------------------------------------------------------------------------
void VS(
    float3 in_pos  : POSITION,
    float in_size : PSIZE,
    out float4 out_pos : POSITION,
    out float2 out_tex : TEXCOORD0,
    out float out_size : PSIZE)
{
    out_pos = mul(float4(in_pos, 1.0f), tWVP);
    
    out_size = in_size;
    
    out_tex = 0.0;
}

// --------------------------------------------------------------------------------------------------
// PIXELSHADERS:
// --------------------------------------------------------------------------------------------------
void PS(
     float2 in_tex : TEXCOORD0,
     out float4 out_color : COLOR)
{
    out_color = tex2D(Samp, in_tex);
}

// --------------------------------------------------------------------------------------------------
// TECHNIQUES:
// --------------------------------------------------------------------------------------------------

technique TSimpleShader
{
    pass P0
    {
        //Wrap0 = U;  // useful when mesh is round like a sphere
        VertexShader = compile vs_1_1 VS();
        PixelShader  = compile ps_1_0 PS();
        FillMode = POINT;
        PointSpriteEnable = TRUE;
        PointScaleEnable = TRUE;
        PointSize_MIN = 0.0;
        PointScale_A = 0.0;
        PointScale_B = 0.0;
        PointScale_C = 1.0;
        
        ALPHABLENDENABLE = TRUE;
        SRCBLEND = ONE;
        DESTBLEND = ONE;
        ZWRITEENABLE = FALSE;
    }
}
