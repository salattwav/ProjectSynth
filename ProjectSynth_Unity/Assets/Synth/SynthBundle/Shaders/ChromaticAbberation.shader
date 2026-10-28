Shader "ProjectSynth/ChromaticAberrationBuiltin"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 0.1)) = 0.02
        _Falloff ("Edge Falloff", Range(0.5, 7)) = 2.0
        _Center ("Center (UV)", Vector) = (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Intensity;
            float _Falloff;
            float4 _Center;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float2 dir = uv - _Center.xy;

                float dist = length(dir) * 1.4142;
                float2 offset = dir * pow(dist, _Falloff) * _Intensity;

                fixed r = tex2D(_MainTex, uv + offset).r;
                fixed4 g = tex2D(_MainTex, uv);
                fixed b = tex2D(_MainTex, uv - offset).b;

                return fixed4(r, g.g, b, g.a);
            }
            ENDCG
        }
    }
}
