Shader "MyShader/Shader_LinearMask"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_MaskMode("Mask Mode (0=Horizontal, 1=Vertical)", Range(0, 1)) = 0
_LinearProgress("_LinearProgress", Range(0, 1)) = 1
_LinearFeather("Feather", Range(0, 0.5)) = 0.05
_InvertDirection("Invert Direction", Float) = 0
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
float2 uv : TEXCOORD0;
};

struct v2f
{
float2 uv  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float4 _MainTex_ST;
float _SpriteFade;
float _MaskMode;
float _LinearProgress;
float _LinearFeather;
float _InvertDirection;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.uv = IN.uv;
OUT.color = IN.color;
return OUT;
}

float4 frag (v2f i) : COLOR
{
    float2 uv = i.uv;
    fixed4 col = tex2D(_MainTex, uv);

    // ==============================
    // LINEAR MASK (HORIZONTAL or VERTICAL)
    // ==============================
    // Horizontal: 0 = left edge, 1 = right edge. Progress reveals left-to-right (or inverted).
    // Vertical:   0 = bottom, 1 = top. Progress reveals bottom-to-top (or inverted).

    float coord = lerp(uv.x, uv.y, _MaskMode);
    if (_InvertDirection > 0.5)
        coord = 1.0 - coord;

    float feather = max(_LinearFeather, 1e-5);
    // coord < progress -> visible; coord > progress -> masked
    float t = (_LinearProgress - coord) / feather;
    float linearMask = saturate(t);

    col.a *= linearMask;
    col.a *= _SpriteFade;

    return col * i.color;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
