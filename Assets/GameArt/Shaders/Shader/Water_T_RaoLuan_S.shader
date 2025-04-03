// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:Mobile/Particles/Additive,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33023,y:32663,varname:node_2865,prsc:2|custl-7736-RGB,alpha-4181-OUT;n:type:ShaderForge.SFN_Tex2d,id:7736,x:32309,y:32897,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:512ff8dfe13c05f49b3005c143ded217,ntxv:2,isnm:False|UVIN-4608-OUT;n:type:ShaderForge.SFN_Tex2d,id:2941,x:31090,y:32450,ptovrint:False,ptlb:nosie_U,ptin:_nosie_U,varname:_nosie_U,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:30f64d1c0ceb9364db4d4483d668d0da,ntxv:0,isnm:False|UVIN-7849-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:6149,x:30715,y:32448,varname:node_6149,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:7849,x:30881,y:32448,varname:node_7849,prsc:2,spu:-0.02,spv:0|UVIN-6149-UVOUT;n:type:ShaderForge.SFN_Multiply,id:7888,x:31327,y:32379,varname:node_7888,prsc:2|A-7129-OUT,B-2941-R;n:type:ShaderForge.SFN_Slider,id:7129,x:30999,y:32304,ptovrint:False,ptlb:noize_U_M,ptin:_noize_U_M,varname:_noize_U_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:4608,x:32176,y:32656,varname:node_4608,prsc:2|A-2136-UVOUT,B-4301-OUT;n:type:ShaderForge.SFN_TexCoord,id:2136,x:31841,y:32578,varname:node_2136,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:2958,x:31991,y:33078,varname:node_2958,prsc:2,v1:3;n:type:ShaderForge.SFN_TexCoord,id:3238,x:30769,y:32773,varname:node_3238,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:1051,x:30935,y:32773,varname:node_1051,prsc:2,spu:0,spv:-0.02|UVIN-3238-UVOUT;n:type:ShaderForge.SFN_Slider,id:5402,x:30916,y:33006,ptovrint:False,ptlb:noize_V_M,ptin:_noize_V_M,varname:_noize_V_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:3077,x:31310,y:32846,varname:node_3077,prsc:2|A-7029-R,B-5402-OUT;n:type:ShaderForge.SFN_Add,id:4301,x:32041,y:33023,varname:node_4301,prsc:2|A-7888-OUT,B-3077-OUT,C-8005-OUT,D-7232-OUT;n:type:ShaderForge.SFN_Tex2d,id:7029,x:31117,y:32773,ptovrint:False,ptlb:nosie_V,ptin:_nosie_V,varname:_nosie_V,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:30f64d1c0ceb9364db4d4483d668d0da,ntxv:0,isnm:False|UVIN-1051-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:3742,x:30640,y:33159,varname:node_3742,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:3334,x:30760,y:33452,ptovrint:False,ptlb:nosie_U2_M,ptin:_nosie_U2_M,varname:_nosie_U2_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.07410108,max:1;n:type:ShaderForge.SFN_Multiply,id:8005,x:31317,y:33324,varname:node_8005,prsc:2|A-2802-R,B-3334-OUT;n:type:ShaderForge.SFN_Tex2d,id:2802,x:31038,y:33210,ptovrint:False,ptlb:nosie_U2,ptin:_nosie_U2,varname:_nosie_U2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:30f64d1c0ceb9364db4d4483d668d0da,ntxv:0,isnm:False|UVIN-1541-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:388,x:30601,y:33687,varname:node_388,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:3569,x:30804,y:33993,ptovrint:False,ptlb:nosie_v2_M,ptin:_nosie_v2_M,varname:_nosie_v2_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:7232,x:31360,y:33796,varname:node_7232,prsc:2|A-3524-R,B-3569-OUT;n:type:ShaderForge.SFN_Tex2d,id:3524,x:31081,y:33681,ptovrint:False,ptlb:nosie_V2,ptin:_nosie_V2,varname:_nosie_V2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:30f64d1c0ceb9364db4d4483d668d0da,ntxv:0,isnm:False|UVIN-3870-UVOUT;n:type:ShaderForge.SFN_Panner,id:1541,x:30832,y:33206,varname:node_1541,prsc:2,spu:0.02,spv:0|UVIN-3742-UVOUT;n:type:ShaderForge.SFN_Panner,id:3870,x:30863,y:33708,varname:node_3870,prsc:2,spu:0,spv:0.04|UVIN-388-UVOUT;n:type:ShaderForge.SFN_Multiply,id:8794,x:32163,y:33241,varname:node_8794,prsc:2|A-2958-OUT,B-8333-A;n:type:ShaderForge.SFN_VertexColor,id:8333,x:31975,y:33177,varname:node_8333,prsc:2;n:type:ShaderForge.SFN_Slider,id:3064,x:32401,y:33338,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:4181,x:32732,y:33227,varname:node_4181,prsc:2|A-8794-OUT,B-3064-OUT;proporder:7736-2941-2802-7029-3524-7129-5402-3334-3569-3064;pass:END;sub:END;*/

