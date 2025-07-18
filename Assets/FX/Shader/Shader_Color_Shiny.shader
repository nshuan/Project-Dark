
Shader "MyShader/Shader_Color_Shiny"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

        _Size("_Size", Range(-1, 1)) = -0.1
        _Smooth("_Smooth", Range(0, 1)) = 0.25
        _Intensity("_Intensity", Range(0, 4)) = 1
        _Rotation("_Rotation", Range(0, 360)) = 0
        _Position("_Position", Float) = 0
        _Color("Color", Color) = (0,0,0,0)

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

            struct appdata_t
            {
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
            float _Size;
            float _Smooth;
            float _Intensity;
            float _Position;
            float _Rotation;
            float4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            float2 RotationUV(float rot, float2 uv)
            {
                float rad = radians(rot);
                float cosR = cos(rad);
                float sinR = sin(rad);

                uv -= 0.5;

                float2 rotatedUV;
                rotatedUV.x = uv.x * cosR - uv.y * sinR;
                rotatedUV.y = uv.x * sinR + uv.y * cosR;

                return rotatedUV + 0.5;
            }

             float4 ShinyFX(float4 txt, float2 uv, float pos, float size, float smooth, float intensity, float rotation)
            {
                uv = RotationUV(rotation, uv);
                pos = pos + 0.5;
                uv = uv - float2(pos, 0.5);
                float a = atan2(uv.x, uv.y) + 1.4, r = 3.1415;
                float d = cos(floor(0.5 + a / r) * r - a) * length(uv);
                float dist = 1.0 - smoothstep(size, size + smooth, d);
                txt.rgb += dist * intensity * _Color;
                return txt;
            }

             float4 frag(v2f i) : COLOR
            {
                float4 _MainTex_1 = tex2D(_MainTex, i.texcoord);
                float4 _ShinyFX_1 = ShinyFX(_MainTex_1, i.texcoord, _Position, _Size, _Smooth, _Intensity, _Rotation);
                float4 FinalResult = _ShinyFX_1;
                FinalResult.rgb *= i.color.rgb;
                FinalResult.a = FinalResult.a * i.color.a;
                return FinalResult;
            }

        ENDCG
        }
    }
    Fallback "Sprites/Default"
}
