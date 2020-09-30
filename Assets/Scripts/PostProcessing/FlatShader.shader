Shader "Custom/FlatShader"
{
    Properties
    {

    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            // vertex shader
            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            // pixel shader
            fixed3 frag(v2f i) : SV_Target
            {
                float3 dpdx = ddx(i.worldPos);
                float3 dpdy = ddy(i.worldPos);
                float3 normal = normalize(cross(dpdy, dpdx));

                return abs(normal);
            }
            ENDCG
        }
    }
}
