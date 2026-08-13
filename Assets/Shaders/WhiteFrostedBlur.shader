Shader "UI/WhiteFrostedBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0.0, 10.0)) = 3.0
        _Color ("Overlay Color (White Tint)", Color) = (1, 1, 1, 0.40)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 9-tap Gaussian/Box blur around UV
                float2 texel = _MainTex_TexelSize.xy * _BlurSize;
                
                fixed4 sum = tex2D(_MainTex, i.uv) * 0.204164;
                sum += tex2D(_MainTex, i.uv + float2(-texel.x, -texel.y)) * 0.0924;
                sum += tex2D(_MainTex, i.uv + float2(0, -texel.y)) * 0.123317;
                sum += tex2D(_MainTex, i.uv + float2(texel.x, -texel.y)) * 0.0924;
                sum += tex2D(_MainTex, i.uv + float2(-texel.x, 0)) * 0.123317;
                sum += tex2D(_MainTex, i.uv + float2(texel.x, 0)) * 0.123317;
                sum += tex2D(_MainTex, i.uv + float2(-texel.x, texel.y)) * 0.0924;
                sum += tex2D(_MainTex, i.uv + float2(0, texel.y)) * 0.123317;
                sum += tex2D(_MainTex, i.uv + float2(texel.x, texel.y)) * 0.0924;

                // Blend/campur dengan warna putih frosted (_Color)
                fixed4 result;
                result.rgb = lerp(sum.rgb, _Color.rgb, _Color.a);
                result.a = 1.0;
                return result;
            }
            ENDCG
        }
    }
}
