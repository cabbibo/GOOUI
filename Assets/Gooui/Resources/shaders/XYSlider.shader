Shader "Custom/XYSlider" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_SliderColor ("SliderColor", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ButtonTexture ("ButtonTexture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_NormalMap( "Normal Map" , 2D ) = "white" {}
    _CubeMap( "Cube Map" , Cube ) = "white" {}
    _BumpMap ("Bumpmap", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		//#include "Unity.cginc"

		uniform sampler2D _NormalMap;
    uniform sampler2D _BumpMap;
		uniform sampler2D _ButtonTexture;
    uniform samplerCUBE _CubeMap;

    uniform float3 _HandL;
    uniform float3 _HandR;

    uniform float _TriggerVal;
    uniform float _ToggleVal;

    uniform float _SliderVal;
    uniform float _DownVal;
    uniform float3 _Debug;


    uniform float _xVal;
    uniform float _yVal;
    sampler2D _MainTex;
    //float4 _MainTex_ST;

		struct Input {
			float2 uv_MainTex;
			float3 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _SliderColor;

		#include "chunks/buttonVertStuff.cginc"
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

      float textureAlpha = tex2D (_ButtonTexture, IN.uv_MainTex * 1.5 - .25).a;

			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex));

			///float2 newUV = TRANSFORM_TEX(IN.uv_MainTex, _MainTex);

			float3 fCol = c.xyz;


      bool b = IN.uv_MainTex.y < .95 && IN.uv_MainTex.y > .05 && IN.uv_MainTex.x < .95 && IN.uv_MainTex.x > .05 && textureAlpha > .5;
    


  		if( b ){

  			float tVal = .3 + _DownVal * 2;
  			fCol = float3( tVal , tVal , tVal ) * c.xyz;
  		}else{
  			fCol = float3( 1 , 1 , 1 ) * c.xyz;
  		}

      if( length( IN.uv_MainTex - float2( 1-_xVal , 1-_yVal ) ) < .05 ){
        fCol = _SliderColor * 1;
      }

      if( abs( IN.uv_MainTex.x - ( 1-_xVal)  ) < .01 ){
        fCol = _SliderColor * 1;
      }
      if( abs( IN.uv_MainTex.y - ( 1-_yVal)  ) < .01 ){
        fCol = _SliderColor * 1;
      }

  		//fCol *= .1 + 100 * _ToggleVal;
      //if( !b ){ fCol = fCol - 2*fCol *_ToggleVal;}

                
			o.Albedo = fCol;

			//clip( IN.color.x - .5 );
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
