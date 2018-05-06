// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "custom/Geometry"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_color ("Main Color", Color) = (1,1,1,1) 
		_pivot ("_pivot", Vector) = (0,0,0,0) 
		_maxDistance ("_maxDistance", Float) = 1.0
		_animationT ("_animationT", Float) = 0.0
		_alphaTime ("_alphaTime", Float) = 1.0
		_wigglyness( "_wigglyness", Float) = 0.0
		_cubemap("_cubemap", Cube) = "white" {}
		[MaterialToggle] _isReflective("_isReflective", Float) = 0
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
            #pragma geometry geom
			//#pragma multi_compile_shadowcaster
           
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
			 samplerCUBE _cubemap;
            float4 _MainTex_ST;
            float4 _pivot;
			float _maxDistance;
			float _animationT;
			float _alphaTime;
			float _wigglyness;
			float4 _color;
			float _isReflective;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
 
			void create_onb( in float3 n, out float3 U, out float3 V)
			{
				 U = cross( n, float3( 0.0f, 1.0f, 0.0f ) );

				 if ( dot( U, U ) < 1e-3f )
				   U = cross( n, float3( 1.0f, 0.0f, 0.0f ) );

				 U = normalize( U );
				 V = cross( n, U );
			}

            [maxvertexcount(3)]
            void geom(triangle v2f input[3], inout TriangleStream<v2f> OutputStream)
            {
                v2f test = (v2f)0;
                float3 normal = normalize(cross(input[1].worldPosition.xyz - input[0].worldPosition.xyz, input[2].worldPosition.xyz - input[0].worldPosition.xyz));
				float3 center_dir = normalize(0.3333f*(input[1].worldPosition.xyz + input[0].worldPosition.xyz + input[2].worldPosition.xyz) - _pivot) ;
				float3 tangent, bitangent;
				create_onb(normal, tangent, bitangent);

                for(int i = 0; i < 3; i++)
                {
					float tt = max(0,_animationT);
                    test.normal = normal;
                    test.vertex = float4(input[i].worldPosition,1) + float4(center_dir,0) * _maxDistance * tt ;
					test.vertex = test.vertex + float4(_wigglyness*(sin(_Time.y) * tangent + cos(_Time.y) * bitangent),0) * tt;
					test.worldPosition = test.vertex.xyz;
					test.vertex = mul(UNITY_MATRIX_VP, test.vertex);
                    test.uv = input[i].uv;
                    OutputStream.Append(test);
                }
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _color * tex2D(_MainTex, i.uv);
				
				fixed3 camera = normalize(_WorldSpaceCameraPos.xyz - i.worldPosition.xyz);
				fixed3 refl = reflect(-camera,i.normal);
                float3 lightDir = float3(_WorldSpaceLightPos0.xyz);
                float ndotl = dot(i.normal, -normalize(lightDir));
				//return fixed4(1, 0, 0, 1);
				fixed4 c = col * max(ndotl,0.2) * 3;
				c.a = _alphaTime;
				fixed4 refl_color = texCUBElod(_cubemap, float4(refl,0));

				c.xyz = c.xyz * (1-_isReflective) + refl_color *_color *  _isReflective;

				return c;
            }
            ENDCG
        }
    }
	//Fallback "Diffuse"
}