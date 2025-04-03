// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32375,y:32582,varname:node_3138,prsc:2|emission-8674-OUT,alpha-2355-A;n:type:ShaderForge.SFN_Tex2d,id:2355,x:31816,y:32573,varname:__MainTex,prsc:2,ntxv:0,isnm:False|TEX-2480-TEX;n:type:ShaderForge.SFN_Tex2d,id:1870,x:31726,y:32892,ptovrint:False,ptlb:Glow_Tex,ptin:_Glow_Tex,varname:_Glow_Tex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2eeb93995cfa7a4459dd8257df9df7ea,ntxv:0,isnm:False|UVIN-2430-UVOUT;n:type:ShaderForge.SFN_Add,id:8674,x:32126,y:32622,varname:node_8674,prsc:2|A-2355-RGB,B-8030-OUT;n:type:ShaderForge.SFN_Color,id:4226,x:31726,y:33120,ptovrint:False,ptlb:Glow_Color,ptin:_Glow_Color,varname:_Glow_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:8030,x:31937,y:32892,varname:node_8030,prsc:2|A-1870-RGB,B-4226-RGB;n:type:ShaderForge.SFN_TexCoord,id:9242,x:30929,y:32878,varname:node_9242,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:6207,x:31171,y:32876,varname:node_6207,prsc:2,spu:0,spv:1|UVIN-9242-UVOUT,DIST-9637-OUT;n:type:ShaderForge.SFN_Time,id:479,x:30523,y:32983,varname:node_479,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9637,x:30826,y:33081,varname:node_9637,prsc:2|A-479-TSL,B-3088-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3088,x:30538,y:33193,ptovrint:False,ptlb:Y_offset,ptin:_Y_offset,varname:_Y_offset,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Panner,id:2430,x:31416,y:32876,varname:node_2430,prsc:2,spu:1,spv:0|UVIN-6207-UVOUT,DIST-6149-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3948,x:30974,y:33431,ptovrint:False,ptlb:X_offset,ptin:_X_offset,varname:_X_offset,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:6149,x:31244,y:33251,varname:node_6149,prsc:2|A-8412-TSL,B-3948-OUT;n:type:ShaderForge.SFN_Time,id:8412,x:30974,y:33217,varname:node_8412,prsc:2;n:type:ShaderForge.SFN_Tex2dAsset,id:2480,x:31553,y:32548,ptovrint:True,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:2480-1870-4226-3088-3948;pass:END;sub:END;*/

Shader "Effect/MaskGlow" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Glow_Tex ("Glow_Tex", 2D) = "white" {}
        _Glow_Color ("Glow_Color", Color) = (0.5,0.5,0.5,1)
        _Y_offset ("Y_offset", Float ) = 1
        _X_offset ("X_offset", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        //MASK SUPPORT ADD
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        //END
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        
    Stencil
    {
        Ref [_Stencil]
        Comp [_StencilComp]
        Pass [_StencilOp] 
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
    }
    
    ColorMask [_ColorMask]
    //END
        Pass {
            Name "FORWARD"
            Tags {
                // "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            // #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _Glow_Tex; uniform float4 _Glow_Tex_ST;
            uniform float4 _Glow_Color;
            uniform float _Y_offset;
            uniform float _X_offset;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                fixed4 __MainTex = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float2 node_2430 = ((i.uv0+(_Time.r*_Y_offset)*float2(0,1))+(_Time.r*_X_offset)*float2(1,0));
                fixed4 _Glow_Tex_var = tex2D(_Glow_Tex,TRANSFORM_TEX(node_2430, _Glow_Tex));
                fixed3 emissive = (__MainTex.rgb+(_Glow_Tex_var.rgb*_Glow_Color.rgb));
                return fixed4(emissive,__MainTex.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
