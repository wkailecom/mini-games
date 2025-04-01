Shader "Custom/SpriteBoard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _ShadowX("Shadow X",Range(-0.5,0.5)) = 0.1
        _ShadowY("Shadow Y",Range(-0.5,0.5)) = -0.05
        _ShadowAlpha("Shadow Alpha",Range(0,1))=0.5
        _PixelOffset("PixelOffset",Range(0,2))=1
        _StencilRef ("Stencil Ref", Range(0,255)) = 1
        _AtlasRect("AtlasRect", Vector) = (0,0,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True"
        }
        LOD 100
        ZWrite Off
        Blend One OneMinusSrcAlpha
        Stencil
        {
            Ref [_StencilRef]
            Comp NotEqual
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv2 : TEXCOORD1;
                float4 modelPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ShadowX;
            float _ShadowY;
            float _ShadowAlpha;
            float _PixelOffset;
            float4 _UVScale;
            float4 _AtlasRect;

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                    fixed4 alpha = tex2D(_AlphaTex, uv);
                    color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
                #endif

                return color;
            }

            // 实现柔光效果的函数
            float4 SoftLight(float4 base, float4 blend)
            {
                float4 result;

                for (int i = 0; i < 3; i++)
                {
                    if (base[i] < 0.5)
                        result[i] = 2.0 * base[i] * blend[i] + base[i] * base[i] * (1.0 - 2.0 * blend[i]);
                    else
                        result[i] = 2.0 * base[i] * (1.0 - blend[i]) + sqrt(base[i]) * (2.0 * blend[i] - 1.0);
                }

                result.a = base.a; // 保持透明度不变
                return result;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.uv2 = o.vertex.xy / o.vertex.w;
                float4 modelPos = mul(unity_WorldToObject, float3(_ShadowX, _ShadowY, 0));
                o.modelPos = modelPos;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texCol = SampleSpriteTexture(i.uv);
                fixed4 c1 = texCol;
                c1 = 1 - c1;
                c1 *= texCol.a;

                fixed4 center = texCol;
                center *= i.color;
                center.rgb *= center.a;
                c1 = c1 + center;

                //阴影
                fixed2 targetDir = normalize(i.modelPos.xy) * (_PixelOffset / _UVScale.x);
                half shadowA = tex2D(_MainTex, i.uv + targetDir.xy).a;

                //裁剪阴影,待优化
                fixed2 checkUV = i.uv + targetDir.xy;
                float uMin = _AtlasRect.x;
                float vMin = _AtlasRect.y;
                float uMax = _AtlasRect.z;
                float vMax = _AtlasRect.w;
                if (checkUV.x < uMin || checkUV.x > uMax || checkUV.y < vMin || checkUV.y > vMax)
                {
                    shadowA = 0;
                }

                //half shadowA = tex2D(_MainTex,i.uv + targetDir.xy).a;
                center.a = max(shadowA * _ShadowAlpha * i.color.a, center.a);

                return fixed4(c1.rgb, center.a);
            }
            ENDCG
        }
    }
}