// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "KK/Water_wave_ice"
{
	Properties
	{
		_Bg_Diamondshui("Bg_Diamondshui", 2D) = "white" {}
		_Braum_base_E_rising_energy_cas("Braum_base_E_rising_energy_cas", 2D) = "white" {}
		_Float0("Float 0", Range( 0 , 1)) = 0.125
		_Float1("Float 1", Range( 0 , 1.4)) = 1.124
		_Color0("Color 0", Color) = (1,1,1,1)
		_Vector0("Vector 0", Vector) = (4.4,0.69,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _Color0;
		uniform sampler2D _Bg_Diamondshui;
		uniform sampler2D _Braum_base_E_rising_energy_cas;
		uniform float2 _Vector0;
		uniform float _Float1;
		uniform float _Float0;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_TexCoord3 = i.uv_texcoord * _Vector0;
			float cos5 = cos( _Float1 );
			float sin5 = sin( _Float1 );
			float2 rotator5 = mul( uv_TexCoord3 - float2( 0.5,0.5 ) , float2x2( cos5 , -sin5 , sin5 , cos5 )) + float2( 0.5,0.5 );
			float2 panner6 = ( 1.0 * _Time.y * float2( 0.1,-0.01 ) + rotator5);
			float4 tex2DNode1 = tex2D( _Bg_Diamondshui, ( i.uv_texcoord + ( tex2D( _Braum_base_E_rising_energy_cas, panner6 ).b * _Float0 ) ) );
			c.rgb = ( tex2DNode1 * _Color0 ).rgb;
			c.a = ( _Color0.a * i.vertexColor * tex2DNode1.a ).r;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
673.6;115.2;1523;796;2795.684;191.1636;1.356608;True;True
Node;AmplifyShaderEditor.Vector2Node;26;-2114.668,38.10324;Float;False;Property;_Vector0;Vector 0;5;0;Create;True;0;0;False;0;4.4,0.69;0.72,4.14;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;16;-1854.502,254.2184;Float;False;Property;_Float1;Float 1;3;0;Create;True;0;0;False;0;1.124;1.4;0;1.4;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1895.497,11.40601;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;5;-1597.948,37.27968;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0.25;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;6;-1297.165,30.81125;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,-0.01;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-983.1366,6.602695;Float;True;Property;_Braum_base_E_rising_energy_cas;Braum_base_E_rising_energy_cas;1;0;Create;True;0;0;False;0;dd26ef78a5c62ee4eae39044fc206802;dd26ef78a5c62ee4eae39044fc206802;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-1280.806,468.4927;Float;True;Property;_Float0;Float 0;2;0;Create;True;0;0;False;0;0.125;0.102;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-583.5789,342.3498;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-836.6951,-259.701;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-344.9301,-21.05029;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-168.5434,-132.4334;Float;True;Property;_Bg_Diamondshui;Bg_Diamondshui;0;0;Create;True;0;0;False;0;620e58e0b07f47f429589b275638a711;620e58e0b07f47f429589b275638a711;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-77.44935,108.7465;Float;False;Property;_Color0;Color 0;4;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;13;-155.1794,459.3456;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;264.897,-36.06759;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;215.9177,386.1865;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;25;493.5729,-135.597;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;KK/Water_wave_ice;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;True;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;26;0
WireConnection;5;0;3;0
WireConnection;5;2;16;0
WireConnection;6;0;5;0
WireConnection;2;1;6;0
WireConnection;8;0;2;3
WireConnection;8;1;7;0
WireConnection;10;0;9;0
WireConnection;10;1;8;0
WireConnection;1;1;10;0
WireConnection;12;0;1;0
WireConnection;12;1;11;0
WireConnection;14;0;11;4
WireConnection;14;1;13;0
WireConnection;14;2;1;4
WireConnection;25;9;14;0
WireConnection;25;13;12;0
ASEEND*/
//CHKSM=5DF12B164CE17EB47E6DC9826AA0D7D5BC7BCE94