Shader "Custom/Sprite/UnlitOutline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineSize("Outline Size (px)", Range(0,16)) = 2
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize; // xy = 1/width,1/height
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineSize;
            float _AlphaCutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 texel = _MainTex_TexelSize.xy;   // 1/texWidth, 1/texHeight

                // center pixel
                fixed4 center = tex2D(_MainTex, uv) * _Color * i.color;
                if (center.a > _AlphaCutoff) return center;

                // if center is transparent, sample neighbors to detect edge
                if (_OutlineSize <= 0.0) return 0;

                // 8-directional checks
                float2 dirs[8] = {
                    float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
                    float2(1,1), float2(-1,1), float2(1,-1), float2(-1,-1)
                };

                float2 offset = texel * _OutlineSize;

                for (int k = 0; k < 8; ++k)
                {
                    float2 sampleUV = uv + dirs[k] * offset;
                    fixed4 s = tex2D(_MainTex, sampleUV);
                    if (s.a > _AlphaCutoff)
                    {
                        fixed4 outC = _OutlineColor;
                        // preserve outline alpha from property
                        return outC;
                    }
                }

                return 0;
            }
            ENDCG
        }
    }
}
