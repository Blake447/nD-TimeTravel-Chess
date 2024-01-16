Shader "Unlit/TopologyMorph"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scroll("Scroll", Vector) = (0,0,0,0)
        _Color("Color", Color) = (1,1,1,1)
        _Mode0("Mode 0", int) = 0
        _Mode1("Mode 1", int) = 0
        _Factor("Factor", Range(0,1)) = 0
        _Radius("Radius", float) = 0.1
        _Offset("Offset", float) = 0.0
        _Phase("Phase", float) = 0.0
        _Min("Min", float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float2 coord : TEXCOORD1;
                float clipEdge : TEXCOORD2;
            };

            const float PI = 3.14159;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            int _Mode0;
            int _Mode1;
            float _Factor;
            float _Radius;
            float _Offset;
            float _Phase;
            float _Min;
            float2 _Scroll;

            float3 board(float3 p)
            {
                return p;
            }
            float3 cylinder_x(float3 p, float r)
            {
                float3 v = p * 2 * 3.14159;
                return float3(r*(cos(v.x) - 1.0f), p.y, -r*sin(v.x));
            }
            float3 wrapping(float3 uv)
            {
                float3 v = uv;
                if (frac(uv.y / 2) >= 0.5)
                {
                    uv.y = frac(uv.y);
                    uv.x = 1.0 - frac(uv.x);
                }
                else
                {
                    uv.x = frac(uv.x);
                    uv.y = frac(uv.y);
                }
                return uv;
            }
            //float3 klein(float3 p)
            //{
            //    float depth = p.z;

            //    float3 v = frac(p+float3(_Scroll, 0.0)) * (2 * 3.14159);
            //    float3 phi = v.y + _Phase;
            //    float2 S = float2(2*((1.0 / 16.0) * sin(v.y) + (1.0 / 32.0) * sin(2*v.y)), 0.5 + 0.5 * cos(v.y));
            //    float2 T = float2(2*((1.0 / 16.0) * cos(v.y) + (1.0 / 16.0) * cos(2*v.y)), 0.0 +-0.5 * sin(v.y));
            //    float f = (sin(phi)*_Radius*0.01 + _Offset);
            //    float r = 6 * f * f * f + _Min;
            //    float3 start = float3(0.0, S);
            //    float3 tangent = float3(0.0, T);
            //    if (length(tangent) < 0.01 && p.y > 0.5)
            //        tangent = float3(0.0, 0.0, -1.0);
            //    float3 side = float3(1.0, 0.0, 0.0);
            //    float theta = -v.x;
            //    float3 offset = r *cross(normalize(tangent), side) * sin(theta) + r * side * cos(theta);
            //    float3 vertex = start + offset;
            //    if (length(offset) > 0.001)
            //        vertex = start + offset + normalize(offset) * depth;
            //    return vertex;
            //}
            float3 klein(float3 p)
            {
                float depth = p.z;

                float3 v = (wrapping(p+float3(_Scroll, 0.0)) * (2 * 3.14159));
                float3 phi = v.y + _Phase;
                float t = v.y;
                
                float t2 = 2 * 3.14159 - t;
                float s = v.x;
                float2 scale = float2(0.5, 1.0);
                float2 S1 = float2(0.25 * t * t * sin(t), 2 * sin(0.5 * t)) * scale;
                float2 S2 = float2(0.25 * t2 * t2 * sin(t), 2 * sin(0.5 * t)) * scale;
                float2 T1 = float2( 0.5 * t * sin(t) + 0.25 * t * t * cos(t), cos(0.5 * t)) * scale;
                float2 T2 = float2(-0.5 * t2 * sin(t) + 0.25 * t2 * t2 * cos(t), cos(0.5 * t)) * scale;
                float swit = step(3.14159, t);
                float2 S = lerp(S1, S2, swit);
                float2 T = lerp(T1, T2, swit);
                float f = (sin(phi)*_Radius*0.01 + _Offset);
                float r = 4 * f * f * f*f*f + _Min;
                float3 start = float3(0.0, S);
                float3 tangent = float3(0.0, T);
                if (length(tangent) < 0.01 && p.y > 0.5)
                    tangent = float3(0.0, 0.0, -1.0);
                float3 side = float3(1.0, 0.0, 0.0);
                float theta = -v.x;
                float3 offset = r *cross(normalize(tangent), side) * sin(theta) + r * side * cos(theta);
                float3 vertex = start + offset;
                if (length(offset) > 0.001)
                    vertex = start + offset + normalize(offset) * depth;
                return vertex;
            }


            v2f vert (appdata v)
            {
                v2f o;

                float4 vertex = float4(lerp( board(v.vertex.xyz), klein(v.vertex.xyz), _Factor), v.vertex.w);
                o.vertex = UnityObjectToClipPos(vertex);
                o.coord = v.vertex.xy;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.clipEdge = 1 - 2* (frac( (v.vertex.y + _Scroll.y) / 2) > 0.5);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //clip(abs(i.coord.x - .01) - 0.025);

                //clip(i.clipEdge);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
