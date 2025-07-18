
Shader "MyShader/Shader_Noise_Distortion"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Speed ("Speed", Vector) = (1, 1, 1, 1)

        _MainNoise ("Noise Texture", 2D) = "white" {}
        _NoiseValue ("Noise Value", Float) = 0.1
        _NoiseSpeed ("Noise Speed", Vector) = (1, 1, 1, 1)


        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex           : SV_POSITION;
                fixed4 color            : COLOR;
                float2 texcoord         : TEXCOORD0;
                float4 worldPosition    : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            sampler2D _MainNoise;
            float4 _MainNoise_ST;
            float _NoiseValue;
            float2 _Speed;
            float2 _NoiseSpeed;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 noiseColor = tex2D(
                    _MainNoise, IN.texcoord * _MainNoise_ST.xy + _MainNoise_ST.zw + _NoiseSpeed * _Time);
                half2 noiseUV = (noiseColor + IN.texcoord) / _NoiseValue;
                half4 color = (tex2D(_MainTex, noiseUV * _MainTex_ST.xy + _MainNoise_ST.zw + _Speed * _Time)) * IN.color;


                #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}