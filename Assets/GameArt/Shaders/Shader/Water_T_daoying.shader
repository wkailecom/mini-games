// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33023,y:32663,varname:node_2865,prsc:2|normal-370-OUT,emission-8989-OUT;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31935,y:32368,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f7fa4e99227bd3240ba44b4796db0ccf,ntxv:2,isnm:False|UVIN-4608-OUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:31864,y:32965,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:930538e31f5dd454e8482952cc861585,ntxv:3,isnm:True|UVIN-4608-OUT;n:type:ShaderForge.SFN_Tex2d,id:2941,x:30508,y:32523,ptovrint:False,ptlb:nosie_U,ptin:_nosie_U,varname:_nosie_U,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:30f64d1c0ceb9364db4d4483d668d0da,ntxv:0,isnm:False|UVIN-7849-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:6149,x:30133,y:32521,varname:node_6149,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:7849,x:30299,y:32521,varname:node_7849,prsc:2,spu:-0.015,spv:0|UVIN-6149-UVOUT;n:type:ShaderForge.SFN_Multiply,id:7888,x:30745,y:32452,varname:node_7888,prsc:2|A-7129-OUT,B-2941-R;n:type:ShaderForge.SFN_Slider,id:7129,x:30417,y:32377,ptovrint:False,ptlb:noize_U_M,ptin:_noize_U_M,varname:_noize_U_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.09504008,max:1;n:type:ShaderForge.SFN_Add,id:4608,x:31594,y:32729,varname:node_4608,prsc:2|A-2136-UVOUT,B-4301-OUT;n:type:ShaderForge.SFN_TexCoord,id:2136,x:31259,y:32651,varname:node_2136,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:370,x:32191,y:32855,varname:node_370,prsc:2|A-5888-OUT,B-5964-RGB,T-9382-OUT;n:type:ShaderForge.SFN_Vector3,id:5888,x:31893,y:32803,varname:node_5888,prsc:2,v1:0,v2:0,v3:0.5;n:type:ShaderForge.SFN_Color,id:2109,x:31953,y:32618,ptovrint:False,ptlb:color,ptin:_color,varname:_color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:3825,x:32304,y:32529,varname:node_3825,prsc:2|A-7736-RGB,B-2109-RGB;n:type:ShaderForge.SFN_TexCoord,id:3238,x:30187,y:32846,varname:node_3238,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:1051,x:30353,y:32846,varname:node_1051,prsc:2,spu:0,spv:-0.015|UVIN-3238-UVOUT;n:type:ShaderForge.SFN_Slider,id:5402,x:30334,y:33079,ptovrint:False,ptlb:noize_V_M,ptin:_noize_V_M,varname:_noize_V_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3595321,max:1;n:type:ShaderForge.SFN_Multiply,id:3077,x:30728,y:32919,varname:node_3077,prsc:2|A-7029-R,B-5402-OUT;n:type:ShaderForge.SFN_Add,id:4301,x:31459,y:33096,varname:node_4301,prsc:2|A-7888-OUT,B-3077-OUT,C-8005-OUT,D-7232-OUT;n:type:ShaderForge.SFN_Tex2d,id:7029,x:30535,y:32846,ptovrint:False,ptlb:nosie_V,ptin:_nosie_V,varname:_nosie_V,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:30f64d1c0ceb9364db4d4483d668d0da,ntxv:0,isnm:False|UVIN-1051-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:3742,x:30058,y:33232,varname:node_3742,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:3334,x:30178,y:33525,ptovrint:False,ptlb:nosie_U2_M,ptin:_nosie_U2_M,varname:_nosie_U2_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3595321,max:1;n:type:ShaderForge.SFN_Multiply,id:8005,x:30735,y:33397,varname:node_8005,prsc:2|A-2802-R,B-3334-OUT;n:type:ShaderForge.SFN_Tex2d,id:2802,x:30456,y:33283,ptovrint:False,ptlb:nosie_U2,ptin:_nosie_U2,varname:_nosie_U2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:30f64d1c0ceb9364db4d4483d668d0da,ntxv:0,isnm:False|UVIN-1541-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:388,x:30019,y:33760,varname:node_388,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:3569,x:30222,y:34066,ptovrint:False,ptlb:nosie_v2_M,ptin:_nosie_v2_M,varname:_nosie_v2_M,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3595321,max:1;n:type:ShaderForge.SFN_Multiply,id:7232,x:30778,y:33869,varname:node_7232,prsc:2|A-3524-R,B-3569-OUT;n:type:ShaderForge.SFN_Tex2d,id:3524,x:30499,y:33754,ptovrint:False,ptlb:nosie_V2,ptin:_nosie_V2,varname:_nosie_V2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:30f64d1c0ceb9364db4d4483d668d0da,ntxv:0,isnm:False|UVIN-3870-UVOUT;n:type:ShaderForge.SFN_Panner,id:1541,x:30250,y:33279,varname:node_1541,prsc:2,spu:0.01,spv:0|UVIN-3742-UVOUT;n:type:ShaderForge.SFN_Panner,id:3870,x:30281,y:33781,varname:node_3870,prsc:2,spu:0,spv:0.01|UVIN-388-UVOUT;n:type:ShaderForge.SFN_Vector1,id:9382,x:31996,y:33211,varname:node_9382,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:2823,x:32418,y:33006,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:8989,x:32749,y:32895,varname:node_8989,prsc:2|A-3825-OUT,B-2823-OUT;proporder:5964-2109-7736-2941-2802-7029-3524-7129-5402-3334-3569-2823;pass:END;sub:END;*/

