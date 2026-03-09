Shader "Custom/LavaSurfaceShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        _HeightLevel ("Height Level", Range(0, 10)) = 5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            // Based on nimitz's "Noise animation - Lava" shader: https://www.shadertoy.com/view/lslXRS

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _NoiseTexture_ST;
                float _HeightLevel;
            CBUFFER_END

            float hash21(float2 n)
            {
                return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
            }

            float2x2 makem2(float theta)
            {
                float c = cos(theta);
                float s = sin(theta);
                // HLSL matrices are row-major by default: float2x2(m00, m01, m10, m11)
                return float2x2(c, -s, s, c);
            }

            float noise(float2 x)
            {
                return _NoiseTexture.SampleLevel(sampler_NoiseTexture, (_NoiseTexture_ST.xy * x) * 0.01, 0).x;
            }

            float2 gradn(float2 p)
            {
                float ep = 0.09;
                float gradx = noise(float2(p.x + ep, p.y)) - noise(float2(p.x - ep, p.y));
                float grady = noise(float2(p.x, p.y + ep)) - noise(float2(p.x, p.y - ep));
                return float2(gradx, grady);
            }

            float flow(float2 p)
            {
                float z = 2.0;
                float rz = 0.0;
                float2 bp = p;

                [unroll] // Optional: explicitly unroll or loop
                for (float i = 1.0; i < 7.0; i++)
                {
                    // Primary flow speed
                    p += _Time.y * 0.6;

                    // Secondary flow speed
                    bp += _Time.y * 1.9;

                    // Displacement field
                    float2 gr = gradn(i * p * 0.34 + _Time.y * 1.0);

                    // Rotation of the displacement field
                    // Note: HLSL mul(vector, matrix) vs mul(matrix, vector) matters
                    gr = mul(gr, makem2(_Time.y * 6.0 - (0.05 * p.x + 0.03 * p.y) * 40.0));

                    // Displace the system
                    p += gr * 0.5;

                    // Add noise octave
                    rz += (sin(noise(p) * 7.0) * 0.5 + 0.5) / z;

                    // Blend factor
                    p = lerp(bp, p, 0.77);

                    // Scaling
                    z *= 1.4;
                    p *= 2.0;
                    bp *= 1.9;
                }
                return rz;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }


            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 pos = IN.positionOS.xyz;

                float2 uv;
                Unity_TilingAndOffset_float(IN.uv, float2(1.0f, 1.0f), float2(_Time.y, _Time.y), uv);

                pos.y += flow(uv * 0.5) * _HeightLevel;

                OUT.positionHCS = TransformObjectToHClip(pos);
                OUT.uv = uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float rz = flow(IN.uv * 0.5);

                half3 col = (_BaseColor / rz).rgb;
                col = pow(abs(col), half(1.4));
                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}