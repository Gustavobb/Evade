Shader "Unlit/Shapes"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PrevFrame ("Previous Frame", 2D) = "white" {}
        _ColorDecay ("Color Decay", Range(0, 1)) = 0.01
        _Background ("Background", Color) = (0, 0, 0, 1)
        _GreyScale ("Grey Scale", Range(0, 1)) = 0.0
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
        _Wobble ("Wobble", Range(0, 1)) = 0.0
        _WobbleFrequency ("Wobble Frequency", Range(0, 10)) = 1.0
        _WobbleAmplitude ("Wobble Amplitude", Range(-5, 5)) = 0.1
        [ShowAsVector2] _WobbleXY ("Wobble XY", Vector) = (0, 0, 0, 0)
        _OldTV ("Old TV", Range(0, 1)) = 0.0
        [ShowAsVector2] _OldTVXY ("Old TV XY", Vector) = (0, 0, 0, 0)
        _ScreenShake ("Screen Shake", Range(0, 1)) = 0.0
        _ScreenShakeFrequency ("Screen Shake Frequency", Range(0, 40)) = 1.0
        _ScreenShakeAmplitude ("Screen Shake Amplitude", Range(0, 10)) = 0.1
        [ShowAsVector2] _ScreenShakeXY ("Screen Shake XY", Vector) = (0, 0, 0, 0)
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
            float _GreyScale;
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
            float _Wobble;
            float _WobbleAmplitude;
            float _WobbleFrequency;
            float2 _WobbleXY;
            float _OldTV;
            float2 _OldTVXY;
            float _ScreenShake;
            float _ScreenShakeFrequency;
            float _ScreenShakeAmplitude;
            float2 _ScreenShakeXY;

            // Circles: x, y, radius, scale and color
            int _CirclesCount;
            float4 _CirclesProperties[100];
            float4 _CirclesExtra[100];
            float4 _CirclesColor[100];
            float _CirclesSortOrder[100];

            // Rectangles: x, y, width, height and color
            int _RectanglesCount;
            float4 _RectanglesProperties[100];
            float4 _RectanglesExtra[100];
            float4 _RectanglesColor[100];
            float _RectanglesSortOrder[100];
            
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
                    case 0.0: break;
                    case 1.0: color = RainbowColor(uv); break;
                    default: color = 0; break;
                }

                color = lerp(0, color, blend);
                return color;
            }

            HitInfo CheckCircle(float2 uv, HitInfo hitInfo, float2 shadowDir)
            {
                float highestSortOrder = -1;
                bool hitShape = false;
                fixed4 oc = hitInfo.col;
                for (int k = 0; k < _CirclesCount; k++)
                {
                    if (_CirclesSortOrder[k] < highestSortOrder)
                        continue;
                    
                    if (IsPointInCircle(uv, _CirclesProperties[k].xy, _CirclesProperties[k].z))
                    {
                        highestSortOrder = _CirclesSortOrder[k];
                        hitInfo.col = HandleColor(uv, _CirclesColor[k], _CirclesExtra[k].x, _CirclesExtra[k].z);
                        hitInfo.col = lerp(oc, hitInfo.col, _CirclesColor[k].a);
                        hitInfo.hit = true;
                        hitShape = true;
                    }

                    if (_CirclesExtra[k].y > 0 && _Shadow > 0 && !hitShape)
                    {
                        float2 shadowCenter = _CirclesProperties[k].xy + shadowDir;
                        if (IsPointInCircle(uv, shadowCenter, _CirclesProperties[k].z))
                        {
                            hitInfo.col = _CirclesExtra[k].x ? 0 : _ShadowColor;
                            hitInfo.col = lerp(oc, hitInfo.col, _CirclesColor[k].a);
                            hitInfo.hit = true;
                        }
                    }
                }

                return hitInfo;
            }

            HitInfo CheckRectangle(float2 uv, HitInfo hitInfo, float2 shadowDir)
            {
                float highestSortOrder = -1;
                bool hitShape = false;
                fixed4 oc = hitInfo.col;
                for (int k = 0; k < _RectanglesCount; k++)
                {
                    if (_RectanglesSortOrder[k] < highestSortOrder)
                        continue;
                    
                    if (IsPointInRectangle(uv, _RectanglesProperties[k].xy, _RectanglesProperties[k].zw, _RectanglesExtra[k].y))
                    {
                        highestSortOrder = _RectanglesSortOrder[k];
                        hitInfo.col = HandleColor(uv, _RectanglesColor[k], _RectanglesExtra[k].x, _RectanglesExtra[k].w);
                        hitInfo.col = lerp(oc, hitInfo.col, _RectanglesColor[k].a);
                        hitInfo.hit = true;
                        hitShape = true;
                    }

                    if (_RectanglesExtra[k].z > 0 && _Shadow > 0 && !hitShape)
                    {
                        float2 shadowCenter = _RectanglesProperties[k].xy + shadowDir;
                        if (IsPointInRectangle(uv, shadowCenter, _RectanglesProperties[k].zw, _RectanglesExtra[k].y))
                        {
                            hitInfo.col = _ShadowColor;
                            hitInfo.col = lerp(oc, hitInfo.col, _RectanglesColor[k].a);
                            hitInfo.hit = true;
                        }
                    }
                }

                return hitInfo;
            }
            /////// SHAPE CHECKING ///////

            ////// EFFECTS //////
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

            float2 ScreenShake(float2 uv)
            {
                float amount = sin(dot(_Time.y, float2(12.9898, 78.233)) * _ScreenShakeFrequency) * _ScreenShakeAmplitude;
                float2 shake = float2(amount * _ScreenShakeXY.x, amount * _ScreenShakeXY.y) * _ScreenShake;
                
                return uv + shake;
            }

            float2 OldTV(float2 uv)
            {
                float hash = frac(sin(dot(uv + _Time.y, float2(12.9898, 78.233))) * 43758.5453);
                if (hash < 0.5) hash = -hash;
                float2 effect = float2(hash * _OldTVXY.x, hash * _OldTVXY.y) * _OldTV;
                return uv + effect;
            }

            float2 Wobble(float2 uv)
            {
                float wsin = sin(dot(uv + _Time.y, float2(12.9898, 78.233)) * _WobbleFrequency) * _WobbleAmplitude;
                float2 effect = float2(wsin * _WobbleXY.x, wsin * _WobbleXY.y) * _Wobble;
                return uv + effect;
            }
            ////// EFFECTS //////

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Background;
                fixed4 prevColor = tex2D(_PrevFrame, float2(i.uv.x, 1 - i.uv.y));
                float2 uv = i.uv - 0.5;
                uv.y += .02;

                if (_Wobble > 0)
                    uv = Wobble(uv);
                
                if (_OldTV > 0)
                    uv = OldTV(uv);
                
                if (_ScreenShake > 0)
                    uv = ScreenShake(uv);

                if (_ChromaticAberration > 0)
                    col = ChromaticAberration(uv, col);

                if (_LensDistortion != 0)
                    uv = LensDistortion(uv);
                
                HitInfo hitInfo = { col, false };
                float2 shadowDir = Rotate(float2(0, -_ShadowOffset), _SunRotation);
                hitInfo = CheckCircle(uv, hitInfo, shadowDir);
                hitInfo = CheckRectangle(uv, hitInfo, shadowDir);

                col = hitInfo.col;
                if (!hitInfo.hit) col = lerp(prevColor, col, _ColorDecay);

                if (_FilmGrain > 0)
                    col += FilmGrain(i.uv);
                
                if (_Pixelate > 0)
                    col *= Pixelate(i.uv);
                
                if (_HueShift > 0)
                    col = HueShift(col.rgb);
                
                if (_GreyScale > 0)
                {
                    float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                    col = lerp(col, grey, _GreyScale);
                    col.a = 1;
                }

                return col;
            }
            ENDCG
        }
    }
}
