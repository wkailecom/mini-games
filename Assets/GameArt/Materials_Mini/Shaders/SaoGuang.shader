// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Effect/SaoGuang"
{
	Properties
	{
		[Toggle(_TONGDAORA_ON)] _TongDaoRA("TongDaoR/A", Float) = 0
		_MianTex("MianTex", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Rotation("Rotation", Float) = 0
		_Xspeed("Xspeed", Float) = 0
		_Yspeed("Yspeed", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Contrast("Contrast", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _TONGDAORA_ON
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform sampler2D _MianTex;
		uniform float _Xspeed;
		uniform float _Yspeed;
		uniform float _Rotation;
		uniform float _Contrast;
		uniform float4 _Color;
		SamplerState sampler_MianTex;
		uniform sampler2D _TextureSample0;
		SamplerState sampler_TextureSample0;
		uniform float4 _TextureSample0_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 appendResult21 = (float4(_Xspeed , _Yspeed , 0.0 , 0.0));
			float cos11 = cos( ( (0.0 + (_Rotation - 0.0) * (6.28 - 0.0) / (1.0 - 0.0)) / 360.0 ) );
			float sin11 = sin( ( (0.0 + (_Rotation - 0.0) * (6.28 - 0.0) / (1.0 - 0.0)) / 360.0 ) );
			float2 rotator11 = mul( i.uv_texcoord - float2( 0,0 ) , float2x2( cos11 , -sin11 , sin11 , cos11 )) + float2( 0,0 );
			float2 panner17 = ( 1.0 * _Time.y * appendResult21.xy + rotator11);
			float4 tex2DNode1 = tex2D( _MianTex, panner17 );
			float4 temp_cast_1 = (_Contrast).xxxx;
			o.Emission = ( i.vertexColor * pow( tex2DNode1 , temp_cast_1 ) * _Color ).rgb;
			#ifdef _TONGDAORA_ON
				float staticSwitch3 = tex2DNode1.a;
			#else
				float staticSwitch3 = tex2DNode1.r;
			#endif
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			o.Alpha = ( i.vertexColor.a * _Color.a * staticSwitch3 * tex2D( _TextureSample0, uv_TextureSample0 ).r );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
2121;73;1007;805;1713.064;504.532;1.722478;True;False
Node;AmplifyShaderEditor.RangedFloatNode;9;-2156.814,253.3749;Inherit;False;Property;_Rotation;Rotation;4;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;14;-1994.541,261.9838;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;6.28;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1973.416,434.9009;Inherit;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;False;360;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1646.275,229.1802;Inherit;False;Property;_Xspeed;Xspeed;5;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1645.247,307.2102;Inherit;False;Property;_Yspeed;Yspeed;6;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;15;-1788.15,264.4207;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1964.504,90.39841;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;21;-1472.761,238.4206;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RotatorNode;11;-1661.407,92.19843;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;17;-1287.953,98.78841;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-1029.906,75.71419;Inherit;True;Property;_MianTex;MianTex;2;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;25;-1006.575,290.3265;Inherit;False;Property;_Contrast;Contrast;8;0;Create;True;0;0;False;0;False;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;3;-347.21,239.4313;Inherit;False;Property;_TongDaoRA;TongDaoR/A;1;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-624.6939,349.1025;Inherit;False;Property;_Color;Color;3;0;Create;True;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;2;-427.1443,-115.5508;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-682.603,533.2833;Inherit;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;24;-530.7548,77.43517;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-33.18501,289.5895;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-30.271,35.86727;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;143.9352,-14.07702;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Effect/SaoGuang;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;False;Custom;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;9;0
WireConnection;15;0;14;0
WireConnection;15;1;16;0
WireConnection;21;0;18;0
WireConnection;21;1;19;0
WireConnection;11;0;12;0
WireConnection;11;2;15;0
WireConnection;17;0;11;0
WireConnection;17;2;21;0
WireConnection;1;1;17;0
WireConnection;3;1;1;1
WireConnection;3;0;1;4
WireConnection;24;0;1;0
WireConnection;24;1;25;0
WireConnection;6;0;2;4
WireConnection;6;1;4;4
WireConnection;6;2;3;0
WireConnection;6;3;23;1
WireConnection;5;0;2;0
WireConnection;5;1;24;0
WireConnection;5;2;4;0
WireConnection;0;2;5;0
WireConnection;0;9;6;0
ASEEND*/
//CHKSM=89B13380832765347CFD3C7CB456AC4D9D60066A