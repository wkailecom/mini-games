// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UISprites/DefaultGray"
{
    Properties
     {
         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
         _Color ("Tint", Color) = (1,1,1,1)
         [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
         //---Add---
         // Change the brightness of the Sprite
         _GrayScale ("GrayScale", Float) = 1
         //---Add---
         //MASK SUPPORT ADD
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        //MASK SUPPORT END
     }
     SubShader
     {
         Tags
         { 
             "Queue"="Transparent" 
             "IgnoreProjector"="True" 
             "RenderType"="Transparent" 
             "PreviewType"="Plane"
             "CanUseSpriteAtlas"="True"
         }
         //MASK SUPPORT ADD
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
        //MASK SUPPORT END
         Cull Off
         Lighting Off
         ZWrite Off
         Blend One OneMinusSrcAlpha
         Pass
         {
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile _ DefGray
             #include "UnityCG.cginc"
             struct appdata_t
             {
                 float4 vertex   : POSITION;
                 float4 color    : COLOR;
                 float2 texcoord : TEXCOORD0;
             };
            struct v2f
             {
                 float4 vertex   : SV_POSITION;
                 fixed4 color    : COLOR;
                 half2 texcoord  : TEXCOORD0;
             };
             fixed4 _Color;
             v2f vert(appdata_t IN)
             {
                 v2f OUT;
                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
                 OUT.texcoord = IN.texcoord;
                 OUT.color = IN.color * _Color;
                 return OUT;
             }
             sampler2D _MainTex;
             float _GrayScale;
             fixed4 frag(v2f IN) : SV_Target
             {
                 fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                 #ifdef DefGray
                 //---Add--
                 float cc = (c.r * 0.299 + c.g * 0.518 + c.b * 0.184);
                 cc *= _GrayScale;
                 c.r = c.g = c.b = cc;
                 //---Add--
                 #endif
                 c.rgb *= c.a;
                 return c;
             }
         ENDCG
         }
     }
}