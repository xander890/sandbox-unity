// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "custom/ShowKinectColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_alphaColor ("_alphaColor", Range(0,1)) = 1.0
		_glowcolor("Glow color", Color) = (1,0,0,0)
		_glowsize("Glow size", Float) = 0.01
		_modscales ("_modscales", Vector) = (0,0,1,1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
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
			sampler2D _Depth;
			float4 _MainTex_ST;
			float4x4 _Homography;
			float4 _modscales;
			float2 _KinectDims;
			float _alphaColor;
			float4 rectangle_points[4];
			int _enable_rect;
			float _glowsize;
			float4 _glowcolor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
 
           
			float point_line_segment_distance(float2 s1, float2 s2, float2 p)
			{
				float2 diff = s2-s1;
				float t = dot(p - s1, diff) / dot(diff, diff);
				t = clamp(t,0.0f,1.0f);
				float2 min_point = s1 + t * diff;
				return length(p - min_point);	
			}

            fixed4 frag (v2f i) : SV_Target
            {
				// sample the texture
                fixed4 c = tex2D(_MainTex, i.uv);
				float2 uv = i.uv;
				if(_enable_rect == 1)
				{
					float d = 200.0f;
					for(int i = 0; i < 4; i++)
					{
						float2 p0 = rectangle_points[i].xy;
						p0 = (p0 - _modscales.xy) / _modscales.zw;
						p0 = (p0 + float2(1,1)) / 2;
						float2 p1 = rectangle_points[(i+1)%4].xy;
						p1 = (p1 - _modscales.xy) / _modscales.zw;
						p1 = (p1 + float2(1,1)) / 2;
						d = min(d, point_line_segment_distance(p0,p1,uv));
					}
					d = exp(-d / _glowsize); 
					d = clamp(d,0,1);
					c = c * (1 - d) + d * _glowcolor;
				}
				c.a = 1;
				return c;
            }
            ENDCG
        }
    }
	//Fallback "Diffuse"
}