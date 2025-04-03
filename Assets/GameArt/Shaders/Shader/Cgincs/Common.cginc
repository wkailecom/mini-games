fixed OverlayBlendMode(fixed basePixel, fixed blendPixel) {
	float param = step(0.5, basePixel);
	return (1 - param) * (2.0 * basePixel * blendPixel) + param * (1.0 - 2.0 * (1.0 - basePixel) * (1.0 - blendPixel));
}
fixed4 OverlayBlend(fixed4 baseColor, fixed4 blendColor)
{
	baseColor.r = OverlayBlendMode(baseColor.r, blendColor.r);
	baseColor.g = OverlayBlendMode(baseColor.g, blendColor.g);
	baseColor.b = OverlayBlendMode(baseColor.b, blendColor.b);
	return baseColor;
}
fixed3 Multiply(fixed3 baseColor, fixed4 blendColor)
{
	return lerp(baseColor, baseColor * blendColor.rgb, blendColor.a);
}

float3 HUEtoRGB(in float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float3(R, G, B));
}

float3 HCYtoRGB(in float3 HCY)
{
	float3 HCYwts = float3(0.299, 0.587, 0.114);
	float3 RGB = HUEtoRGB(HCY.x);
	float Z = dot(RGB, HCYwts);
	if (HCY.z < Z)
	{
		HCY.y *= HCY.z / Z;
	}
	else if (Z < 1)
	{
		HCY.y *= (1 - HCY.z) / (1 - Z);
	}
	return (RGB - Z) * HCY.y + HCY.z;
}

float3 RGBtoHCV(in float3 RGB)
{
	float Epsilon = 1e-10;
	// Based on work by Sam Hocevar and Emil Persson
	float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
	float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
	float C = Q.x - min(Q.w, Q.y);
	float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
	return float3(H, C, Q.x);
}

float3 RGBtoHCY(in float3 RGB)
{
	float3 HCYwts = float3(0.299, 0.587, 0.114);
	float Epsilon = 1e-10;
	// Corrected by David Schaeffer
	float3 HCV = RGBtoHCV(RGB);
	float Y = dot(RGB, HCYwts);
	float Z = dot(HUEtoRGB(HCV.x), HCYwts);
	if (Y < Z)
	{
		HCV.y *= Z / (Epsilon + Y);
	}
	else
	{
		HCV.y *= (1 - Z) / (Epsilon + 1 - Y);
	}
	return float3(HCV.x, HCV.y, Y);
}
half3 BlendColor(half4 base, half4 blend)
{
	half3 blendHCY = RGBtoHCY(blend.rgb);
	half3 result = HCYtoRGB(half3(blendHCY.r, blendHCY.g, RGBtoHCY(base).b));
	return lerp(base.rgb, result, blend.a);
}
float3 RGBConvertToHSV(float3 rgb)
{
	float R = rgb.x, G = rgb.y, B = rgb.z;
	float3 hsv;
	float max1 = max(R, max(G, B));
	float min1 = min(R, min(G, B));
	if (R == max1)
	{
		hsv.x = (G - B) / (max1 - min1);
	}
	if (G == max1)
	{
		hsv.x = 2 + (B - R) / (max1 - min1);
	}
	if (B == max1)
	{
		hsv.x = 4 + (R - G) / (max1 - min1);
	}
	hsv.x = hsv.x * 60.0;
	if (hsv.x < 0)
		hsv.x = hsv.x + 360;
	hsv.z = max1;
	hsv.y = (max1 - min1) / max1;
	return hsv;
}

//HSV to RGB

fixed4 MultiplyColor(fixed4 a, fixed4 b)
{
	fixed4 outColor;
	if (a.r < 0.5)
	{
		outColor = 2 * a*b;
	}
	else
	{
		outColor = 1 - 2 * (1 - a)*(1 - b);
	}
	return outColor;

}