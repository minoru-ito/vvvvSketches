// this is a very basic template. use it to start writing your own effects.
// if you want effects with lighting start from one of the GouraudXXXX or PhongXXXX effects

// --------------------------------------------------------------------------------------------------
// PARAMETERS:
// --------------------------------------------------------------------------------------------------

//transforms
float4x4 tW: WORLD;        //the models world matrix
float4x4 tV: VIEW;         //view matrix as set via Renderer (EX9)
float4x4 tP: PROJECTION;
float4x4 tWVP: WORLDVIEWPROJECTION;

//texture
texture Tex <string uiname="Texture";>;
sampler Samp = sampler_state    //sampler for doing the texture-lookup
{
    Texture   = (Tex);          //apply a texture to the sampler
    MipFilter = NONE;//ito edited         //sampler states
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;//ito added
    AddressV = CLAMP;//ito added
};

//texture transformation marked with semantic TEXTUREMATRIX to achieve symmetric transformations
float4x4 tTex: TEXTUREMATRIX <string uiname="Texture Transform";>;

float4 g_offset;//ito added
float4 g_direction;//ito added

//the data structure: "vertexshader to pixelshader"
//used as output data with the VS function
//and as input data with the PS function
struct vs2ps
{
    float4 Pos  : POSITION;
    float2 TexCd : TEXCOORD0;
};

// --------------------------------------------------------------------------------------------------
// VERTEXSHADERS
// --------------------------------------------------------------------------------------------------
vs2ps VS(
    float4 PosO  : POSITION,
    float4 TexCd : TEXCOORD0)
{
    //declare output struct
    vs2ps Out;

    //transform position
    Out.Pos = mul(PosO, tWVP);
    
    //transform texturecoordinates
    Out.TexCd = TexCd;//ito edited mul(TexCd, tTex);

    return Out;
}

// --------------------------------------------------------------------------------------------------
// PIXELSHADERS:
// --------------------------------------------------------------------------------------------------

float4 PS(vs2ps In): COLOR
{
    In.TexCd.x = In.TexCd.x + g_offset.x;
    In.TexCd.y = In.TexCd.y + g_offset.y;
    float4 color = 0;
    float2 tex;
    float jump = 8.0;
    float weight = 1.0;
    float coef = 0.1;
    float att = 0.4;
    
    for(int i=0; i<8; i++){
      tex = In.TexCd + (g_direction.xy * jump * g_offset.xy * 2.0 * i);
      weight -= coef;
      color += weight * tex2D(Samp, tex);
    }
    
    color = color * att;
    return color;
    
    //float4 col = tex2D(Samp, In.TexCd);
    //return col;
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
        PixelShader  = compile ps_2_0 PS();//ito edited 1_0->2_0
    }
}
