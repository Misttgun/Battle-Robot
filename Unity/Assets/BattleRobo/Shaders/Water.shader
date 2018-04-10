// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Water"
{
    
    Properties
    {
        _Twat ("Twat", Color)= (1,1,1,1)
        _Normal ("Normal map",2D) = "Bump Map" {} 
        _Speed ("Speed", float)=1
        _Amount ("Amount" ,float)=1 
        _Amplitude ("Amplitude",float)=1
        _Distance ("Distance", float)=1
    }
	SubShader 
	{
	
	    Blend One One
        ZWrite Off
        Cull Off
        LOD 200
        
        Tags
             {
                 "RenderType"="Transparent"
                 "Queue"="Transparent"
             }
	    Pass
	    {
	        CGPROGRAM
	        
	     
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma shader_feature MIRROR_EDGE
            #include "UnityCG.cginc"
            
            
            float4 _Twat;
            float4 _Normal_ST;
            float _Amount;
            float _Amplitude;
            float _Distance;
            float _Speed;
          
            sampler2D _Normal;
            
             
            struct Owat //interpolators
            {
                float4 position: SV_POSITION;
                float3 worldPos : TEXCOORD0;  
                half3 tspace0 : TEXCOORD1 ;
                half3 tspace1 : TEXCOORD2 ;
                half3 tspace2 : TEXCOORD3 ;
                float2 uv : TEXCOORD4 ;
                
            };
            
            struct VData //vertex data
            {
                float4 position: POSITION; //vertex 
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float tangent : TANGENT;
            };
            
            Owat Vertex (VData v)        
            {
                Owat o;
                v.position.y += sin(_Time.y * _Speed + v.position.z * _Amplitude) *_Amount;  
                v.position.y += sin(_Time.y * _Speed + v.position.x * _Amplitude) *_Amount;
                
                o.position= UnityObjectToClipPos(v.position);
                o.worldPos= mul(unity_ObjectToWorld, v.position).xyz;
                half3 worldNormal =UnityObjectToWorldNormal(v.normal);
                half3 worldTangent =UnityObjectToWorldDir(v.tangent);
                
                half tangentSign = (v.tangent) * unity_WorldTransformParams;
                half3 worldBitangent = cross(worldNormal, worldTangent) * tangentSign;
                // output the tangent space matrix
                float f= sin(_Time.gg);
                o.tspace0 = half3(worldTangent.x*f, worldBitangent.x*f, worldNormal.x*f);
                o.tspace1 = half3(worldTangent.y*f, worldBitangent.y, worldNormal.y);
                o.tspace2 = half3(worldTangent.z, worldBitangent.z*f, worldNormal.z*f);
                o.uv = TRANSFORM_TEX(v.uv, _Normal);
    
				o.uv.x += sin(_Time.x*_Speed*10+ o.uv.y* _Amplitude) * _Amount/5;
                // o.uv.x += _Amplitude * _Amount;
                //o.uv += sin(-_Time.gg  * _Speed + v.position.y * _Amplitude)* _Distance *_Amount;
                return o;
			}

            
            
			fixed4 Fragment (Owat o): SV_TARGET 
			{
               
                half3 tnormal = UnpackNormal(tex2D(_Normal, o.uv));
           
                half3 worldNormal;
                worldNormal.x = dot(o.tspace0, tnormal);
                worldNormal.y = dot(o.tspace1, tnormal);
                worldNormal.z = dot(o.tspace2, tnormal);

         
                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                half3 worldRefl = reflect(-worldViewDir, worldNormal);
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
                fixed4 c = 0;
                c.rgb = skyColor;
                return c*_Twat*_Twat.a;
			}
            
            
	        ENDCG
	    }
	    
	}   
}


