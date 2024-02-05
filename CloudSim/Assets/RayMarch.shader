Shader "Unlit/RayMarch"
{
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
#define MAX_DIST 100
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

            float sdCone(float3 p, float2 c, float h)
            {
                float q = length(p.xz);
                return max(dot(c.xy, float2(q, p.y)),-h - p.y);
            } 

            float sdPyramid(float3 p, float h)
            {
                float m2 = h * h + 0.25;

                p.xz = abs(p.xz);
                p.xz = (p.z > p.x) ? p.zx : p.xz;
                p.xz -= 0.5;

                float3 q = float3(p.z, h * p.y - 0.5 * p.x, h * p.x + 0.5 * p.y);

                float s = max(-q.x, 0.0);
                float t = clamp((q.y - 0.5 * p.z) / (m2 + 0.25), 0.0, 1.0);

                float a = m2 * (q.x + s) * (q.x + s) + q.y * q.y;
                float b = m2 * (q.x + 0.5 * t) * (q.x + 0.5 * t) + (q.y - m2 * t) * (q.y - m2 * t);

                float d2 = min(q.y, -q.x * m2 - q.y * 0.5) > 0.0 ? 0.0 : min(a, b);

                return sqrt((d2 + q.z * q.z) / m2) * sign(max(q.z, -p.y));
            }

            float GetDist(float3 p) {
                float d = length(p) - .5;// sphere
              	d = length(float2(length(p.xz) - .5, p.y)) - .1;// torus 
                d = sdPyramid(p, .5);
  
                return d;
            }
            float Raymarch(float3 ro, float3 rd) {
                float dO = 0;
                float dS;
                for (int i = 0; i < MAX_STEPS;i++) {
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
                float3 ro = i.ro;// float3(0, 0, -3);// RayOrigin
                float3 rd = normalize(i.hitPos-ro);// normalize(float3(uv.x, uv.y, 1));// RayDirection
              
                float d = Raymarch(ro, rd);
                fixed4 col=0;

                if (d<MAX_DIST)
                {
                    float3 p = ro + rd * d;
                    float3 n = GetNormal(p);
                    col= tex2D( _MainTex, i.uv);
                }


                return col;
            }
            ENDCG
        }
    }
}
