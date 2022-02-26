Shader "DCQ/CutSpaceEffectEmpty"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CutX ("Cut X(Y1)", float) = 0.0
        _CutXTo ("Cut X To(Y0)", float) = 0.0
        _CutY (" Cut Y", float) = 0.0
        _CutYTo ("Cut Y To", float) = 0.0
        _CutWidth ("Cut Width", float) = 0.0
        _TexW ("Tex W", float) = 0.0
        _FillTex ("Fill Tex", 2D) = "white"
        _FillTexW ("Fill Tex Width", float) = 0.0
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "CanUseSpriteAtlas"="true" "PreviewType"="Plane" }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _CutX;
            float _CutWidth;
            float _CutXTo;

            float _CutY;
            float _CutYTo;

            float _TexW;

            sampler2D _FillTex;
            float _FillTexW;

            float getK()
            {
                float s= _CutX - _CutXTo;
                return (_CutY - _CutYTo) / (s == 0 ? 1 : s);
            }
            float getB(float k)
            {
                return _CutY - (k * _CutX);
            }
            float getX(float y)
            {
                float k = getK();
                float b = getB(k);
                return (k == 0)? 0 : ((y - b) / k);
            }
            float getY(float x)
            {
                float k = getK();
                float b = getB(k);
                return k * x + b;
            }
            float PX(float2 pos)
            {
                float halfW = _CutWidth / 2;
                float xbase = getX(pos.y);
                float mx = xbase + halfW;
                float minx = xbase - halfW;
                float test = pos.x > minx ? (pos.x < mx ? -(1 + pos.x - minx) : 1) : 1;
                return test < 0? test : pos.x;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.x = PX(i.uv);
                fixed4 col = uv.x < 0 ? tex2D(_FillTex, float2((-uv.x - 1) * _TexW / _FillTexW, 0)) : 
                    tex2D(_MainTex, uv);
                clip(uv.x < 0 ? (uv.x >= -1 ? -1 : 1) : 1);
                clip(uv.x > 1 ? -1 : 1); 
                
                clip(col.a - 0.01);
                return col;
            }
            ENDCG
        }
    }
}