Shader "Shader Forge/Water_T_RaoLuan_S" {
    Properties {
        _MainTex ("Base Color", 2D) = "black" {}
        _nosie_U ("nosie_U", 2D) = "white" {}
        _nosie_U2 ("nosie_U2", 2D) = "white" {}
        _nosie_V ("nosie_V", 2D) = "white" {}
        _nosie_V2 ("nosie_V2", 2D) = "white" {}
        _noize_U_M ("noize_U_M", Range(0, 1)) = 0
        _noize_V_M ("noize_V_M", Range(0, 1)) = 0
        _nosie_U2_M ("nosie_U2_M", Range(0, 1)) = 0.07410108
        _nosie_v2_M ("nosie_v2_M", Range(0, 1)) = 0
        _Alpha ("Alpha", Range(0, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _nosie_U; uniform float4 _nosie_U_ST;
            uniform float _noize_U_M;
            uniform float _noize_V_M;
            uniform sampler2D _nosie_V; uniform float4 _nosie_V_ST;
            uniform float _nosie_U2_M;
            uniform sampler2D _nosie_U2; uniform float4 _nosie_U2_ST;
            uniform float _nosie_v2_M;
            uniform sampler2D _nosie_V2; uniform float4 _nosie_V2_ST;
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
                float4 node_123 = _Time;
                float2 node_7849 = (i.uv0+node_123.g*float2(-0.02,0));
                float4 _nosie_U_var = tex2D(_nosie_U,TRANSFORM_TEX(node_7849, _nosie_U));
                float2 node_1051 = (i.uv0+node_123.g*float2(0,-0.02));
                float4 _nosie_V_var = tex2D(_nosie_V,TRANSFORM_TEX(node_1051, _nosie_V));
                float2 node_1541 = (i.uv0+node_123.g*float2(0.02,0));
                float4 _nosie_U2_var = tex2D(_nosie_U2,TRANSFORM_TEX(node_1541, _nosie_U2));
                float2 node_3870 = (i.uv0+node_123.g*float2(0,0.04));
                float4 _nosie_V2_var = tex2D(_nosie_V2,TRANSFORM_TEX(node_3870, _nosie_V2));
                float2 node_4608 = (i.uv0+((_noize_U_M*_nosie_U_var.r)+(_nosie_V_var.r*_noize_V_M)+(_nosie_U2_var.r*_nosie_U2_M)+(_nosie_V2_var.r*_nosie_v2_M)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4608, _MainTex));
                float3 finalColor = _MainTex_var.rgb;
                return fixed4(finalColor,((3.0*i.vertexColor.a)*_Alpha));
            }
            ENDCG
        }
    }
    FallBack "Mobile/Particles/Additive"
    CustomEditor "ShaderForgeMaterialInspector"
}
