﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Textured" {
	Properties      
	{         
		_MainTex ("RGBA", 2D) = "white" {}          
		//_Color ("Main Color", Color) = (1,1,1,1)
	}               
	
   SubShader 
   {
   	  Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass 
      {    
         //Cull Off // since the front is partially transparent, 
            // we shouldn't cull the back
 		 Blend SrcAlpha OneMinusSrcAlpha 
 		 ZWrite Off
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         uniform sampler2D _MainTex;    
         //float4 _Color;
 
         struct vertexInput 
         {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
            float4 col : COLOR;
         };
         
         struct vertexOutput 
         {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
            float4 col : COLOR;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.tex = input.texcoord;
            output.pos = UnityObjectToClipPos(input.vertex);
            output.col = input.col;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	float4 rgbcolor = tex2D(_MainTex, input.tex.xy); 
            return rgbcolor * input.col;
         }
 
         ENDCG
      }
   }
}
