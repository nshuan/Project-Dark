Shader "MyShader/GlassBreak2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BreakCenter ("Break Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _BreakTime ("Break Progress", Range(0,1)) = 0
        _CrackScale ("Crack Scale", Float) = 10
        _ScatterAmount ("Scatter Amount", Float) = 0.3
        _ShowSilhouette ("Show Silhouette", Range(0,1)) = 0
        _SilhouetteColor ("Silhouette Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _BreakCenter;
            float _BreakTime;
            float _CrackScale;
            float _ScatterAmount;
            float _ShowSilhouette;
            float4 _SilhouetteColor;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color    : COLOR;
            };

            // Simple hash for random offset per fragment
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 dir = uv - _BreakCenter;
                float randomFactor = hash21(floor(uv * _CrackScale));
                float2 offset = normalize(dir + 0.001) * randomFactor * _ScatterAmount * _BreakTime;
                float2 shatteredUV = uv + offset;

                fixed4 texCol = tex2D(_MainTex, shatteredUV);
                float alpha = texCol.a;
                float fade = saturate(1.0 - _BreakTime * 1.5);
                alpha *= fade;

                fixed4 silhouetteColor = _SilhouetteColor;
                silhouetteColor.a *= alpha * i.color.a;

                // Blend between texture and silhouette
                return lerp(texCol, silhouetteColor, _ShowSilhouette);
            }

            ENDCG
        }
    }
}