Shader "Shader Forge/Water_T_daoying" {
    Properties {
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _color ("color", Color) = (1,1,1,1)
        _MainTex ("Base Color", 2D) = "black" {}
        _nosie_U ("nosie_U", 2D) = "white" {}
        _nosie_U2 ("nosie_U2", 2D) = "white" {}
        _nosie_V ("nosie_V", 2D) = "white" {}
        _nosie_V2 ("nosie_V2", 2D) = "white" {}
        _noize_U_M ("noize_U_M", Range(0, 1)) = 0.09504008
        _noize_V_M ("noize_V_M", Range(0, 1)) = 0.3595321
        _nosie_U2_M ("nosie_U2_M", Range(0, 1)) = 0.3595321
        _nosie_v2_M ("nosie_v2_M", Range(0, 1)) = 0.3595321
        _Alpha ("Alpha", Range(0, 1)) = 1
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
            Blend One One
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
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _nosie_U; uniform float4 _nosie_U_ST;
            uniform float _noize_U_M;
            uniform float4 _color;
            uniform float _noize_V_M;
            uniform sampler2D _nosie_V; uniform float4 _nosie_V_ST;
            uniform float _nosie_U2_M;
            uniform sampler2D _nosie_U2; uniform float4 _nosie_U2_ST;
            uniform float _nosie_v2_M;
            uniform sampler2D _nosie_V2; uniform float4 _nosie_V2_ST;
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float4 node_3559 = _Time;
                float2 node_7849 = (i.uv0+node_3559.g*float2(-0.015,0));
                float4 _nosie_U_var = tex2D(_nosie_U,TRANSFORM_TEX(node_7849, _nosie_U));
                float2 node_1051 = (i.uv0+node_3559.g*float2(0,-0.015));
                float4 _nosie_V_var = tex2D(_nosie_V,TRANSFORM_TEX(node_1051, _nosie_V));
                float2 node_1541 = (i.uv0+node_3559.g*float2(0.01,0));
                float4 _nosie_U2_var = tex2D(_nosie_U2,TRANSFORM_TEX(node_1541, _nosie_U2));
                float2 node_3870 = (i.uv0+node_3559.g*float2(0,0.01));
                float4 _nosie_V2_var = tex2D(_nosie_V2,TRANSFORM_TEX(node_3870, _nosie_V2));
                float2 node_4608 = (i.uv0+((_noize_U_M*_nosie_U_var.r)+(_nosie_V_var.r*_noize_V_M)+(_nosie_U2_var.r*_nosie_U2_M)+(_nosie_V2_var.r*_nosie_v2_M)));
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_4608, _BumpMap)));
                float3 normalLocal = lerp(float3(0,0,0.5),_BumpMap_var.rgb,1.0);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4608, _MainTex));
                float3 emissive = ((_MainTex_var.rgb*_color.rgb)*_Alpha);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _nosie_U; uniform float4 _nosie_U_ST;
            uniform float _noize_U_M;
            uniform float4 _color;
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
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 node_4781 = _Time;
                float2 node_7849 = (i.uv0+node_4781.g*float2(-0.015,0));
                float4 _nosie_U_var = tex2D(_nosie_U,TRANSFORM_TEX(node_7849, _nosie_U));
                float2 node_1051 = (i.uv0+node_4781.g*float2(0,-0.015));
                float4 _nosie_V_var = tex2D(_nosie_V,TRANSFORM_TEX(node_1051, _nosie_V));
                float2 node_1541 = (i.uv0+node_4781.g*float2(0.01,0));
                float4 _nosie_U2_var = tex2D(_nosie_U2,TRANSFORM_TEX(node_1541, _nosie_U2));
                float2 node_3870 = (i.uv0+node_4781.g*float2(0,0.01));
                float4 _nosie_V2_var = tex2D(_nosie_V2,TRANSFORM_TEX(node_3870, _nosie_V2));
                float2 node_4608 = (i.uv0+((_noize_U_M*_nosie_U_var.r)+(_nosie_V_var.r*_noize_V_M)+(_nosie_U2_var.r*_nosie_U2_M)+(_nosie_V2_var.r*_nosie_v2_M)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4608, _MainTex));
                o.Emission = ((_MainTex_var.rgb*_color.rgb)*_Alpha);
                
                float3 diffColor = float3(0,0,0);
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
