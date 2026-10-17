Shader "ProjectSynth/CircleGlowOverlayEffect"
{
    Properties
    {
        _Intensity ("Intensity", Range(0, 1)) = 1.0
        _MainColor ("Main Color", Color) = (1.0, 1.0, 1.0)
        _ScrollTexture ("Scroll Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 3.0
        _BaseColor ("Base Color", Color) = (0.1, 0.5, 0.9, 1.0)
        _BaseColorIntensity ("Base Color Intensity", Range(0, 5)) = 2.0
        _AccentColor ("Accent Color", Color) = (0.4, 0.6, 0.6, 1.0)
        _AccentColorIntensity ("Accent Color Intensity", Range(0, 5)) = 0.5
        _PatternIntensity ("Pattern Intensity", Float) = 3.0
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _RadiusX ("Radius X", Float) = 0.8
        _RadiusY ("Radius Y", Float) = 0.4
        _EdgeWidth ("Edge Width", Float) = 0.7
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Intensity;
            float3 _MainColor;
            sampler2D _ScrollTexture;
            float4 _ScrollTexture_ST;
            float _ScrollSpeed;
            float4 _BaseColor;
            float _BaseColorIntensity;
            float4 _AccentColor;
            float _AccentColorIntensity;
            float _PatternIntensity;
            float _PulseSpeed;
            float _RadiusX;
            float _RadiusY;
            float _EdgeWidth;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 computeScrollLayer(float2 uv)
            {
                // Fold the screen into one mirrored quadrant
                float2 local = 1.0 - abs(1.0 - 2.0 * uv);

                float2 scrollOffset = _Time.y * _ScrollSpeed * 0.01;

                // No frac: Repeat wrap mode does the wrapping in the sampler
                float2 sampleUV = TRANSFORM_TEX(local - scrollOffset, _ScrollTexture);

                float4 scrollLayer = tex2D(_ScrollTexture, sampleUV);

                return _MainColor * scrollLayer.a;
            }

            float3 computePatternColor(float2 fragCoord)
            {
                float2 uv = fragCoord / _ScreenParams.xy;

                float pulse = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed);

                float3 baseContribution = _BaseColor * _BaseColorIntensity;
                float3 accentContribution = _AccentColor * _PatternIntensity * pulse * _AccentColorIntensity;

                float3 scrollLayer = computeScrollLayer(uv);

                return baseContribution + accentContribution + scrollLayer;
            }

            float computeEllipseMask(float2 screenUv)
            {
                float2 centered = (screenUv - 0.5) * float2(_ScreenParams.x / _ScreenParams.y, 1.0);
                float dist = length(centered / float2(_RadiusX, _RadiusY));
                return smoothstep(1.0, 1.0 + _EdgeWidth, dist);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 fragCoord = i.uv * _ScreenParams.xy;

                float3 patternColor = computePatternColor(fragCoord);
                float3 combinedPattern = patternColor;

                float mask = computeEllipseMask(i.uv);

                fixed4 sceneColor = tex2D(_MainTex, i.uv);
                float3 finalColor = lerp(sceneColor.rgb, combinedPattern, mask * _Intensity);

                return fixed4(finalColor, sceneColor.a);
            }
            ENDCG
        }
    }
}
