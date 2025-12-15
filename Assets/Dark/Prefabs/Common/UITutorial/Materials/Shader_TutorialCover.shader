Shader "MyShader/RoundedRectHoleOverlay"
{
    Properties
    {
        _Color ("Overlay Color", Color) = (0,0,0,0.8)
        _Alpha ("Overlay Alpha", Range(0,1)) = 1

        _Center ("Hole Center (UV 0-1)", Vector) = (0.5, 0.5, 0, 0)
        _RadiusXY ("Hole Size X/Y (UV)", Vector) = (0.2, 0.2, 0, 0)

        _Roundness ("Roundness (2=circle, bigger=rect)", Range(2, 64)) = 2
        _Softness ("Edge Softness", Range(0.0001, 0.5)) = 0.05

        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex:POSITION; float2 texcoord:TEXCOORD0; fixed4 color:COLOR; };
            struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };

            fixed4 _Color;
            float _Alpha;

            float4 _Center;      // xy
            float4 _RadiusXY;    // xy
            float _Roundness;    // exponent p
            float _Softness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color * i.color;
                col.a *= _Alpha;

                // delta normalized by size
                float2 d = (i.uv - _Center.xy) / max(_RadiusXY.xy, 1e-6);

                // Superellipse distance:
                // p=2 => circle/ellipse, p -> large => rectangle-ish
                float p = max(_Roundness, 2.0);
                float dx = pow(abs(d.x), p);
                float dy = pow(abs(d.y), p);
                float dist = pow(dx + dy, 1.0 / p);

                // dist < 1 => inside hole (transparent)
                float mask = smoothstep(1.0, 1.0 + _Softness, dist);
                col.a *= mask;

                return col;
            }
            ENDCG
        }
    }
}
