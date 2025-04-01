Shader "FYY/Water"
{
    Properties
    {
        [Toggle(_VertexAlphaToggle_ON)]_VertexAlphaToggle("使用顶点透明度", int) = 0
        [Toggle(_ReceiveShadowToggle_ON)]_ReceiveShadow("接收阴影", int) = 0

        [Header(Water Color and Vertex)]
        [Space]
        _MaxDepth("可见深度", float) = 30
        _NoiseMap("水面顶点运动噪点贴图", 2D) = "white" {}
        _NoiseInfo("xy: 移动速度; z: 缩放; w: 强度", Vector) = (0.2, 0.1, 0.5, 0.1)
        _NoiseColorIntensity("Low Ploy型颜色反差强度", float) = 1
        [Space(20)]

        [Header(Distortion and Real Specular Texture)]
        [Space]
        _SurfaceMap("扰动贴图", 2D) = "white" {}
        _SurfaceInfo("xy: 缩放ab; zw: 混合因子ab", Vector) = (0.1, 0.4, 1, 0.5)
        _SurfaceMove("xy: 移动速度a; zw: 移动速度b", Vector) = (1, 1, 1, 1)
        _SurfaceIntensity("贴图法线混合（海浪）强度", Range(0 , 0.5)) = 0.1
        [Space(20)]

        [Header(Specular)]
        [Space]
        _LightRotation("xyz: 灯光位置; w: 整体高光颜色强度", Vector) = (1, 1, 1, 1)
        _SpecularBlendValue("反光混合，0时真实风格最明显", Range(0, 1)) = 1
        _GlossLowPoly("Low Poly风格反光集中度", float) = 50
        _GlossReal("真实风格反光集中度", float) = 50
        //_FresnelValue("反射菲涅尔强度", float) = 1

        [Header(Foam)]
        [Space]
        [Toggle(_FoamToggle_ON)]_FoamToggle("显示岸边泡沫", int) = 1
        _FoamSpread("泡沫扩散度", float) = 1
        _FoamMap("泡沫贴图", 2D) = "white" {}
        [Space(20)]

        [Header(WaterFall)]
        [Space]
        [Toggle(_WaterFallToggle_ON)]_WaterFallToggle("显示瀑布水流", int) = 0
        _WaterFallMapA("水流贴图A", 2D) = "white" {}
        _WaterFallMapB("水流贴图B", 2D) = "white" {}
        _WaterFallInfo("瀑布", Vector) = (0, 0, 0, 0)
        _WaterFallStrength("瀑布强度", Vector) = (0, 0, 0, 0)
        [HDR]_WaterFallColor("瀑布颜色", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent-100"
            "RenderPipeline" = "UniversalPipeline"
        }
        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            //Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "GerstnerWave.hlsl"
            #include "WaterSpecularLight.hlsl"
            // pragma
            #pragma target 3.0
            #pragma require derivatives
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _FoamToggle_ON
            #pragma shader_feature_local _WaterFallToggle_ON
            #pragma shader_feature_local _VertexAlphaToggle_ON
            #pragma shader_feature_local _ReceiveShadowToggle_ON
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS//主光源阴影
            #pragma multi_compile _ _SHADOWS_SOFT

            CBUFFER_START(UnityPerMaterial)
                half _MaxDepth;
                half4 _NoiseInfo;
                half _FoamSpread;
                half _GlossLowPoly;
                half _GlossReal;
                half _SpecularBlendValue;
                half4 _SurfaceMove;
                half4 _SurfaceInfo;
                half _SurfaceIntensity;
                half4 _LightRotation;

                half _WaveCount; // how many waves, set via the water component
                half4 _WaveData[10]; // 0-9 amplitude, direction, wavelength, omni, 10-19 origin.xy
                half _WaveSpeed;
                half _WavePeak;

                half _NoiseColorIntensity;
                half4 _WaterFallInfo;
                half4 _WaterFallColor;
                half4 _WaterFallStrength;
                half4 _WaterFallMapA_ST;
                half4 _WaterFallMapB_ST;
                half4 _NoiseMap_ST;
            CBUFFER_END


            TEXTURE2D(_CameraOpaqueTexture); SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_PlanarReflectionTexture); SAMPLER(sampler_PlanarReflectionTexture);
            TEXTURE2D(_AbsorptionScatteringRamp); SAMPLER(sampler_AbsorptionScatteringRamp);
            TEXTURE2D(_SurfaceMap); SAMPLER(sampler_SurfaceMap);
            TEXTURE2D(_FoamMap); SAMPLER(sampler_FoamMap);
            TEXTURE2D(_NoiseMap); SAMPLER(sampler_NoiseMap);
            TEXTURE2D(_WaterFallMapA); SAMPLER(sampler_WaterFallMapA);
            TEXTURE2D(_WaterFallMapB); SAMPLER(sampler_WaterFallMapB);

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 positionC : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float4 distortionUV : TEXCOORD0;
                float3 positionW : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float4 additionalData : TEXCOORD4;
                float2 uv1 : TEXCOORD5;
                float2 uv2: TEXCOORD6;
            };

            /**/
            half3 SampleReflections(half3 normalWS, half2 screenUV, half fresnelTerm)
            {
                half3 reflection = 0;
                half2 reflectionUV = screenUV + normalWS.zx * half2(0.02, 0.15);
                reflection += SAMPLE_TEXTURE2D(_PlanarReflectionTexture, sampler_PlanarReflectionTexture, reflectionUV).rgb;//planar reflection

                return reflection * fresnelTerm;
            }

            // half CalculateFresnelTerm(half3 normalWS, half3 viewDirectionWS)
            // {
                //     return pow(1.0 - saturate(dot(normalWS, viewDirectionWS)), _FresnelValue);
            // }

            half2 DistortionUVs(half depth, half3 normalWS)
            {
                half3 viewNormal = mul((half3x3)GetWorldToHClipMatrix(), -normalWS).xyz;
                return viewNormal.xz * saturate((depth) * 0.005);
            }

            half4 AdditionalData(half3 positionW, WaveStruct wave)
            {
                half4 data = half4(0.0, 0.0, 0.0, 0.0);
                half3 viewPos = TransformWorldToView(positionW);    //世界到相机空间
                data.x = length(viewPos / viewPos.z);   // distance to surface
                data.y = length(GetCameraPositionWS().xyz - positionW); // local position in camera space
                data.z = wave.position.y / 1; // encode the normalized wave height into additional data
                data.w = wave.position.x + wave.position.z;
                return data;
            }

            half3 Absorption(half depth)
            {
                return SAMPLE_TEXTURE2D(_AbsorptionScatteringRamp, sampler_AbsorptionScatteringRamp, half2(depth, 0.0)).rgb;
            }

            half3 Scattering(half depth)
            {
                return SAMPLE_TEXTURE2D(_AbsorptionScatteringRamp, sampler_AbsorptionScatteringRamp, half2(depth, 0.375)).rgb;
            }

            half3 Refraction(half2 distortionUV)
            {
                return SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, distortionUV).rgb;
            }

            v2f vert(a2v v)
            {
                v2f o;

                o.normal = half3(0, 1, 0);
                o.positionW  = TransformObjectToWorld(v.vertex.xyz);
                // 表面顶点移动uv
                half2 noiseUV =  o.positionW.xz * _NoiseInfo.z + _Time.y * _NoiseInfo.xy;
                half3 noiseValue = (SAMPLE_TEXTURE2D_LOD(_NoiseMap, sampler_NoiseMap, noiseUV, 0) - 0.5) * _NoiseInfo.w;

                WaveStruct wave = GanerateWaves(o.positionW.xz, _WaveData, _WaveSpeed, _WavePeak, _WaveCount);
                o.positionW.y += noiseValue.x;

                o.positionW += wave.position;
                o.normal = wave.normal.xyz;

                // 扰动uv
                o.distortionUV.zw = o.positionW.xz * _SurfaceInfo.x + _Time.y * _SurfaceMove.xy;
                o.distortionUV.xy = o.positionW.xz * _SurfaceInfo.y + _Time.y * _SurfaceMove.zw;
                //

                o.positionC = TransformWorldToHClip(o.positionW);
                o.screenPos = ComputeScreenPos(o.positionC);
                o.viewDir = SafeNormalize(_WorldSpaceCameraPos - o.positionW);
                o.additionalData = AdditionalData(o.positionW, wave);
                o.uv1 = TRANSFORM_TEX(v.uv, _WaterFallMapA);
                o.uv2 = TRANSFORM_TEX(v.uv, _WaterFallMapB);

                o.color = v.color;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 finalRes = 0;
                half3 screenUV = i.screenPos.xyz / i.screenPos.w;

                Light mainLight = GetMainLight(TransformWorldToShadowCoord(i.positionW));
                half4 lightDir = LightDirection(_LightRotation);

                // 水下深度计算
                half rawD = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV.xy);
                half cameraDepline = LinearEyeDepth(rawD, _ZBufferParams);  //转换成线性变换
                half depth = cameraDepline * i.additionalData.x - i.additionalData.y;
                half adjustDepth = saturate(depth * 0.25 + 0.25);
                half depthMulti = 1 / _MaxDepth;

                // 扰动uv //?
                half2 detailBump1 = SAMPLE_TEXTURE2D(_SurfaceMap, sampler_SurfaceMap, i.distortionUV.zw).xy * 2 - 1;  // ?
                half2 detailBump2 = SAMPLE_TEXTURE2D(_SurfaceMap, sampler_SurfaceMap, i.distortionUV.xy).xy * 2 - 1;
                half2 detailBump = (detailBump1 * _SurfaceInfo.z + detailBump2  * _SurfaceInfo.w) ;
                half3 detailBumpNormal = half3(detailBump.x, 0, detailBump.y);

                // Low Poly型颜色
                half3 normalNoise = normalize(cross(ddy(i.positionW) , ddx(i.positionW)));
                half3 noiseColor = max(0, dot(normalNoise, lightDir)) * _NoiseColorIntensity;

                // 扰动
                half2 distortion = DistortionUVs(depth, detailBumpNormal * _SurfaceIntensity);
                distortion += screenUV.xy;

                // 深度相关颜色渐变
                half3 absorptionColor = Absorption(depth * depthMulti);

                // sssColor颜色渐变
                half shadow = mainLight.shadowAttenuation;
                half3 GI = shadow;  //!
                half3 sssColor = half3(2, 2, 2);
                // return half4(sssColor, 1);
                #ifdef _ReceiveShadowToggle_ON
                    // sssColor = saturate(shadow * mainLight.color + GI);
                    sssColor = shadow * mainLight.color + GI;
                #endif
                sssColor *= Scattering(depth * depthMulti);
                // return half4(sssColor, 1);

                // 镜面光
                half3 normalSpecular = normalize(i.normal + detailBumpNormal *_SurfaceIntensity);
                half specLowPoly = pow(max(0, dot(normalNoise, normalize(lightDir + i.viewDir))), _GlossLowPoly * 100).r;
                half specReal = pow(max(0, dot(normalSpecular, normalize(lightDir + i.viewDir))), _GlossReal * 100).r;
                half specRes = (specLowPoly * _SpecularBlendValue + specReal * (1 - _SpecularBlendValue)) * _LightRotation.w;

                finalRes = Refraction(distortion);

                half vertexAlphaCtrl = 1;
                #ifdef _VertexAlphaToggle_ON
                    vertexAlphaCtrl = i.color.a;
                #endif

                half3 origin = finalRes * (1 - vertexAlphaCtrl);

                finalRes *= absorptionColor.rgb * vertexAlphaCtrl;

                finalRes += (sssColor * noiseColor + specRes) * vertexAlphaCtrl;
                // return half4(finalRes, 1);

                // 岸边泡沫
                #ifdef _FoamToggle_ON
                    half foamMap = SAMPLE_TEXTURE2D(_FoamMap, sampler_FoamMap, i.distortionUV.zw).x; //r=thick, g=medium, b=light
                    half foamCtrl = saturate(1 - depth * _FoamSpread);
                    half foamValue = step(foamMap, foamCtrl) * (mainLight.shadowAttenuation * mainLight.color + GI);
                    finalRes += foamValue * 0.3 * vertexAlphaCtrl;
                #endif

                // 流水
                #ifdef _WaterFallToggle_ON
                    half waterfall1 = SAMPLE_TEXTURE2D(_WaterFallMapA, sampler_WaterFallMapA, i.uv1 + _WaterFallInfo.xy * _Time.y).x + _WaterFallStrength.x;  //
                    half waterfall2 = SAMPLE_TEXTURE2D(_WaterFallMapB, sampler_WaterFallMapB, i.uv2 + _WaterFallInfo.zw * _Time.y).x + _WaterFallStrength.z;
                    waterfall1 = pow(waterfall1, _WaterFallStrength.y);
                    waterfall2 = pow(waterfall2, _WaterFallStrength.w);
                    half waterfall = saturate((waterfall1 * waterfall2) * i.color.r) ;
                    // return half4(waterfall.xxx, 1);

                    finalRes = (1- waterfall) * finalRes + waterfall * _WaterFallColor.rgb * _WaterFallColor.a;

                    // return half4(finalRes, 1);
                #endif

                // 最终结果
                return half4(finalRes + origin, 1);
            }
            ENDHLSL
        }
    }
}