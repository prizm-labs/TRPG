Shader "Effects/Mobile/Distortion/Particles" {
Properties {
        _TintColor ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "black" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
		_ColorStrength ("Color Strength", Float) = 1
		_BumpAmt ("Distortion", Float) = 10

		
}

SubShader {
        Tags { "Queue"="Transparent+1" "IgnoreProjector"="True"  "RenderType"="Transperent" }
		//GrabPass {"_GrabTextureMobile"}	
        LOD 200
		ZWrite On
		Cull Back

CGPROGRAM

#pragma surface surf Lambert alpha vertex:vert
#pragma target 3.0
#pragma glsl


sampler2D _MainTex;
sampler2D _BumpMap;

float _BumpAmt;
float _ColorStrength;
sampler2D _GrabTextureMobile;
float4 _GrabTextureMobile_TexelSize;

fixed4 _TintColor;

struct Input {
		float2 uv_MainTex;
        float2 uv_BumpMap;
		float4 proj : TEXCOORD0;
		fixed4 color;
};

void vert (inout appdata_full v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input,o);
	float4 oPos = mul(UNITY_MATRIX_MVP, v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
	#else
		float scale = 1.0;
	#endif
	o.proj.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
	o.proj.zw = oPos.zw;
	o.color = v.color;
}
 
void surf (Input IN, inout SurfaceOutput o) {
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	   
	    half2 offset = o.Normal.rg * _BumpAmt * _GrabTextureMobile_TexelSize.xy;
		IN.proj.xy = offset * IN.proj.z + IN.proj.xy;
		half4 col = tex2Dproj(_GrabTextureMobile, UNITY_PROJ_COORD(IN.proj));

		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
		o.Emission = col.xyz * IN.color + tex*_ColorStrength * _TintColor;
        o.Alpha = _TintColor.a * IN.color.a;
}
ENDCG
}

FallBack "Reflective/Bumped Diffuse"
}