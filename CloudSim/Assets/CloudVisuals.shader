/*
    In this shader the 
*/

Shader "Unlit/CloudVisuals"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vertex_shader
            #pragma fragment fragment_shader
            

            #include "UnityCG.cginc"
            
            // Structs. All but Cell come with the standard shader
            struct Cell
            {
                float3 pos;
                int alive;
                int aliveFor;
                float4 col;
            };

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint id: SV_VertexID;
            };

            struct SHADERDATA
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            shared RWStructuredBuffer<Cell> cells; 

            SHADERDATA vertex_shader (appdata IN)
            {
                SHADERDATA vs;
                vs.vertex = UnityObjectToClipPos(IN.vertex);
                vs.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                
                return vs;
            }

            fixed4 fragment_shader (SHADERDATA i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
