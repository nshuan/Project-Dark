//////////////////////////////////////////////
/// 2D Shader Collection - by VETASOFT 2018 //
//////////////////////////////////////////////


//////////////////////////////////////////////

Shader "MyShader/Shader_Wave"
{
Properties
{
//[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
[PerRendererData] _Tex ("Sprite Texture", 2D) = "white" { }
LiquidUV_WaveX_1("LiquidUV_WaveX_1", Range(0, 2)) = 2
LiquidUV_WaveY_1("LiquidUV_WaveY_1", Range(0, 2)) = 2
LiquidUV_DistanceX_1("LiquidUV_DistanceX_1", Range(0, 1)) = 0.3
LiquidUV_DistanceY_1("LiquidUV_DistanceY_1", Range(0, 1)) = 0.3
LiquidUV_Speed_1("LiquidUV_Speed_1", Range(-2, 2)) = 1
Effect_Fade("Effect_Fade", Range(0, 1)) = 1
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

[Enum(UnityEngine.Rendering.BlendMode)]
_SrcFactor("Src Factor", Float) = 5
[Enum(UnityEngine.Rendering.BlendMode)]
_DstFactor("Dst Factor", Float) = 10
[Enum(UnityEngine.Rendering.BlendOp)]
_Opp("Operation", Float) = 0

[Toggle(_CANVAS_GROUP_COMPATIBLE)] _CanvasGroupCompatible("CanvasGroup Compatible", Int) = 1

// required for UI.Mask
[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector][Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15
[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

}

SubShader
{

Tags
{
	"Queue"="Transparent"
	"IgnoreProjector"="True"
	"RenderType"="Transparent"
	"PreviewType"="Plane"
	"CanUseSpriteAtlas"="True"
}
Cull Off
Lighting Off
ZWrite Off
ZTest [unity_GUIZTestMode]
Fog { Mode Off }
LOD 100
Blend [_SrcFactor] [_DstFactor]
BlendOp [_Opp]
ColorMask [_ColorMask]

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}



Pass
{
    Name "Normal"

CGPROGRAM
#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
#pragma shader_feature _ _CANVAS_GROUP_COMPATIBLE
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
#include "Assets/Spine/Runtime/spine-unity/Shaders/SkeletonGraphic/CGIncludes/Spine-SkeletonGraphic-NormalPass.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _Tex;
float _SpriteFade;
float LiquidUV_WaveX_1;
float LiquidUV_WaveY_1;
float LiquidUV_DistanceX_1;
float LiquidUV_DistanceY_1;
float LiquidUV_Speed_1;
float Effect_Fade;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 LiquidUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{ Speed *= _Time * 100;
float x = sin(p.y * 4 * WaveX + Speed);
float y = cos(p.x * 4 * WaveY + Speed);
x += sin(p.x)*0.1;
y += cos(p.y)*0.1;
x *= y;
y *= x;
x *= y + WaveY*8;
y *= x + WaveX*8;
p.x = p.x + x * DistanceX * 0.015;
p.y = p.y + y * DistanceY * 0.015;

return p;
}
float4 frag (v2f i) : COLOR
{
float2 LiquidUV_1 = LiquidUV(i.texcoord,LiquidUV_WaveX_1,LiquidUV_WaveY_1,LiquidUV_DistanceX_1,LiquidUV_DistanceY_1,LiquidUV_Speed_1);
i.texcoord = lerp(i.texcoord,LiquidUV_1,Effect_Fade);
float4 _MainTex_1 = tex2D(_MainTex,i.texcoord);
float4 FinalResult = _MainTex_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
CustomEditor "SpineShaderWithOutlineGUI"
Fallback "Sprites/Default"
}
