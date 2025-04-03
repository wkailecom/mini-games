// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader"Custom/Tile3dPlane"
{
    Properties{
        _Color ("Start Color Tint", Color) = (1.0,1.0,1.0,1.0)
        _ColorEnd ("End Color Tint", Color) = (1.0,1.0,1.0,1.0)
    }

    SubShader
    {
        Pass
        {
            Name "GradientWithRecieveShadow"
            Tags {"LightMode" = "ForwardBase"}

            CGPROGRAM        
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fwdbase
            #pragma multi_compile SHADOWS_SCREEN
            
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            float4 _Color;
            float4 _ColorEnd;
            struct a2v
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_Position;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                fixed3 color : COLOR0;
                SHADOW_COORDS(2)
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = (1 - o.pos.y) * fixed3(_ColorEnd.r - _Color.r, _ColorEnd.g - _Color.g, _ColorEnd.b - _Color.b) + (_Color);
                //pass shadow coordinate to pixel shader
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 c = i.color;    
                //Use shadow coordinates to sample shadow map
                fixed shadow = SHADOW_ATTENUATION(i);        
                return fixed4(c * shadow, 1.0);
            }        
            ENDCG
        }  
    }
    Fallback"Diffuse"
}
