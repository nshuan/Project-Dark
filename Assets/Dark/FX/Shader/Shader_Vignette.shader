Shader "MyShader/VignetteOverlay"
{
    Properties
    {
        [PerRendererData] _MainTex("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _Radius ("Radius", Range(0,1)) = 0.5
        _Smoothness ("Smoothness", Range(0.001,0.5)) = 0.25
        _Roundness ("Roundness", Range(0.2,2.0)) = 1.0

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
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
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
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            fixed4 _Color;
            float _Radius;
            float _Smoothness;
            float _Roundness;
            float4 _ClipRect;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; fixed4 color : COLOR; float4 worldPosition : TEXCOORD1; };

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // uv in 0..1
                float2 uv = i.uv - 0.5;
                // correct shape using roundness
                uv.x *= _Roundness;
                float d = length(uv);

                // compute smooth vignette: 0 at center -> 1 at edge
                float edge0 = _Radius - _Smoothness;
                float edge1 = _Radius;
                float t = smoothstep(edge0, edge1, d);

                // output overlay color with alpha = t
                fixed4 outc = _Color * i.color;
                outc.a *= t;

                #ifdef UNITY_UI_CLIP_RECT
                    outc.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip(outc.a - 0.001);
                #endif

                return outc;
            }
            ENDCG
        }
    }
}
