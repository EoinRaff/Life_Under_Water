// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Plastic" {
   Properties {
	  _Strength("Wave Strength", Range(0, 2)) = 1.0
	  _Speed("Wave Speed", Range(-200, 200)) = 100
	  _Direction("Wave Direction", Vector) = (0.0, 1.0, 0.0, 0.0)
      _Color("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
      _MainTex ("Diffuse Texture", 2D) = "white" {} 
	  _BumpMap ("Normal Texture", 2D) = "bump" {} 
	  _SpecMap ("Gloss Texture", 2D) = "white" {} 
	  _BumpDepth("Bump Depth", Range(-2, 2)) = 1.0
	  _SpecColor("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
	  _Shininess("Shininess", Float) = 10
	  _RimColor("RimColor", Color) = (1.0, 1.0, 1.0, 1.0)
	  _RimPower("RimPower", Range(0.1, 10)) = 3.0
   }
   SubShader {
      Pass {	
	     Tags {"Lightmode" = "ForwardBase"}
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
		 // User defined variables
         uniform sampler2D _MainTex;
         uniform sampler2D _BumpMap;		
         uniform sampler2D _SpecMap;		
         uniform float4 _MainTex_ST; //used for offset
         uniform float4 _BumpMap_ST; //used for offset
         uniform float4 _SpecMap_ST; //used for offset
		 uniform float4 _Color;
		 uniform float4 _SpecColor;
		 uniform float4 _RimColor;
		 uniform float _Shininess;
		 uniform float _RimPower;
		 uniform float _BumpDepth;
		 uniform float _Strength;
		 uniform float _Speed;
		 uniform float4 _Direction;
 
		 // Unity defined variables
		 uniform float4 _LightColor0;

         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
         };

         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
			float4 posWorld : TEXCOORD1; 
			float3 normalWorld : TEXCOORD2;
			float3 tangentWorld : TEXCOORD3;
			float3 binormalWorld : TEXCOORD4;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;

			float4 worldPosition = mul(unity_ObjectToWorld, input.vertex);

			float displacement = cos(worldPosition.z +(_Speed * _Time * _Direction.z)) + cos(worldPosition.y+(_Speed * _Time * _Direction.y)) + cos(worldPosition.x + (_Speed * _Time * _Direction.x));
			worldPosition.y = worldPosition.y + displacement * _Strength;
			output.posWorld = worldPosition;

			output.normalWorld = normalize(mul(float4(input.normal, 0.0), unity_WorldToObject));
			output.tangentWorld = normalize(mul(input.normal, unity_ObjectToWorld).xyz );
			output.binormalWorld = normalize(cross(output.normalWorld, output.tangentWorld) * input.tangent.w);
            
			output.tex = input.texcoord;
			output.pos = mul(UNITY_MATRIX_VP, worldPosition);

            return output;
         }

         float4 frag(vertexOutput input) : COLOR
         {

			float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.posWorld.xyz);
			float3 lightDirection;
			float atten;

			if(_WorldSpaceLightPos0.w == 0.0){//directional light
				atten = 1.0;
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			} else {
				float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - float3(input.posWorld.xyz);
				float distance = length(fragmentToLightSource);
				atten = 1.0 / distance;
				lightDirection = normalize(fragmentToLightSource);
			}

			//Texture Maps
			float4 tex = tex2D(_MainTex, _MainTex_ST.xy * input.tex.xy + _MainTex_ST.zw);	
			float4 texN = tex2D(_BumpMap, _BumpMap_ST.xy * input.tex.xy + _BumpMap_ST.zw);	
			float4 texSpec = tex2D(_SpecMap, _SpecMap_ST.xy * input.tex.xy + _SpecMap_ST.zw);	

			//unpack Normal
			float3 loocalCoords = float3(2.0 * texN.ag - float2(1.0, 1.0),0.0);
			loocalCoords.z = _BumpDepth;

			//normal transpose matric
			float3x3 local2WorldTranspose = float3x3(
				input.tangentWorld,
				input.binormalWorld,
				input.normalWorld
			);

			float3 normalDirection = normalize(mul(loocalCoords, local2WorldTranspose));


			//Lighting
			float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
			float3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
			
			//Rim Lighting
			float rim = 1 - saturate(dot(viewDirection, normalDirection)); 
			float3 rimLighting = saturate(dot(normalDirection, lightDirection)* _RimColor.xyz * _LightColor0.xyz * pow(rim, _RimPower));
			float3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection + (specularReflection * texSpec.a) + rimLighting;

			return float4(tex.xyz * lightFinal * _Color.xyz, 1.0);
         }
 
         ENDCG
      }
      Pass {	
	     Tags {"Lightmode" = "ForwardAdd"}
		 Blend One One
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
		 // User defined variables
         uniform sampler2D _MainTex;
         uniform sampler2D _BumpMap;		
         uniform sampler2D _SpecMap;		
         uniform float4 _MainTex_ST; //used for offset
         uniform float4 _BumpMap_ST; //used for offset
         uniform float4 _SpecMap_ST; //used for offset
		 uniform float4 _Color;
		 uniform float4 _SpecColor;
		 uniform float4 _RimColor;
		 uniform float _Shininess;
		 uniform float _RimPower;
		 uniform float _BumpDepth;
		 uniform float _Strength;
		 uniform float _Speed;
		 uniform float4 _Direction;
 
		 // Unity defined variables
		 uniform float4 _LightColor0;

         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
         };

         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
			float4 posWorld : TEXCOORD1; 
			float3 normalWorld : TEXCOORD2;
			float3 tangentWorld : TEXCOORD3;
			float3 binormalWorld : TEXCOORD4;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;

			float4 worldPosition = mul(unity_ObjectToWorld, input.vertex);

			float displacement = cos(worldPosition.z +(_Speed * _Time * _Direction.z)) + cos(worldPosition.y+(_Speed * _Time * _Direction.y)) + cos(worldPosition.x + (_Speed * _Time * _Direction.x));
			worldPosition.y = worldPosition.y + displacement * _Strength;
			output.posWorld = worldPosition;

			output.normalWorld = normalize(mul(float4(input.normal, 0.0), unity_WorldToObject));
			output.tangentWorld = normalize(mul(input.normal, unity_ObjectToWorld).xyz );
			output.binormalWorld = normalize(cross(output.normalWorld, output.tangentWorld) * input.tangent.w);
            
			output.tex = input.texcoord;
			output.pos = mul(UNITY_MATRIX_VP, worldPosition);

            return output;
         }

         float4 frag(vertexOutput input) : COLOR
         {

			float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - input.posWorld.xyz);
			float3 lightDirection;
			float atten;

			if(_WorldSpaceLightPos0.w == 0.0){//directional light
				atten = 1.0;
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			} else {
				float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - float3(input.posWorld.xyz);
				float distance = length(fragmentToLightSource);
				atten = 1.0 / distance;
				lightDirection = normalize(fragmentToLightSource);
			}

			//Texture Maps
			float4 tex = tex2D(_MainTex, _MainTex_ST.xy * input.tex.xy + _MainTex_ST.zw);	
			float4 texN = tex2D(_BumpMap, _BumpMap_ST.xy * input.tex.xy + _BumpMap_ST.zw);	
			float4 texSpec = tex2D(_SpecMap, _SpecMap_ST.xy * input.tex.xy + _SpecMap_ST.zw);	

			//unpack Normal
			float3 loocalCoords = float3(2.0 * texN.ag - float2(1.0, 1.0),0.0);
			loocalCoords.z = _BumpDepth;

			//normal transpose matric
			float3x3 local2WorldTranspose = float3x3(
				input.tangentWorld,
				input.binormalWorld,
				input.normalWorld
			);

			float3 normalDirection = normalize(mul(loocalCoords, local2WorldTranspose));


			//Lighting
			float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
			float3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
			
			//Rim Lighting
			float rim = 1 - saturate(dot(viewDirection, normalDirection)); 
			float3 rimLighting = saturate(dot(normalDirection, lightDirection)* _RimColor.xyz * _LightColor0.xyz * pow(rim, _RimPower));
			float3 lightFinal = atten * diffuseReflection + (specularReflection * texSpec.a) + rimLighting;

			return float4(lightFinal * _Color.xyz, 1.0);
         }
 
         ENDCG
      }
   }
   Fallback "Unlit/Texture"
}