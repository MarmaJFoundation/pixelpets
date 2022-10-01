// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/GlowRank"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _PlasmaLightFX_Fade_1("_PlasmaLightFX_Fade_1", Range(0, 1)) = 0.2
        _PlasmaLightFX_Speed_1("_PlasmaLightFX_Speed_1", Range(0, 1)) = 0.5
        _PlasmaLightFX_BW_1("_PlasmaLightFX_BW_1", Range(0, 1)) = 1
        _Color("Tint", Color) = (1,1,1,1)
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(HAS_OUTLINE)] _UseOutline("Use Outline", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]

            Pass
            {
                Name "Default"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ HAS_OUTLINE
                #pragma multi_compile_local _ HAS_GRAYSCALE

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                fixed4 _Color;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                    OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                    OUT.color = v.color;// * _Color;
                    return OUT;
                }
                inline float RBFXmod2(float x, float modu)
                {
                    return x - floor(x * (1.0 / modu)) * modu;
                }

                float3 RBFXrainbow2(float t)
                {
                    t = RBFXmod2(t, 1.0);
                    float tx = t * 8;
                    float r = clamp(tx - 4.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);
                    float g = tx < 2.0 ? clamp(tx, 0.0, 1.0) : clamp(4.0 - tx, 0.0, 1.0);
                    float b = tx < 4.0 ? clamp(tx - 2.0, 0.0, 1.0) : clamp(6.0 - tx, 0.0, 1.0);
                    return float3(r, g, b);
                }

                float4 PlasmaLight(float4 txt, float2 uv, float _Fade, float speed, float bw)
                {
                    float _TimeX = _Time.y * speed;
                    float a = 1.1 + _TimeX * 2.25;
                    float b = 0.5 + _TimeX * 1.77;
                    float c = 8.4 + _TimeX * 1.58;
                    float d = 610 + _TimeX * 2.03;
                    float x1 = 2.0 * uv.x;
                    float n = sin(a + x1) + sin(b - x1) + sin(c + 2.0 * uv.y) + sin(d + 5.0 * uv.y);
                    n = RBFXmod2(((5.0 + n) / 5.0), 1.0);
                    float4 nx = txt;
                    n += nx.r * 0.2 + nx.g * 0.4 + nx.b * 0.2;
                    float4 ret = float4(RBFXrainbow2(n), txt.a);
                    ret = lerp(ret, dot(ret.rgb, 1), bw);
                    ret = lerp(txt, txt + ret, _Fade);
                    ret.a = txt.a;
                    return ret;
                }
                float4 _MainTex_TexelSize;
                float _PlasmaLightFX_Fade_1;
                float _PlasmaLightFX_Speed_1;
                float _PlasmaLightFX_BW_1;
                fixed4 frag(v2f IN) : SV_Target
                {
                    half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                    #ifdef HAS_OUTLINE
                    if (color.a == 0)
                    {
                        float totalAlpha = 0;

                        [unroll(16)]
                        for (int i = 1; i < 2; i++) {
                            fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
                            fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, i * _MainTex_TexelSize.y));
                            fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
                            fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

                            totalAlpha += pixelUp.a + pixelDown.a + pixelRight.a + pixelLeft.a;
                        }

                        if (totalAlpha > 0) {
                            color.rgba = fixed4(1, 1, 1, 1);
                        }
                    }
                    #endif
                    color = PlasmaLight(color, IN.texcoord, _PlasmaLightFX_Fade_1, _PlasmaLightFX_Speed_1, _PlasmaLightFX_BW_1);
                    #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                    #endif
                    clip(color.a - 0.001);

                    return color;
                }
            ENDCG
            }
        }
}
