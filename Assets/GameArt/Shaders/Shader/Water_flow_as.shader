// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water_flow_as"
{
	Properties
	{
		_Bg_Moonlight("Bg_Moonlight", 2D) = "white" {}
		_Texture0("Texture 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Bg_Moonlight;
		uniform sampler2D _Texture0;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord9 = i.uv_texcoord * float2( 1.5,2 );
			float2 panner7 = ( 1.0 * _Time.y * float2( -0.05,-0.01 ) + uv_TexCoord9);
			float2 uv_TexCoord27 = i.uv_texcoord * float2( 1,2 );
			float2 panner28 = ( 1.0 * _Time.y * float2( -0.01,-0.01 ) + uv_TexCoord27);
			float4 tex2DNode1 = tex2D( _Bg_Moonlight, ( i.uv_texcoord + ( ( tex2D( _Texture0, panner7 ).r * 0.15 ) * ( tex2D( _Texture0, panner28 ).r * 0.15 ) * 4.0 ) ) );
			o.Emission = ( tex2DNode1 * i.vertexColor ).rgb;
			o.Alpha = ( tex2DNode1.a * i.vertexColor.a );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
1613.6;52;1261;778;2174.362;119.5465;1.647111;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;27;-2177.639,550.9556;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-2201.352,48.75708;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1.5,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;26;-1993.899,239.7459;Float;True;Property;_Texture0;Texture 0;1;0;Create;True;0;0;False;0;None;9789d23040cb1fb45ad60392430c3c15;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;7;-1882.088,44.33791;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.05,-0.01;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;28;-1868.06,540.6583;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.01,-0.01;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;29;-1618.319,511.7317;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;61c0b9c0523734e0e91bc6043c72a490;9789d23040cb1fb45ad60392430c3c15;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-1418.531,423.1834;Float;False;Constant;_Float0;_Float0;2;0;Create;True;0;0;False;0;0.15;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-1606.371,13.41308;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;61c0b9c0523734e0e91bc6043c72a490;9789d23040cb1fb45ad60392430c3c15;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1205.76,243.521;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1169.908,584.1546;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1124.227,844.3388;Float;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-964.8107,-214.9705;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-901.6716,397.1761;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-670.7242,-213.5184;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-458.7846,-244.7281;Float;True;Property;_Bg_Moonlight;Bg_Moonlight;0;0;Create;True;0;0;False;0;3253f3b83418a1f4687d5dcf4087d6e0;fd6747818b7e23443a7f867fdfaed11f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;2;-331.9609,96.88615;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-47.418,-164.3246;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-53.22985,240.5947;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;185.506,-93.48838;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Water_flow_as;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;9;0
WireConnection;28;0;27;0
WireConnection;29;0;26;0
WireConnection;29;1;28;0
WireConnection;6;0;26;0
WireConnection;6;1;7;0
WireConnection;12;0;6;1
WireConnection;12;1;14;0
WireConnection;30;0;29;1
WireConnection;30;1;14;0
WireConnection;32;0;12;0
WireConnection;32;1;30;0
WireConnection;32;2;35;0
WireConnection;10;0;5;0
WireConnection;10;1;32;0
WireConnection;1;1;10;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;4;0;1;4
WireConnection;4;1;2;4
WireConnection;0;2;3;0
WireConnection;0;9;4;0
ASEEND*/
//CHKSM=8E6BF636C386A3E8FE398C30FD2E44AC98B215B0