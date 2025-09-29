Shader "MyShader/Shader_Fresnel" {
    //show values to edit in inspector
    Properties {
        _Color ("Tint", Color) = (0, 0, 0, 0)
        _MainTex ("Texture", 2D) = "white" {}
        //_Smoothness ("Smoothness", Range(0, 1)) = 0
        //_Metallic ("Metalness", Range(0, 1)) = 0
        [HDR] _Emission ("Emission", color) = (0,0,0)
        _Threshold("Cutout threshold", Float) = 0.1
        _Softness("Cutout softness", Float) = 0.0
        
        _FresnelAlphaPower ("Fresnel Alpha Power", Range(0.1, 4)) = 1
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        [PowerSlider(4)] _FresnelExponent ("Fresnel Exponent", Range(0.25, 4)) = 1
    }
    SubShader {
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend One One

        CGPROGRAM

        //the shader is a surface shader, meaning that it will be extended by unity in the background to have fancy lighting and other features
        //our surface shader function is called surf and we use the standard lighting model, which means PBR lighting
        //fullforwardshadows makes sure unity adds the shadow passes the shader might need
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;

        //half _Smoothness;
        //half _Metallic;
        half3 _Emission;

        float3 _FresnelColor;
        float _FresnelExponent;
        float _FresnelAlphaPower;

        float _Threshold;
        float _Softness;

        //input struct which is automatically filled by unity
        struct Input {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            fixed4 color : COLOR;
            INTERNAL_DATA
        };

        //the surface shader function which sets parameters the lighting function then uses
        void surf (Input i, inout SurfaceOutputStandard o) {
            //sample and tint albedo texture
            fixed4 col = tex2D(_MainTex, i.uv_MainTex);
            col *= _Color;
            o.Albedo = col.rgb;
            // Calculate Fresnel
            float fresnel = dot(normalize(i.worldNormal), normalize(i.viewDir));
            fresnel = saturate(1.0 - fresnel);
            fresnel = pow(fresnel, _FresnelExponent);

            // Dùng fresnel để điều khiển alpha
            float baseAlpha = i.color.a * smoothstep(_Threshold, _Threshold + _Softness, 0.333 * (col.r + col.g + col.b));
            o.Alpha = baseAlpha * pow(fresnel, _FresnelAlphaPower);

            //o.Metallic = _Metallic;
            //o.Smoothness = _Smoothness;

            float3 fresnelColor = fresnel * _FresnelColor;
            o.Emission = _Emission + fresnelColor;
        }
        ENDCG
    }
    FallBack "Standard"
}