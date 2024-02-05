Shader "Unlit/RayMarch"
{
    // this shader was done by closely following this video: https://www.youtube.com/watch?v=S8AWd66hoCo
    // sdf func was taken from: https://iquilezles.org/articles/distfunctions/
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
                #define MAX_DIST 100000
                #define MAX_STEPS 50
                #define SURF_DIST 1e-4

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos: TEXCOORD2;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.hitPos = v.vertex;
                return o;
            }

            
            float GetDist(float3 p) {
                float3 b = float3(0,0,0); //Origin of the box?? should change dynamically with placement of the cloud sim box??
                float3 q = abs(p) - b;
                float d = length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0); //box
                d =length(p)-.5;//sphere...just for testing 
                return d;
            }

            
            float Raymarch(float3 ro, float3 rd) {
                float dO = 0;
                float dS;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = ro + dO * rd;
                    dS = GetDist(p);
                    dO += dS;
                    if (dS<SURF_DIST || dO>MAX_DIST) break;
                }
                return dO;
            }

            
            float3 GetNormal(float3 p) {
                float2 e = float2 (1e-2, 0);
                float3 n = GetDist(p) - float3(
                    GetDist(p-e.xyy),
                    GetDist(p-e.yxy),
                    GetDist(p-e.yyx)
                    );
                return normalize(n);
            }

            
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv-.5;
                float3 rayOrigin = i.ro;// float3(0, 0, -3);// RayOrigin
                float3 rayDirection = normalize(i.hitPos-rayOrigin);// normalize(float3(uv.x, uv.y, 1));// RayDirection
              
                float d = Raymarch(rayOrigin, rayDirection);
                fixed4 col=0;

                if (d<MAX_DIST)
                {
                    float3 p = rayOrigin + rayDirection * d;
                    float3 n = GetNormal(p);
                    col= tex2D( _MainTex, i.uv);
                }


                return col;
            }
            ENDCG
        }
    }
}
