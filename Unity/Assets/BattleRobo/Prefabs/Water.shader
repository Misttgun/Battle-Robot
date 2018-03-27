// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Water"
{
    
    Properties
    {
        _Twat ("Twat", Color)= (1,1,1,1)
        _Tex ("Texture", 2D) = "white" {}
    }
	SubShader 
	{
        Tags
             {
                 "RenderType"="Transparent"
             }
	    Pass
	    {
	        CGPROGRAM
	        
            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "UnityCG.cginc"
            
            float4 _Twat;
            sampler2D _Tex;
            float4 _Tex_ST;
             
            struct Iwat //interpolators
            {
                float4 position: SV_POSITION;
                float2 uv : TEXCOORD0;  
            };
            
            struct VData //vertex data
            {
                float4 position: POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Iwat Vertex (VData v)        
            {
                Iwat i;
                i.position= UnityObjectToClipPos(v.position);
                i.uv = TRANSFORM_TEX(v.uv, _Tex);
                //i.uv = v.uv* _Tex_ST.xy+ _Tex_ST.zw;
                return i;
			}

			float4 Fragment (Iwat i): SV_TARGET 
			{
                return tex2D(_Tex,i.uv)*_Twat;
			}
            
            
	        ENDCG
	    }
	}   
}


