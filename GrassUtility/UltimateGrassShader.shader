Shader "Unlit/UltimateGrassShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Color2("C2", Color) = (1,1,1,1)
		_ParametersGrass("Parameters grass", Vector) = (1,1,1,1)
		_Wind("Wind direction", Vector) = (1,1,1,1)

	}
	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 100

		Pass
		{
			Cull Off 
			Tags{"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom 
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#pragma target 4.0

			half4 _Color;
			half4 _Color2; 
			half4 _Wind; 
			half4 _ParametersGrass; 


			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal :NORMAL; 
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
				float3 normal: TEXCOORD0; 
				float scale : TEXCOORD1; 
				half direction : TEXCOORD2; 
			};

			struct g2f
			{
				float4 pos: SV_POSITION; 
				float3 normal: TEXCOORD1; 
				float4 color : TEXCOORD0; 
			}; 

			half rand(half2 co)
			{
	    		return frac(sin(dot(co.xy ,half2(12.9898,78.233)))*43487.12654);
			}

		
			v2g vert (appdata v)
			{

				half r = rand(v.vertex.zz);

				v2g o; 
				o.vertex = v.vertex; 
				o.normal = v.normal; //r*float3(1,0,0) + (1-r)*float3(0,0,1); 
				o.scale = r*3; 
				o.direction = rand(v.vertex.xx);
				return o; 
			}

			[maxvertexcount(15)]
			void geom(point v2g IN[1], inout TriangleStream <g2f> ts)
			{

				float3 d = (float3(1.,0.,0.)*IN[0].direction + (1.-IN[0].direction)*float3(0.,0.,1.))*_ParametersGrass.x*IN[0].scale*_ParametersGrass.z; 
				float3 h = float3(0,1.,0)*_ParametersGrass.y*IN[0].scale*_ParametersGrass.z; 

				float3 w = normalize( _Wind.xyz)*_Wind.w*sin(_Time[2]*_ParametersGrass.w + IN[0].vertex.x);
				float3 v1 = IN[0].vertex.xyz + d; 
				float3 v2 = v1 + h; 
				float3 v3 = v2 - d; 

				float3 v4 = v2 + h + w*0.2; 
				float3 v5 = v3 + h + w*0.2; 

				// float3 v6 = v4  + h + w*0.2; 
				// float3 v7 = v5  + h + w*0.2; 

				float3 v8 = v4 + h/2 -d/2. + w*0.25; 

				float3 normal = IN[0].normal; //UnityObjectToWorldNormal(IN[0].normal);
				g2f o; 

				o.pos = UnityObjectToClipPos(IN[0].vertex.xyz); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 0.); 
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v1); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 0.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v3); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 1/3.); 
				ts.Append(o); 

				ts.RestartStrip();

				o.pos = UnityObjectToClipPos(v1); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 0.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v2); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 1/3.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v3); 
				o.normal = normal; 
				o.color =lerp(_Color2, _Color, 1/3.);  
				ts.Append(o); 

				ts.RestartStrip(); 

				o.pos = UnityObjectToClipPos(v3); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 1/3.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v2); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 1/3.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v5); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 2/3.);  
				ts.Append(o); 

				ts.RestartStrip(); 

				o.pos = UnityObjectToClipPos(v2); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 1/3.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v4); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 2/3.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v5); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color,2/3.);  
				ts.Append(o); 

				ts.RestartStrip(); 

				o.pos = UnityObjectToClipPos(v5); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 2/3.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v4); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 2/3.);  
				ts.Append(o); 

				o.pos = UnityObjectToClipPos(v8); 
				o.normal = normal; 
				o.color = lerp(_Color2, _Color, 1.);  
				ts.Append(o); 
			}
			
			fixed4 frag (g2f i) : COLOR
			{
				float3 norm = UnityObjectToWorldNormal(i.normal); 
				fixed3 diffuseLight = saturate(dot(norm, _WorldSpaceLightPos0.xyz))*_LightColor0;  //UnityWorldSpaceLightDir(i.pos)

				fixed4 c = i.color; 
				c.rgb *= diffuseLight; 
				
				return c; 
			}
			ENDCG
		}
	}
}
