Shader "MyShader/Shader_MetalShiny"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
[PerRendererData] _SecondaryTex ("Base (RGB)", 2D) = "white" { }
_TurnMetal("_TurnMetal", Range(-8, 8)) = 1
_MetalFade("_Transparent",Range(0, 1)) = 1
_Speed("_Speed", Float) = 0
_EffectScale ("Size Scale", Float) = 2.5
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
[Toggle(_CANVAS_GROUP_COMPATIBLE)] _CanvasGroupCompatible("CanvasGroup Compatible", Int) = 1

[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8
[HideInInspector] _Stencil ("Stencil ID", Float) = 0
[HideInInspector][Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
[HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
[HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
[HideInInspector] _ColorMask ("Color Mask", Float) = 15
[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

// Outline properties are drawn via custom editor.
[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0

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
Blend SrcAlpha OneMinusSrcAlpha

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

CGPROGRAM
#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
#pragma shader_feature _ _CANVAS_GROUP_COMPATIBLE
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"
#include "Assets/Spine/Runtime/spine-unity/Shaders/SkeletonGraphic/CGIncludes/Spine-SkeletonGraphic-NormalPass.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
float2 worldPos : TEXCOORD1;
UNITY_VERTEX_OUTPUT_STEREO
};

//sampler2D _MainTex;
sampler2D _SecondaryTex;

float _SpriteFade;
float _TurnMetal;
float _MetalFade;
float _Speed;
float _EffectScale;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
OUT.worldPos = IN.vertex.xy;
return OUT;
}


float4 ColorTurnMetal(float2 uv, float2 pos, sampler2D txt, sampler2D mainTex, float motion, float speed, float size)
{

float4 txt1 = tex2D(txt,uv);
float4 txtMain = tex2D(mainTex,uv);
float4 txtFin = txt1 * txtMain;

float2 shimmerCoord = pos * size;
float loopMotion = fmod(motion, 6.2831853) + (_Time.yy * speed);
float a = loopMotion;
float n = sin(a + 2.0 * shimmerCoord.x) + 
          sin(a - 2.0 * shimmerCoord.x) + 
          sin(a + 2.0 * shimmerCoord.y) + 
          sin(a + 5.0 * shimmerCoord.y);

n = fmod(((5.0 + n) / 5.0), 1.0);
n += tex2D(txt, uv).r * 0.21 + tex2D(txt, uv).g * 0.4 + tex2D(txt, uv).b * 0.2;
n = fmod(n,1.0);

float tx = n * 6.0;
float r = clamp(tx - 2.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);

float4 sortie = float4(txtFin.r, txtFin.g, txtFin.b, r);
sortie.rgb = txtFin.rgb + ((1-sortie.a) * _MetalFade);
sortie.a = txtFin.a;
return sortie;
}
float4 frag (v2f i) : COLOR
{
float4 _TurnMetal_1 = ColorTurnMetal(i.texcoord, i.worldPos, _MainTex, _SecondaryTex, _TurnMetal, _Speed, _EffectScale);
float4 FinalResult = _TurnMetal_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
CustomEditor "SpineShaderWithOutlineGUI"
}
