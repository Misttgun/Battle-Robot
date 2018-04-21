Shader "Custom/Storm"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,0)
    }
 
    SubShader
    {
        Blend One One
        ZWrite Off
        Cull Off
 
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
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 position: POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct IStorm
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //float depth : DEPTH;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
 
            IStorm Vertex (appdata v)
            {
                IStorm o;
                o.vertex = UnityObjectToClipPos(v.position);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.depth = 0;
                
                return o;
            }
           
            fixed4 _Color;
 
 
            fixed4 Fragment (IStorm i) : SV_Target
            {  
                fixed4 h = tex2D(_MainTex,i.uv)*_Color;
 
                fixed4 colStorm = _Color * _Color.a  + h ;
                return colStorm;
            }
            ENDCG
        }
    }
}
 
