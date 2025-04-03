// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32716,y:32678,varname:node_4795,prsc:2|emission-6074-RGB,alpha-7489-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32259,y:32801,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4639-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7611,x:31728,y:32798,varname:node_7611,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:4639,x:32046,y:32862,varname:node_4639,prsc:2,spu:-0.1,spv:0|UVIN-7611-UVOUT,DIST-7453-OUT;n:type:ShaderForge.SFN_Time,id:3859,x:31548,y:32967,varname:node_3859,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7453,x:31821,y:33128,varname:node_7453,prsc:2|A-3859-T,B-8996-OUT;n:type:ShaderForge.SFN_Slider,id:8996,x:31366,y:33343,ptovrint:False,ptlb:node_8996,ptin:_node_8996,varname:_node_8996,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:7489,x:32474,y:32976,varname:node_7489,prsc:2|A-6074-A,B-8198-A,C-2390-OUT,D-8249-A;n:type:ShaderForge.SFN_Color,id:8198,x:32017,y:33082,ptovrint:False,ptlb:node_8198,ptin:_node_8198,varname:_node_8198,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:2390,x:32086,y:33401,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_VertexColor,id:8249,x:32063,y:33271,varname:node_8249,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7903,x:32582,y:32764,varname:node_7903,prsc:2|A-6074-RGB,B-8249-RGB;proporder:6074-8996-8198-2390;pass:END;sub:END;*/

Shader "Shader Forge/Water_flow_Sea wave" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _node_8996 ("node_8996", Range(-1, 1)) = 1
        _node_8198 ("node_8198", Color) = (0.5,0.5,0.5,1)
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _node_8996;
            uniform float4 _node_8198;
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
////// Emissive:
                float4 node_3859 = _Time;
                float2 node_4639 = (i.uv0+(node_3859.g*_node_8996)*float2(-0.1,0));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4639, _MainTex));
                float3 emissive = _MainTex_var.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*_node_8198.a*_Alpha*i.vertexColor.a));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
