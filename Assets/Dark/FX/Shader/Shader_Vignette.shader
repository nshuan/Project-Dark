Shader "MyShader/VignetteOverlay"
{
    Properties
    {
        [PerRendererData] _MainTex("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _Radius ("Radius", Range(0,1)) = 0.5
        _Smoothness ("Smoothness", Range(0.001,0.5)) = 0.25
        _Roundness ("Roundness", Range(0.2,2.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Radius;
            float _Smoothness;
            float _Roundness;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; fixed4 color : COLOR; };

            v2f vert (appdata v)
            {
                v2f o;
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
                fixed4 outc = _Color * i.color;;
                outc.a *= t;
                return outc;
            }
            ENDCG
        }
    }
}
