Shader "ProjectSynth/Hologram"
{
    Properties
    {
        [HDR] _Color ("Hologram Color", Color) = (0.2, 0.8, 1, 1)
        _BaseAlpha ("Base Alpha", Range(0, 1)) = 0.25

        [Header(Sweep Line)]
        _SweepSpeed ("Sweep Speed (cycles/sec)", Float) = 0.4
        _SweepWidth ("Sweep Width", Float) = 0.15
        _SweepAlpha ("Sweep Extra Alpha", Range(0, 1)) = 0.6
        _YMin ("Model Bottom Y (object space)", Float) = 0
        _YMax ("Model Top Y (object space)", Float) = 2

        [Header(Scanlines)]
        _LineDensity ("Line Density", Float) = 60
        _LineStrength ("Line Strength", Range(0, 1)) = 0.35
        _LineScroll ("Line Scroll Speed", Float) = 0.5

        [Header(Rim)]
        _RimPower ("Rim Power", Range(0.5, 8)) = 3
        _RimStrength ("Rim Strength", Range(0, 2)) = 1

        [Header(Flicker)]
        _FlickerStrength ("Flicker Strength", Range(0, 1)) = 0.08
        _FlickerSpeed ("Flicker Speed", Float) = 20

        [Header(Ridge (back face hull))]
        [Toggle] _RidgeOn ("Enable Ridge", int) = 1
        [Toggle] _RidgeColorShare ("Use Hologram Color", int) = 0
        [HDR] _RidgeColor ("Ridge Color", Color) = (0.4, 1, 1, 1)
        _RidgeWidth ("Ridge Width", Float) = 0.02
        _RidgeBase ("Ridge Base Alpha", Range(0, 1)) = 0.15
        _RidgeGlow ("Ridge Glow Alpha", Range(0, 1)) = 0.9
        _RidgeDensity ("Glow Density (0-1)", Range(0, 1)) = 0.15
        _RidgeCells ("Glow Cells per Unit", Float) = 12
        _RidgeSpeed ("Glow Change Rate (Hz)", Float) = 8

        [Header(Dither)]
        _DitherAmount ("Dither Amount", Range(0, 1)) = 0.5
        _DitherDotSize ("Dither Dot Size (screen px)", Float) = 3

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

        CGINCLUDE
        #include "UnityCG.cginc"

        fixed4 _Color;
        float _BaseAlpha;
        float _SweepSpeed, _SweepWidth, _SweepAlpha, _YMin, _YMax;
        float _LineDensity, _LineStrength, _LineScroll;
        float _RimPower, _RimStrength;
        float _FlickerStrength, _FlickerSpeed;
        fixed4 _RidgeColor;
        float _RidgeWidth, _RidgeBase, _RidgeGlow, _RidgeDensity, _RidgeCells, _RidgeSpeed;
        float _DitherAmount, _DitherDotSize;

        float hash11(float n) { return frac(sin(n) * 43758.5453); }

        float DitherThreshold(float4 svPos)
        {
            const float bayer[16] = { 0,8,2,10, 12,4,14,6, 3,11,1,9, 15,7,13,5 };
            float ps = max(_DitherDotSize, 1.0);
            int x = (int)fmod(floor(svPos.x / ps), 4.0);
            int y = (int)fmod(floor(svPos.y / ps), 4.0);
            return (bayer[y * 4 + x] + 0.5) / 16.0;
        }

        float ApplyDither(float alpha, float4 svPos)
        {
            float th = DitherThreshold(svPos) * _DitherAmount;
            clip(alpha - th);
            return lerp(alpha, saturate(alpha * 1.5 + 0.3), _DitherAmount);
        }
        ENDCG

        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull [_Cull]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.objPos = v.vertex.xyz;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sweep line
                float t = frac(_Time.y * _SweepSpeed);
                float sweepY = lerp(_YMax, _YMin, t);
                float dist = abs(i.objPos.y - sweepY);
                float sweep = 1.0 - smoothstep(0.0, _SweepWidth, dist);
                sweep *= sweep;

                // scan lines
                float lines = sin((i.worldPos.y + _Time.y * _LineScroll) * _LineDensity);
                lines = lines * 0.5 + 0.5;
                float lineMask = lerp(1.0, lines, _LineStrength);

                // rim
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float fres = pow(1.0 - saturate(dot(normalize(i.worldNormal), viewDir)), _RimPower);

                // flickering
                float flicker = 1.0 - _FlickerStrength * hash11(floor(_Time.y * _FlickerSpeed));

                float alpha = _BaseAlpha + fres * _RimStrength * 0.5 + sweep * _SweepAlpha;
                alpha = saturate(alpha * lineMask * flicker);

                alpha = ApplyDither(alpha, i.pos);

                float3 col = _Color.rgb * (1.0 + fres * _RimStrength + sweep * 1.5);
                return fixed4(col, alpha);
            }
            ENDCG
        }

        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            float _RidgeOn, _RidgeColorShare;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float3 p = v.vertex.xyz + normalize(v.normal) * _RidgeWidth * _RidgeOn;
                o.pos = UnityObjectToClipPos(float4(p, 1));
                o.objPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_RidgeOn < 0.5) discard;

                float3 c = floor(i.objPos * _RidgeCells);
                float tick = floor(_Time.y * _RidgeSpeed);
                float r = hash11(dot(c, float3(12.9898, 78.233, 37.719)) + tick * 1.37);
                float glow = step(1.0 - _RidgeDensity, r);

                float alpha = saturate(_RidgeBase + glow * _RidgeGlow);
                alpha = ApplyDither(alpha, i.pos);

                float3 colorToUse = _RidgeColorShare > 0.5 ? _Color.rgb : _RidgeColor.rgb;

                float3 col = colorToUse * (1.0 + glow * 2.0);
                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
}