// This is a premultiply-alpha adaptation of the built-in Unity shader "UI/Default" in Unity 5.6.2 to allow Unity UI stencil masking.

Shader "MyShader/Shader_Dissolve"
{
	Properties
	{
		 //_MainTex ("Sprite Texture", 2D) = "white" {}

		 [PerRendererData] _Tex ("Base (RGB)", 2D) = "white" { }
        _DissolveNoiseTex ("Dissolve Noise Texture", 2D) = "black" { }
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.0
        _DissolveColor ("Dissolve Color", Color) = (0, 0, 0, 1)
        _Cutoff ("Dissolve Cutoff", Range(0, 1)) = 0
		_Size ("_Size", Float) = 0

		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[Toggle(_CANVAS_GROUP_COMPATIBLE)] _CanvasGroupCompatible("CanvasGroup Compatible", Int) = 1

		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector] _Stencil ("Stencil ID", Float) = 0
		[HideInInspector][Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255

		[HideInInspector] _ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
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
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Normal"

		CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma shader_feature _ _CANVAS_GROUP_COMPATIBLE
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

            #include "UnityCG.cginc"
			#include "Assets/Spine/Runtime/spine-unity/Shaders/SkeletonGraphic/CGIncludes/Spine-SkeletonGraphic-NormalPass.cginc"

			struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
			};

            struct v2f
            {
                float4 pos : POSITION;
                float4 uv : TEXCOORD0;
                float4 color : COLOR;
				float localPos : TEXCOORD1;
            };

			sampler2D _Tex;  // Base texture for the skeleton
			float4 _Tex_ST;
            sampler2D _DissolveNoiseTex;  // Noise texture used for dissolving
			float4 _DissolveNoiseTex_ST;
            float _DissolveAmount;  // Controls the dissolve strength
            float4 _DissolveColor;  // Color of the dissolved part
            float _Cutoff;  // Threshold to control dissolve
			float _Size;

			v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv,_Tex);
				o.uv.zw = TRANSFORM_TEX(v.uv,_DissolveNoiseTex);
                o.color = v.color;
				o.localPos = v.vertex.xy;
                return o;
            }

            half4  frag (v2f i) : SV_Target
            {
				float2 Coord = lerp(i.uv.xy, i.localPos, _Size);
                 // Sample the main texture and noise texture
				 half4 texColor = tex2D(_Tex, Coord);
				 half noiseValue = tex2D(_DissolveNoiseTex, i.uv.zw).r;
 
				 // Apply dissolve based on the dissolve amount and noise texture
				 float dissolveFactor = smoothstep(_Cutoff - _DissolveAmount, _Cutoff + _DissolveAmount, noiseValue);
 
				 // If the dissolve factor is low, apply dissolve color; otherwise, show the original texture
				 if (dissolveFactor < 0.5)
				 {
					 texColor += (_DissolveColor * texColor.a);  // Use the dissolve color for dissolved parts
				 }
 
				 // Apply transparency based on dissolve factor
				 texColor.a *= dissolveFactor;
 
				 return texColor;
			}
			ENDCG
		}
	}
	CustomEditor "SpineShaderWithOutlineGUI"
}
