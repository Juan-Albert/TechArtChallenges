Shader "Unlit/Skybox"
{
    Properties
    {
        _TopColor("Top Color", color)=(0,0,0,1)
        _BotColor("Bottom Color",color)=(1,1,1,1)
        _Height("Height", float)=0.5
    }
    SubShader
    {
        Tags { "RenderType"="Skybox" }
        LOD 100

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _TopColor;
            float4 _BotColor;
            float _Height;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col=lerp(_BotColor, _TopColor,saturate(i.uv.y+_Height));

                return col;
            }
            ENDCG
        }
    }
}
