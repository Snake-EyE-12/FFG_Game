Shader "Custom/CubeSixSidedFlippableTinted"
{
    Properties
    {
        _TexRight ("Right (+X)", 2D) = "white" {}
        _FlipRightX ("Flip Right X", Float) = 0
        _FlipRightY ("Flip Right Y", Float) = 0
        _ColorRight ("Right Tint", Color) = (1,1,1,1)

        _TexLeft ("Left (-X)", 2D) = "white" {}
        _FlipLeftX ("Flip Left X", Float) = 0
        _FlipLeftY ("Flip Left Y", Float) = 0
        _ColorLeft ("Left Tint", Color) = (1,1,1,1)

        _TexUp ("Up (+Y)", 2D) = "white" {}
        _FlipUpX ("Flip Up X", Float) = 0
        _FlipUpY ("Flip Up Y", Float) = 0
        _ColorUp ("Up Tint", Color) = (1,1,1,1)

        _TexDown ("Down (-Y)", 2D) = "white" {}
        _FlipDownX ("Flip Down X", Float) = 0
        _FlipDownY ("Flip Down Y", Float) = 0
        _ColorDown ("Down Tint", Color) = (1,1,1,1)

        _TexForward ("Forward (+Z)", 2D) = "white" {}
        _FlipForwardX ("Flip Forward X", Float) = 0
        _FlipForwardY ("Flip Forward Y", Float) = 0
        _ColorForward ("Forward Tint", Color) = (1,1,1,1)

        _TexBack ("Back (-Z)", 2D) = "white" {}
        _FlipBackX ("Flip Back X", Float) = 0
        _FlipBackY ("Flip Back Y", Float) = 0
        _ColorBack ("Back Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            sampler2D _TexRight, _TexLeft, _TexUp, _TexDown, _TexForward, _TexBack;
            float _FlipRightX, _FlipRightY;
            float _FlipLeftX, _FlipLeftY;
            float _FlipUpX, _FlipUpY;
            float _FlipDownX, _FlipDownY;
            float _FlipForwardX, _FlipForwardY;
            float _FlipBackX, _FlipBackY;

            float4 _ColorRight, _ColorLeft, _ColorUp, _ColorDown, _ColorForward, _ColorBack;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(UnityObjectToWorldNormal(v.normal));
                o.uv = v.uv;
                return o;
            }

            float2 ApplyFlip(float2 uv, float flipX, float flipY)
            {
                if (flipX > 0.5) uv.x = 1.0 - uv.x;
                if (flipY > 0.5) uv.y = 1.0 - uv.y;
                return uv;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = i.normal;
                float3 an = abs(n);

                float2 uv = i.uv;
                fixed4 col;

                if (an.x > an.y && an.x > an.z)
                {
                    if (n.x > 0)
                        col = tex2D(_TexRight, ApplyFlip(uv, _FlipRightX, _FlipRightY)) * _ColorRight;
                    else
                        col = tex2D(_TexLeft, ApplyFlip(uv, _FlipLeftX, _FlipLeftY)) * _ColorLeft;
                }
                else if (an.y > an.z)
                {
                    if (n.y > 0)
                        col = tex2D(_TexUp, ApplyFlip(uv, _FlipUpX, _FlipUpY)) * _ColorUp;
                    else
                        col = tex2D(_TexDown, ApplyFlip(uv, _FlipDownX, _FlipDownY)) * _ColorDown;
                }
                else
                {
                    if (n.z > 0)
                        col = tex2D(_TexForward, ApplyFlip(uv, _FlipForwardX, _FlipForwardY)) * _ColorForward;
                    else
                        col = tex2D(_TexBack, ApplyFlip(uv, _FlipBackX, _FlipBackY)) * _ColorBack;
                }

                return col;
            }
            ENDHLSL
        }
    }
}
