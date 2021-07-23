Shader "SpaceTarget/VertexColor"
{
	Properties {
		_AlphaScale ("Alpha Scale", Range(0, 1)) = 1
	}
	SubShader {
        
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		// Extra pass that renders to depth buffer only
		Pass {
			ZWrite On
			ColorMask 0
		}
		
		Pass {
			Tags { "LightMode"="ForwardBase" }
			
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"
			

			fixed _AlphaScale;
            
			
			struct a2v {
				float4 vertex : POSITION;
                float4 vertexColor : COLOR;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
                float4 vertexColor : COLOR;
			};
			
			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

                o.vertexColor = v.vertexColor;
				
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {

                float3 emissive = i.vertexColor.rgb;

                fixed4 finalRGBA = fixed4(emissive,_AlphaScale);
				
				return finalRGBA;
			}
			
			ENDCG
		}
	} 
	FallBack "Transparent/VertexLit"
}
