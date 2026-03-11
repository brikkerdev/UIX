// UIX Framework - Rounded rectangle shader (border-radius)
// Built-in Unlit, SDF-based rounded corners

Shader "UIX/RoundedRect"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0, 0.5)) = 0.1
        _BorderWidth ("Border Width", Range(0, 0.2)) = 0
        _BorderColor ("Border Color", Color) = (0,0,0,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

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

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            float _Radius;
            float _BorderWidth;
            fixed4 _BorderColor;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.uv = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            // SDF for rounded rect - uv in [0,1], radius in normalized [0, 0.5]
            float roundedBoxSDF(float2 uv, float r)
            {
                float2 p = uv - 0.5;
                float2 b = 0.5 - r;
                float2 q = abs(p) - b + r;
                return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float d = roundedBoxSDF(uv, _Radius);
                float alpha = 1.0 - smoothstep(-0.01, 0.01, d);

                half4 color = IN.color;
                color.a *= alpha;

                // Border
                if (_BorderWidth > 0)
                {
                    float borderOuter = roundedBoxSDF(uv, _Radius);
                    float borderInner = roundedBoxSDF(uv, max(0.001, _Radius - _BorderWidth));
                    float borderAlpha = smoothstep(-0.01, 0.01, borderOuter) * (1.0 - smoothstep(-0.01, 0.01, borderInner));
                    color = lerp(color, _BorderColor * IN.color, borderAlpha);
                }

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
