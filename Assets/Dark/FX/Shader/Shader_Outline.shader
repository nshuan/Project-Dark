Shader "MyShader/Shader_Outline"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_Outline_Size_1("_Outline_Size_1", Range(0, 3)) = 1
_Outline_Color_1("_Outline_Color_1", COLOR) = (1,1,1,1)
_Outline_HDR_1("_Outline_HDR_1", Range(0, 2)) = 1
_Speed_1("_Speed_1", Float) = 0
_MinValue("_MinValue", Float) = 0
_MaxValue("_MaxValue", Float) = 1
_StrokeOpacity("_StrokeOpacity", Range(0,1)) = 1
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

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
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

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

sampler2D _MainTex;
float _SpriteFade;
float _Outline_Size_1;
float4 _Outline_Color_1;
float _Outline_HDR_1;
float _Speed_1;
float _MinValue;
float _MaxValue;
float _StrokeOpacity;

v2f vert(appdata_t IN)
{
    v2f OUT;
    OUT.vertex = UnityObjectToClipPos(IN.vertex);
    OUT.texcoord = IN.texcoord;
    OUT.color = IN.color;
    return OUT;
}


float4 OutLine(float2 uv,sampler2D source, float value, float4 color, float HDR, float speed, float minValue, float maxValue, float opacity)
{
    float4 mainColor = tex2D(source, uv);
    if (opacity < 0.5)
    return mainColor;

    float pulse = sin(_Time.y * speed) * 0.5 + 0.5; // [0,1]
    value = lerp(minValue, maxValue, pulse) * 0.01;
    mainColor = tex2D(source, uv + float2(-value, 0))
    + tex2D(source, uv + float2(value, 0))
    + tex2D(source, uv + float2(0, -value))
    + tex2D(source, uv + float2(0, value))
    + tex2D(source, uv + float2(-value, value))
    + tex2D(source, uv + float2(value, -value))
    + tex2D(source, uv + float2(value, value))
    + tex2D(source, uv - float2(value, value));

    color *= HDR;
    mainColor.rgb = color;
    float4 addcolor = tex2D(source, uv);
    if (addcolor.a > 0.40) { mainColor = addcolor; mainColor.a = addcolor.a; }
    if (addcolor.a < 0.1 && mainColor.a > 0.4) { return float4(color.rgb * HDR, 1.0);}
    return mainColor;
}
float4 frag (v2f i) : COLOR
{
    float4 _Outline_1 = OutLine(i.texcoord,_MainTex,_Outline_Size_1,_Outline_Color_1,_Outline_HDR_1, _Speed_1, _MinValue, _MaxValue, _StrokeOpacity);
    float4 FinalResult = _Outline_1;
    FinalResult.rgb *= i.color.rgb;
    FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
    return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
