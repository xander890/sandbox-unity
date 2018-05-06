// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "custom/ShowKinectColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		LOD 200
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
			#include "AutoLight.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
            };
 
            sampler2D _MainTex;
			float4 _MainTex_ST;
			float4x4 _Homography;
			float2 _KinectDims;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
 
           
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 c = tex2D(_MainTex, i.uv);
				fixed3 pixel = fixed3(i.vertex.xy,1.0f);
				float3x3 hh = _Homography;
				fixed3 t = mul(hh, pixel);
				fixed2 uuv = t.xy;
				c = tex2D(_MainTex,fixed2(0,1) + fixed2(1,-1)*i.uv);
				return c;
            }
            ENDCG
        }
    }
	//Fallback "Diffuse"
}