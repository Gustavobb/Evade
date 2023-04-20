Shader "Unlit/Shapes"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PrevFrame ("Previous Frame", 2D) = "white" {}
        _ColorDecay ("Color Decay", Range(0, 1)) = 0.01
        _Background ("Background", Color) = (0, 0, 0, 1)
        _Shadow ("Shadow", Range(0, 1)) = 0.0
        _ShadowColor ("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
        _ShadowOffset ("Shadow Offset", Range(0, 1)) = 0.01
        _SunRotation ("Sun Rotation", Range(0, 360)) = 0
        _FilmGrain ("Film Grain", Range(0, 1)) = 0.0
        _GrainStrength ("Grain Strength", Range(0, 1000)) = 32.0
        _ChromaticAberration ("Chromatic Aberration", Range(0, 1)) = 0.0
        _LensDistortion ("Lens Distortion", Range(-3, 3)) = 0.0
        _LensZoom ("Lens Zoom", Range(-10, 10)) = 1.0
        _Pixelate ("Pixelate", Range(0, 1)) = 0.0
        _PixelSize ("Pixel Size", Range(0, 64)) = 0.01
        _HueShift ("Hue Shift", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _PrevFrame;
            float _ColorDecay;
            float4 _Background;
            float _Shadow;
            fixed4 _ShadowColor;
            float _ShadowOffset;
            float _FilmGrain;
            float _GrainStrength;
            float _SunRotation;
            float _ChromaticAberration;
            float _LensDistortion;
            float _LensZoom;
            float _Pixelate;
            float _PixelSize;
            float _HueShift;

            // Circles: x, y, radius, scale and color
            int _CirclesCount;
            float4 _CirclesProperties[100];
            float4 _CirclesExtra[100];
            float4 _CirclesColor[100];

            // Rectangles: x, y, width, height and color
            int _RectanglesCount;
            float4 _RectanglesProperties[100];
            float4 _RectanglesExtra[100];
            float4 _RectanglesColor[100];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 Rotate(float2 v, float angle)
            {
                angle = radians(angle);
                float c = cos(angle);
                float s = sin(angle);
                return float2(c * v.x - s * v.y, s * v.x + c * v.y);
            }

            fixed4 RainbowColor(float2 uv)
            {
                return fixed4(0.5 + cos(_Time.y + uv.xyx + float3(0, 2, 4)), 1);
            }

            /////// SHAPE CHECKING ///////
            bool IsPointInCircle(float2 p, float2 circleCenter, float radius)
            {
                float2 diff = p - circleCenter;
                float distance = dot(diff, diff);
                return distance < radius * radius;
            }

            bool IsPointInRectangle(float2 p, float2 rectCenter, float2 rectSize, float rotationZ)
            {
                float2 diff = p - rectCenter;
                float2 rotatedDiff = Rotate(diff, rotationZ);
                return abs(rotatedDiff.x) < rectSize.x && abs(rotatedDiff.y) < rectSize.y;
            }

            struct HitInfo
            {
                fixed4 col;
                bool hit;
            };

            fixed4 HandleColor(float2 uv, float4 color, float colorType, float blend)
            {
                switch (colorType)
                {
                    case 0.0: return lerp(0, color, blend);
                    case 1.0: return lerp(0, RainbowColor(uv), blend);
                    default: return 0;
                }
            }

            HitInfo CheckCircle(float2 uv, HitInfo hitInfo, float2 shadowDir)
            {
                for (int k = 0; k < _CirclesCount; k++)
                {
                    if (IsPointInCircle(uv, _CirclesProperties[k].xy, _CirclesProperties[k].z))
                    {
                        HitInfo hitInfo = { HandleColor(uv, _CirclesColor[k], _CirclesExtra[k].x, _CirclesExtra[k].z), true };
                        return hitInfo;
                    }

                    if (_CirclesExtra[k].y > 0 && _Shadow > 0)
                    {
                        // add shadow
                        float2 shadowCenter = _CirclesProperties[k].xy + shadowDir;
                        if (IsPointInCircle(uv, shadowCenter, _CirclesProperties[k].z))
                        {
                            HitInfo hitInfo = { _CirclesExtra[k].x ? 0 : _ShadowColor, true };
                            return hitInfo;
                        }
                    }
                }

                return hitInfo;
            }

            HitInfo CheckRectangle(float2 uv, HitInfo hitInfo, float2 shadowDir)
            {
                for (int k = 0; k < _RectanglesCount; k++)
                {
                    if (IsPointInRectangle(uv, _RectanglesProperties[k].xy, _RectanglesProperties[k].zw, _RectanglesExtra[k].y))
                    {
                        HitInfo hitInfo = { HandleColor(uv, _RectanglesColor[k], _RectanglesExtra[k].x, _RectanglesExtra[k].w), true };
                        return hitInfo;
                    }

                    if (_RectanglesExtra[k].z > 0 && _Shadow > 0)
                    {
                        // add shadow
                        float2 shadowCenter = _RectanglesProperties[k].xy + shadowDir;
                        if (IsPointInRectangle(uv, shadowCenter, _RectanglesProperties[k].zw, _RectanglesExtra[k].y))
                        {
                            HitInfo hitInfo = { _ShadowColor, true };
                            return hitInfo;
                        }
                    }
                }

                return hitInfo;
            }
            /////// SHAPE CHECKING ///////

            fixed4 Explosion(float2 uv)
            {
                float seed = 0.32;
                float particles = 128.0;
                float res = 16.0;
                float gravity = 0.0;
                float scale = 10;
                float2 offset = float2(0.0, 0.0);
                uv = uv - offset;
                uv = uv * scale;

                float clr = 0.0;
                float timecycle = _Time.y - floor(_Time.y);
                float seed2 = seed + floor(_Time.y);

                float invres = 1.0 / res;
                float invparticles = 1.0 / particles;

                for (float i = 0.0; i < particles; i += 1.0)
                {
                    seed2 += i + tan(seed2);
                    float2 tPos = (float2(cos(seed2), sin(seed2))) * i * invparticles;

                    float2 pPos = float2(0.0, 0.0);
                    pPos.x = ((tPos.x) * timecycle);
                    pPos.y = -gravity * (timecycle * timecycle) + tPos.y * timecycle + pPos.y;

                    // pPos = floor(pPos*res)*invres;

                    float2 p1 = pPos;
                    float4 r1 = float4(float2(step(p1, uv)), 1.0 - float2(step(p1 + invres, uv)));
                    float px1 = r1.x * r1.y * r1.z * r1.w;

                    // glow
                    float px2 = smoothstep(0.0, 200.0, (1.0 / distance(uv, pPos + .015)));
                    px1 = max(px1, px2);

                    clr += px1 * (sin(20.0 + i) + 1.0);
                }

                return clr * (1.0 - timecycle) * float4(1, 0.0, 0.9848619, 1.0);
            }

            float2 LensDistortion(float2 uv)
            {
                return uv * (_LensZoom + _LensDistortion * (uv.x * uv.x + uv.y * uv.y));
            }

            fixed4 ChromaticAberration(float2 uv, fixed4 col)
            {
                uv += 0.5;
                float r = tex2D(_MainTex, float2(uv.x + _ChromaticAberration, uv.y)).r;
                float g = tex2D(_MainTex, float2(uv.x, uv.y + _ChromaticAberration)).r;
                float b = tex2D(_MainTex, float2(uv.x - _ChromaticAberration, uv.y)).r;
                return fixed4(r, g, b, 1) * col;
            }

            fixed4 FilmGrain(float2 uv)
            {
                float x = (uv.x + 4.0 ) * (uv.y + 4.0 ) * (_Time.y * 10.0);
                fixed4 grain = fmod((fmod(x, 13.0) + 1.0) * (fmod(x, 123.0) + 1.0), 0.01) - 0.005;
                return grain * _GrainStrength;
            }

            fixed4 Pixelate(float2 uv)
            {
                int2 Pixelate = int2(_PixelSize,_PixelSize);
				fixed4 col = float4(0, 0, 0, 0);

                float2 PixelSize = 1 / float2(_ScreenParams.x, _ScreenParams.y);
                float2 BlockSize = PixelSize * Pixelate;
                float2 CurrentBlock = float2((floor(uv.x / BlockSize.x) * BlockSize.x), (floor(uv.y / BlockSize.y) * BlockSize.y));
                
                col = tex2D(_MainTex, CurrentBlock + BlockSize / 2);
                col += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 4, BlockSize.y / 4));
                col += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 2, BlockSize.y / 4));
                col += tex2D(_MainTex, CurrentBlock + float2((BlockSize.x / 4) * 3, BlockSize.y / 4));
                col += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 4, BlockSize.y / 2));
                col += tex2D(_MainTex, CurrentBlock + float2((BlockSize.x / 4) * 3, BlockSize.y / 2));
                col += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 4, (BlockSize.y / 4) * 3));
                col += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 2, (BlockSize.y / 4) * 3));
                col += tex2D(_MainTex, CurrentBlock + float2((BlockSize.x / 4) * 3, (BlockSize.y / 4) * 3));
                col /= 9;

                return col;
            }

            fixed4 HueShift(float3 col)
            {
                float3 v = float3(0.57735, 0.57735, 0.57735);
                float3 P = v * dot(v, col);
                float3 U = col - P;
                float3 V = cross(v, U);
                col = U * cos(_HueShift * 6.2832) + V * sin(_HueShift * 6.2832) + P;
                return fixed4(col, 1.0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Background;
                fixed4 prevColor = tex2D(_PrevFrame, float2(i.uv.x, 1 - i.uv.y));
                float2 uv = i.uv - 0.5;

                if (_ChromaticAberration > 0)
                    col = ChromaticAberration(uv, col);

                if (_LensDistortion != 0)
                    uv = LensDistortion(uv);
                
                HitInfo hitInfo = { col, false };
                float2 shadowDir = Rotate(float2(0, -_ShadowOffset), _SunRotation);
                hitInfo = CheckRectangle(uv, hitInfo, shadowDir);
                hitInfo = CheckCircle(uv, hitInfo, shadowDir);

                col = hitInfo.col;
                if (!hitInfo.hit) col = lerp(prevColor, col, _ColorDecay);

                if (_FilmGrain > 0)
                    col += FilmGrain(i.uv);
                
                if (_Pixelate > 0)
                    col *= Pixelate(i.uv);
                
                if (_HueShift > 0)
                    col = HueShift(col.rgb);

                return col;
            }
            ENDCG
        }
    }
}
