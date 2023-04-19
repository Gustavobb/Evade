Shader "Unlit/Shapes"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PrevFrame ("Previous Frame", 2D) = "white" {}
        _ColorDecay ("Color Decay", Range(0, 1)) = 0.01
        _FilmGrain ("Film Grain", Range(0, 1)) = 0.0
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
            float _FilmGrain;

            // Circles: x, y, radius, scale and color
            int _CirclesCount;
            float4 _CirclesCenter[100];
            float4 _CirclesData[100];
            float4 _CirclesColor[100];

            // Rectangles: x, y, width, height and color
            int _RectanglesCount;
            float4 _RectanglesCenter[100];
            float4 _RectanglesData[100];
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

            fixed4 FilmGrain(float2 uv)
            {
                float grain = frac(sin(dot(uv, float2(12.9898, 78.233) + _Time.y)) * 43758.5453);
                return fixed4(grain, grain, grain, 1);
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
                float4 col;
                bool hit;
            };

            fixed4 HandleColor(float2 uv, float4 color, int colorType)
            {
                if (colorType == 0) return color;
                if (colorType == 1) return RainbowColor(uv);
                return 0;
            }

            HitInfo CheckCircle(float2 uv, HitInfo hitInfo)
            {
                for (int k = 0; k < _CirclesCount; k++)
                    if (IsPointInCircle(uv, _CirclesCenter[k].xy, _CirclesData[k].x))
                    {
                        HitInfo hitInfo = { HandleColor(uv, _CirclesColor[k], _CirclesData[k].w), true };
                        return hitInfo;
                    }

                return hitInfo;
            }

            HitInfo CheckRectangle(float2 uv, HitInfo hitInfo)
            {
                for (int k = 0; k < _RectanglesCount; k++)
                    if (IsPointInRectangle(uv, _RectanglesCenter[k].xy, _RectanglesData[k].xy, _RectanglesData[k].z))
                    {
                        HitInfo hitInfo = { HandleColor(uv, _RectanglesColor[k], _RectanglesData[k].w), true };
                        return hitInfo;
                    }

                return hitInfo;
            }
            /////// SHAPE CHECKING ///////

            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = RainbowColor(i.uv);
                fixed4 col = 0;
                fixed4 prevColor = tex2D(_PrevFrame, float2(i.uv.x, 1 - i.uv.y));
                float2 uv = i.uv - 0.5;

                HitInfo hitInfo = {col, false};
                hitInfo = CheckCircle(uv, hitInfo);
                hitInfo = CheckRectangle(uv, hitInfo);

                col = hitInfo.col;
                if (_FilmGrain > 0)
                    col *= FilmGrain(i.uv);

                if (!hitInfo.hit) col = lerp(prevColor, col, _ColorDecay);

                return saturate(col);
            }
            ENDCG
        }
    }
}
