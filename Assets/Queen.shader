// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DCQ/Queen"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("Normal Tex", 2D) = "bump" {}
        _NormalMapRect("Normal Tex Rect", Vector) = (0, 0, 1, 1)
        _SrcColor ("Src Color", Color) = (1,0,1)
        _DstColor ("Dst Color", Color) = (1,1,0)
        _BumpScale ("Bump Scale", float) = 1
        _Flash ("Flash", float) = 0
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "CanUseSpriteAtlas"="true" "PreviewType"="Plane" }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _UseNormalMap
            //#pragma surface surf

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0; 
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 lightDir : TEXCOORD0;
                float3 worldVertex : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            struct Input
            {

            };

            fixed4 _SrcColor;
            fixed4 _DstColor;

            /*void surf(Input i, inout SurfaceOutput o)
            {
                o.
            }*/

            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;

                /*#if _UseNormalMap != 0
                o.worldVertex = mul(v.vertex, unity_WorldToObject).xyz;
                

                TANGENT_SPACE_ROTATION;

                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
                #endif*/
                return o;
            }

            sampler2D _MainTex;
            sampler2D _NormalMap;
            float _BumpScale;
            float4 _NormalMapRect;
            float _Flash;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed s = clamp(ceil(abs(col.r - _SrcColor.r) + 
                    abs(col.g - _SrcColor.g) + abs(col.b - _SrcColor.b) +
                    abs(col.a - _SrcColor.a)), 0, 1);
                col = col * s + _DstColor * (1 - s);
                
                /*clip(i.uv.x);
                clip(i.uv.y);
                clip(i.uv.x > 1 ? -1 : 1);
                clip(i.uv.y > 1 ? -1 : 1);*/
                clip(col.a - 0.1);
                col = lerp(col, float4(1,1,1,1), _Flash);
                /*#if _UseNormalMap != 0
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                fixed4 normalColor = tex2D(_NormalMap, 
                    fixed2(i.uv.x * _NormalMapRect.z + _NormalMapRect.x, 
                        i.uv.y * _NormalMapRect.w + _NormalMapRect.y));
                fixed3 tangentNormal = UnpackNormal(normalColor);
                tangentNormal.xy = tangentNormal.xy * _BumpScale;
                tangentNormal = normalize(tangentNormal);

                fixed3 lightDir = normalize(i.lightDir);
                
                fixed3 diffuse = _LightColor0 * max(0, dot(tangentNormal, lightDir)) * col.rgb; 

                fixed3 tempColor = diffuse + ambient * col.rgb;
                return fixed4(tempColor, 1);
                #endif*/
                return col;
            }
            ENDCG
        }
    }
}
