// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Effect/LS_Add_RaoDong"
{
	Properties
	{
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_U("U", Float) = 0
		_V("V", Float) = 0
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (1,1,1,1)
		_Float0("Float 0", Float) = 0.1
		_Mask("Mask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _NoiseTex;
		uniform float _U;
		uniform float _V;
		uniform float4 _NoiseTex_ST;
		uniform float _Float0;
		uniform sampler2D _Mask;
		SamplerState sampler_Mask;
		uniform float4 _Mask_ST;
		uniform float4 _Color0;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 appendResult10 = (float4(_U , _V , 0.0 , 0.0));
			float2 uv_NoiseTex = i.uv_texcoord * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
			float2 panner3 = ( 1.0 * _Time.y * appendResult10.xy + uv_NoiseTex);
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			o.Emission = ( ( tex2D( _MainTex, ( float4( uv_MainTex, 0.0 , 0.0 ) + ( tex2D( _NoiseTex, panner3 ) * _Float0 ) ).rg ) * tex2D( _Mask, uv_Mask ).r ) * _Color0 * i.vertexColor * i.vertexColor.a ).rgb;
			float temp_output_25_4 = i.vertexColor.a;
			o.Alpha = temp_output_25_4;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
2121;73;862;805;215.5224;836.4165;2.402924;True;False
Node;AmplifyShaderEditor.CommentaryNode;12;-1384.118,-281.8537;Inherit;False;1317.858;557;UV流动;4;2;5;7;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;13;-1293.118,-18.85377;Inherit;False;383;274;提取UV的X和Y的轴向;3;9;8;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;7;-1295.118,-231.8537;Inherit;False;292;209;UV坐标节点;1;6;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1244.26,145.7858;Inherit;False;Property;_V;V;3;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1244.26,33.78575;Inherit;False;Property;_U;U;1;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1245.118,-181.8537;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1068.26,49.78575;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;5;-828.1175,-186.8537;Inherit;False;255;209;平移节点;1;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;16;-19.39404,-250.276;Inherit;False;866.3254;469.3427;UV扰动;4;11;15;14;19;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;3;-778.1175,-136.8537;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;19;-3.614437,1.852043;Inherit;False;398.8835;197.2135;流动强度;2;17;18;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2;-491.2595,-101.2142;Inherit;False;370;280;贴图;1;1;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;18;32.08554,97.36535;Inherit;False;Property;_Float0;Float 0;6;0;Create;True;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-441.2595,-51.21424;Inherit;True;Property;_NoiseTex;NoiseTex;0;0;Create;True;0;0;False;0;False;-1;None;b39e6e61f6e61994a91b9933e65cd734;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;155.8971,-200.276;Inherit;False;0;11;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;237.5888,65.40463;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;391.0281,-94.65253;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;11;537.3314,-124.0332;Inherit;True;Property;_MainTex;MainTex;4;0;Create;True;0;0;False;0;False;-1;None;59b3e750f6d04a2479c8624dc51f938d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;621.8427,246.8331;Inherit;True;Property;_Mask;Mask;7;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;25;937.9677,-490.6592;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;20;1037.543,128.6325;Inherit;False;Property;_Color0;Color 0;5;1;[HDR];Create;True;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;959.5235,-106.0043;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;1254.041,-112.8673;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1553.199,-144.71;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Effect/LS_Add_RaoDong;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;4;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;8;0
WireConnection;10;1;9;0
WireConnection;3;0;6;0
WireConnection;3;2;10;0
WireConnection;1;1;3;0
WireConnection;17;0;1;0
WireConnection;17;1;18;0
WireConnection;15;0;14;0
WireConnection;15;1;17;0
WireConnection;11;1;15;0
WireConnection;24;0;11;0
WireConnection;24;1;23;1
WireConnection;21;0;24;0
WireConnection;21;1;20;0
WireConnection;21;2;25;0
WireConnection;21;3;25;4
WireConnection;0;2;21;0
WireConnection;0;9;25;4
ASEEND*/
//CHKSM=AE88595A64CD2F1C7D6985718338ED7ACF83C8EC