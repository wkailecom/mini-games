#ifndef WATER_SPECULAR_LIGHT_INCLUDED
#define WATER_SPECULAR_LIGHT_INCLUDED

   inline half4x4 GetRotateMatrix(half3 eularAngle)
    {
        eularAngle.xyz*=0.01745;  //角度转弧度（rad）：度数 * (UNITY_PI / 180)

        half angleX = eularAngle.x;   // X轴旋转
        half angleY = eularAngle.y;   // Y轴旋转
        half angleZ = eularAngle.z;   // Z轴旋转

        half sinX = sin(angleX);
        half cosX = cos(angleX);
        half sinY = sin(angleY);
        half cosY = cos(angleY);
        half sinZ = sin(angleZ);
        half cosZ = cos(angleZ);

        half m00 = cosY * cosZ;
        half m01 = -cosY * sinZ;
        half m02 = sinY;
        half m03 = 0;
        half m10 = cosX * sinZ + sinX * sinY * cosZ;
        half m11 = cosX * cosZ - sinX * sinY * sinZ;
        half m12 = -sinX * cosY;
        half m13 = 0;
        half m20 = sinX * sinZ - cosX * sinY * cosZ;
        half m21 = sinX * cosZ + cosX * sinY * sinZ;
        half m22 = cosX * cosY;
        half m23 = 0;
        half m30 = 0;
        half m31 = 0;
        half m32 = 0;
        half m33 = 1;

        return  half4x4(m00,m01,m02,m03,m10,m11,m12,m13,m20,m21,m22,m23,m30,m31,m32,m33);
    }

    inline half4 LightDirection(half3 lightRotation)
    {
        half4x4 matrixRota = GetRotateMatrix(lightRotation);
        half4 LightDirection = mul(matrixRota, half4(0,0,1,0));
        return LightDirection;
    }

#endif
