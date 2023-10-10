Shader "Unlit/ReversiDisc"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (0, 0, 0, 1)
        _DiffuseShade("Diffuse Shade",Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "LightMode"="ForwardBase"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _MainColor;
            float _DiffuseShade;

            struct appdata
            {
                float4 vertex:POSITION;
                float3 normal:NORMAL;
            };

            struct v2f
            {
                float4 pos:SV_POSITION;
                float4 localPos:POSITION1;
                float3 worldNormal:TEXCOORD0;
                float3 ambient : COLOR0; //環境光
            };

            //頂点シェーダー
            v2f vert(appdata v)
            {
                v2f o;
                o.localPos = v.vertex;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.ambient = ShadeSH9(half4(o.worldNormal,1));
                return o;
            }

            //フラグメントシェーダー
            fixed4 frag(v2f i) : SV_Target
            {
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 N = normalize(i.worldNormal);
                fixed4 diffuseColor = max(0, dot(N, L) * _DiffuseShade + (1 - _DiffuseShade));
                // diffuseColor = step(0.5,diffuseColor);
                fixed heightColor = step(0.0,i.localPos.y) + 0.01;
                fixed4 finalColor = heightColor * _MainColor * diffuseColor * _LightColor0 * float4(i.ambient,0);
                return finalColor;
            }
            ENDCG
        }

        // 影を落とすパス
		Pass 
		{
			Name "CastShadow"
			Tags { "LightMode" = "ShadowCaster" }
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
	
			struct v2f 
			{ 
				V2F_SHADOW_CASTER;
			};
	
			v2f vert( appdata_base v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}
	
			float4 frag( v2f i ) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
    }
}
