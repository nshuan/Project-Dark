Shader "MyShader/Shader_RadialMask"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_RadialCenter("_RadialCenter", Vector) = (0.5, 0.5, 0, 0)
_RadialProgress("_RadialProgress", Range(-360, 0)) = 0
_RadialFeather("_RadialFeather", Range(0, 0.5)) = 0.05
_RadialOffsetAngle("_RadialOffsetAngle", Range(-180, 180)) = -90
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
float4 _RadialCenter;
float _RadialProgress;
float _RadialFeather;
float _RadialOffsetAngle;

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
    // RADIAL WIPE (KIỂU ĐỒNG HỒ NGƯỢC)
    // ==============================

    // tâm radial theo UV
    float2 center = _RadialCenter.xy;

    // vector từ tâm đến pixel
    float2 dir = uv - center;

    // góc [-PI, PI]
    float angle = atan2(dir.y, dir.x); 

    // offset góc (deg -> rad), ví dụ -90 để bắt đầu từ đỉnh (12h)
    float offsetRad = radians(_RadialOffsetAngle);
    angle += offsetRad;

    // chuẩn hóa về [0, 2PI)
    const float TWO_PI = 6.28318530718;
    angle = fmod(angle + TWO_PI, TWO_PI);

    // chuẩn hóa về [0,1]: 0..1 tương ứng 0..360°
    float angle01 = angle / TWO_PI;

    // ĐẢO CHIỀU XOAY: dùng 1 - angle01 để quay ngược
    angle01 = 1.0 - angle01;

    // _RadialProgress: 0..-360 → chuyển thành 0..1 theo độ lớn
    // 0   -> 0.0  (không fill)
    // -180 -> 0.5 (nửa vòng)
    // -360 -> 1.0 (full vòng)
    float prog = saturate(-_RadialProgress / 360.0);

    // feather tránh mép cứng
    float feather = max(_RadialFeather, 1e-5);

    // angle01 < prog - feather → mask ~ 1
    // angle01 > prog           → mask ~ 0
    float t = (prog - angle01) / feather;
    float radialMask = saturate(t);

    // áp dụng mask vào alpha
    col.a *= radialMask;

    // nếu có fade tổng: 
    col.a *= _SpriteFade;

    return col * i.color;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
