Shader "Custom/UI/FELightSweep" {

	Properties{

		[HideInInspector]

		_MainTex("Base (RGB)", 2D) = "white" {}
		

		_FlashColor("Flash Color", Color) = (1,1,1,1)

		_Angle("Flash Angle", Range(0, 180)) = 45

		_Width("Flash Width", Range(0, 1)) = 0.2

		_LoopTime("Loop Time", Float) = 0.5

		_Interval("Time Interval", Float) = 1.5
	
		_Persent("Persent", Float) = 0

		[HideInInspector]
		_AtlasPosition("AtlasPosition",Vector) = (0,0,1,1)

	}

		SubShader

		{

			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

			LOD 200

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma surface surf Lambert alpha exclude_path:prepass noforwardadd
            
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			

			float4 _FlashColor;

			float _Angle;

			float _Width;

			float _LoopTime;

			float _Interval;
			
			float4 _AtlasPosition;

			float _Persent;
			
			struct Input

			{

				half2 uv_MainTex;
				float4 color : COLOR;
			};

			float inFlash(half2 uv)

			{

				float brightness = 0;

				float angleInRad = 0.0174444 * _Angle;

				float tanInverseInRad = 1.0 / tan(angleInRad);

				float currentTime = _Time.y;

				float totalTime = _Interval + _LoopTime;

				float currentTurnStartTime = (int)((currentTime / totalTime)) * totalTime;

				float currentTurnTimePassed = currentTime - currentTurnStartTime - _Interval;

				bool onLeft = (tanInverseInRad > 0);

				float xBottomFarLeft = onLeft ? 0.0 : tanInverseInRad;

				float xBottomFarRight = onLeft ? (1.0 + tanInverseInRad) : 1.0;

				float percent = _Persent;//currentTurnTimePassed / _LoopTime;

				float xBottomRightBound = xBottomFarLeft + percent * (xBottomFarRight - xBottomFarLeft);

				float xBottomLeftBound = xBottomRightBound - _Width;

				float xProj = uv.x + uv.y * tanInverseInRad;

				if (xProj > xBottomLeftBound && xProj < xBottomRightBound)

				{

					brightness = 1.0 - abs(2.0 * xProj - (xBottomLeftBound + xBottomRightBound)) / _Width;

				}

				return brightness;

			}

			void surf(Input IN, inout SurfaceOutput o)
			{
			
              
                float2 uv = IN.uv_MainTex;

				half4 texCol = tex2D(_MainTex,  IN.uv_MainTex);
				float brightness = inFlash(uv);
				o.Emission = texCol.rgb + _FlashColor.rgb * brightness;//改为输出Emission，不受光影响

				o.Alpha = texCol.a * IN.color.a;

			}

			ENDCG

		}

			FallBack "Diffuse"

}