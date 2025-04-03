Shader "UI/Alpha Blended Mask"
{
  Properties
  {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _MainTex ("Particle Texture", 2D) = "white" {}
    _InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
    _StencilComp ("Stencil Comparison", float) = 8
    _Stencil ("Stencil ID", float) = 0
    _StencilOp ("Stencil Operation", float) = 0
    _StencilWriteMask ("Stencil Write Mask", float) = 255
    _StencilReadMask ("Stencil Read Mask", float) = 255
    _ColorMask ("Color Mask", float) = 15
    _ClipRect ("Clip Rect", Vector) = (-32767,-32767,32767,32767)
  }
  SubShader
  {
    Tags
    { 
      "CanUseSpriteAtlas" = "true"
      "IGNOREPROJECTOR" = "true"
      "PreviewType" = "Plane"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "CanUseSpriteAtlas" = "true"
        "IGNOREPROJECTOR" = "true"
        "PreviewType" = "Plane"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZWrite Off
      Cull Off
      Stencil
      { 
		  Ref[_Stencil]
	      Comp[_StencilComp]
	      Pass[_StencilOp]
	      ReadMask[_StencilReadMask]
	      WriteMask[_StencilWriteMask]
        //Ref 0
        //ReadMask 0
        //WriteMask 0
        //Pass Keep
        //Fail Keep
        //ZFail Keep
        //PassFront Keep
        //FailFront Keep
        //ZFailFront Keep
        //PassBack Keep
        //FailBack Keep
        //ZFailBack Keep
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _TintColor;
      uniform float4 _MainTex_ST;
      uniform float4 _ClipRect;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 color :COLOR0;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 color :COLOR0;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 color :COLOR0;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_1(unity_ObjectToWorld));
          u_xlat0 = ((conv_mxt4x4_0(unity_ObjectToWorld) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_2(unity_ObjectToWorld) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_3(unity_ObjectToWorld));
          out_v.texcoord.zw = ((conv_mxt4x4_3(unity_ObjectToWorld).xy * in_v.vertex.ww) + u_xlat0.xy).xy;
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.color = (in_v.color * _TintColor);
          out_v.texcoord.xy = float2(TRANSFORM_TEX(in_v.texcoord.xy, _MainTex));
          out_v.texcoord1 = in_v.vertex;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlatb0;
      float4 u_xlat1_d;
      float4 u_xlat16_1;
      float4 u_xlat10_2;
      float u_xlat16_3;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlatb0.xy = float2(bool4(in_f.texcoord.zwzz >= _ClipRect.xyxx).xy);
          u_xlatb0.zw = bool4(_ClipRect.zzzw >= in_f.texcoord.zzzw).zw;
          u_xlat0_d = lerp(float4(0, 0, 0, 0), float4(1, 1, 1, 1), float4(u_xlatb0));
          u_xlat0_d.xy = float2((u_xlat0_d.zw * u_xlat0_d.xy));
          u_xlat0_d.x = (u_xlat0_d.y * u_xlat0_d.x);
          u_xlat16_1 = (in_f.color + in_f.color);
          u_xlat10_2 = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat1_d = (u_xlat16_1 * u_xlat10_2);
          u_xlat16_3 = u_xlat1_d.w;
          u_xlat16_3 = clamp(u_xlat16_3, 0, 1);
          u_xlat1_d.w = (u_xlat0_d.x * u_xlat16_3);
          out_f.color = u_xlat1_d;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
