Shader "Custom/MyRainbow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_EmissionColor("Color", Color) = (1,1,1,1)

    }
    SubShader
    {
		
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
		#include "Shared/ShaderTools.cginc"
        sampler2D _MainTex;
	

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _EmissionColor;
		
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
	

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

		

			half a = IN.uv_MainTex.x;
			if (a > .5f) a = 1 - a;

			half b = IN.uv_MainTex.y;
			if (b > .5f) b = 1 - b;

			//fixed4 hsl = RGBtoHSL(_EmissionColor);
		
		//half t = (_SinTime + 1.0f) / 2.0f;
			half t = _Time;int g = _Time;t -= g;
			

			half h = 2*t +
				(sqrt(a*a+b*b)) *1
			//sqrt(a *b)*8
				;

			fixed4 hsl = fixed4(h, 1, .5f, 1);

			int gh = hsl.x;
			hsl.x -= gh;
	/*	hsl.x /= 4.0f;
			hsl.x += .5f;


			if (hsl.x > .625f) hsl.x = .625f - (hsl.x - .625f);*/

			fixed4 rgb = HSLtoRGB(hsl);



			o.Emission = tex2D(_MainTex, IN.uv_MainTex) * rgb;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
