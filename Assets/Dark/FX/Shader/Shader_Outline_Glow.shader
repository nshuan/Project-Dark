Shader "MyShader/Shader_Outline_Glow"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

        // Outline
        _OutlineColor("Outline Color", Color) = (1,0.3,0,1)
        _OutlineSize("Outline Size", Range(0, 10)) = 1.0
        _OutlinePrecision("Outline Precision", Range(1, 32)) = 8

        // Outer Glow
        _OuterGlowColor("Outer Glow Color", Color) = (0.3,0.8,1,1)
        _OuterGlowSize("Outer Glow Size", Range(0, 20)) = 5
        _OuterGlowIntensity("Outer Glow Intensity", Range(0, 4)) = 1.5
        _OuterGlowPrecision("Outer Glow Precision", Range(1, 32)) = 10
        _OuterGlowSpeed("Outer Glow Pulse Speed", Float) = 0.3
        _OuterGlowBlurAmount("Outer Glow Blur Amount", Range(0.5, 10)) = 3

        // Sprite
        _SpriteFade("Sprite Fade", Range(0, 1)) = 1.0

        // UI Mask
        [HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }

        ZWrite Off
        Cull Off

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
            Name "OUTLINE"
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineSize;
            float _OuterGlowIntensity;
            float _OuterGlowSpeed;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {

                float pulse = (sin(_Time.y * _OuterGlowSpeed) * 0.5 + 0.5);

                float sizePulse = lerp(0.9, 1.15, pulse);

                float2 texel = _MainTex_TexelSize.xy;
                float2 offset = texel * _OutlineSize * sizePulse;

                float a = 0.0;
                a += tex2D(_MainTex, i.uv + float2( offset.x,  0)).a;
                a += tex2D(_MainTex, i.uv + float2(-offset.x,  0)).a;
                a += tex2D(_MainTex, i.uv + float2( 0,  offset.y)).a;
                a += tex2D(_MainTex, i.uv + float2( 0, -offset.y)).a;
                a += tex2D(_MainTex, i.uv + float2( offset.x,  offset.y)).a;
                a += tex2D(_MainTex, i.uv + float2(-offset.x,  offset.y)).a;
                a += tex2D(_MainTex, i.uv + float2( offset.x, -offset.y)).a;
                a += tex2D(_MainTex, i.uv + float2(-offset.x, -offset.y)).a;

                float baseAlpha = tex2D(_MainTex, i.uv).a;

                float neigh = a * (1.0 / 8.0);

                float outline = saturate(neigh - baseAlpha);

                outline = smoothstep(0.02, 0.6, outline);

                float4 col = _OutlineColor;

                float intensity = _OuterGlowIntensity * (0.6 + 0.8 * pulse);
                col.rgb *= intensity;
                col.a = outline * _OuterGlowIntensity * 0.9;

                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "OUTERGLOW"
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOuterGlow
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float4 _OuterGlowColor;
            float _OuterGlowSize;
            float _OuterGlowIntensity;
            int _OuterGlowPrecision;
            float _OuterGlowSpeed;
            float _OuterGlowBlurAmount;

            v2f vert(appdata_t IN)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(IN.vertex);
                o.uv = IN.uv;
                return o;
            }

            float4 fragOuterGlow(v2f i) : SV_Target
            {
                int samples = max(1, _OuterGlowPrecision);
                int halfSamples = samples / 2;
                float4 sumColor = 0;
                float alphaSum = 0;

                for (int y = -halfSamples; y <= halfSamples; y++)
                {
                    for (int x = -halfSamples; x <= halfSamples; x++)
                    {
                        float2 offset = float2(x, y) / samples * _OuterGlowSize * 0.1;
                        float4 c = tex2D(_MainTex, saturate(i.uv + offset));
                        if (c.a > 0.001)
                        {
                            float dist = length(float2(x, y)) / samples;
                            float weight = exp(-dist * dist * _OuterGlowBlurAmount);
                            sumColor += c * c.a * weight;
                            alphaSum += c.a * weight;
                        }
                    }
                }

                if (alphaSum > 0) sumColor /= alphaSum;
                float pulse = (sin(_Time.y * _OuterGlowSpeed) * 0.5 + 0.5);
                float intensity = _OuterGlowIntensity * pulse;

                float4 glow = sumColor;
                glow.rgb = _OuterGlowColor.rgb * glow.a * intensity;
                glow.a = glow.a * intensity * 0.7;
                return glow;
            }
            ENDCG
        }

        Pass
        {
            Name "BASE"
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBase
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };

            sampler2D _MainTex;
            float _SpriteFade;

            v2f vert(appdata_t IN)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(IN.vertex);
                o.uv = IN.uv;
                o.color = IN.color;
                return o;
            }

            float4 fragBase(v2f i) : SV_Target
            {
                float4 c = tex2D(_MainTex, i.uv) * i.color;
                c.a *= _SpriteFade;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
