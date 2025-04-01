Shader "Custom/SpriteBox" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
        _MaskTex ("Mask", 2D) = "white" { } 
    }
    SubShader {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
        LOD 100
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            fixed4 SampleSpriteTexture(float2 uv) {
                fixed4 color = tex2D(_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                    fixed4 alpha = tex2D(_AlphaTex, uv);
                    color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
                #endif

                return color;
            }

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 texCol = tex2D(_MainTex, i.uv);
                fixed4 maskCol = tex2D(_MaskTex, i.uv);
                fixed4 c1 = texCol * i.color * maskCol.a;
                texCol.rgb *= texCol.a;
                fixed4 c2 = lerp(texCol, c1, c1.a);
                return c2;
            }
            ENDCG
        }
    }
}