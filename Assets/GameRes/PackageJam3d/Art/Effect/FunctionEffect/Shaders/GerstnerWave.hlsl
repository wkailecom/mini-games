#ifndef GERSTNER_WAVE_INCLUDED
#define GERSTNER_WAVE_INCLUDED

    struct WaveStruct
    {
        float3 position;
        float3 normal;
    };

    struct Wave
    {
        half amplitude;
        half direction;
        half wavelength;
        half speed;
        half peak;
    };

    // WaveStruct SinWave(half2 pos, half waveCountMulti, half amplitude, half angle, half wavelength)
    // {
    //     WaveStruct waveOut;
    //     half w = 6.28318 / wavelength;
    //     angle = radians(angle);
    //     half2 direction = half2(sin(angle), cos(angle));
    //     half dir = dot(direction, pos);
    //     half calc = dir * w + _Time.y * _WaveSpeed; // the wave calculation
    //     half cosCalc = cos(calc);
    //     half sinCalc = sin(calc);
    //     waveOut.position = 0;
    //     waveOut.position.y = amplitude * sinCalc * waveCountMulti;
    //     waveOut.normal = normalize(float3(-w*direction.x*amplitude*sinCalc, 1, -w*direction.y*amplitude*sinCalc)) * waveCountMulti;
    //     return waveOut;
    // }

    WaveStruct GerstnerWave(half2 pos, half waveCountMulti, half amplitude, half angle, half wavelength, half waveSpeed, half wavePeak, half waveCount)
    {
        WaveStruct waveOut;
        half w = 6.28318 / wavelength;
        half qi = wavePeak / (amplitude * w * waveCount);

        angle = radians(angle);
        half2 windDir = normalize(half2(sin(angle), cos(angle)));

        half dir = dot(windDir, pos);
        half calc = dir * w + _Time.y * waveSpeed; // the wave calculation
        half cosCalc = cos(calc);
        half sinCalc = sin(calc);
        waveOut.position.xz = qi * amplitude * windDir.xy * cosCalc;;
        waveOut.position.y = amplitude * sinCalc*waveCountMulti;
        half wa = w * amplitude;
        half3 n = half3(-(windDir.xy * wa * cosCalc),
        1-(qi * wa * sinCalc));
        waveOut.normal = (n * waveCountMulti).xzy;
        return waveOut;
    }

    WaveStruct GanerateWaves(half2 pos, half4 waveData[10], half waveSpeed, half wavePeak, half waveCount)
    {
        WaveStruct wave;
        wave.position = 0;
        wave.normal = 0;
        half waveCountMulti = 1.0 / waveCount;
        UNITY_LOOP
        for(uint i = 0; i < waveCount; i++)
        {
            WaveStruct w = GerstnerWave(pos, waveCountMulti, waveData[i].x, waveData[i].y, waveData[i].z, waveSpeed, wavePeak, waveCount);
            wave.position += w.position;
            wave.normal += w.normal;
        }
        return wave;
    }
#endif
