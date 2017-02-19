Shader "Custom/LazerShader" {
	Properties      
	{         
		_MainTex ("RGBA", 2D) = "white" {} 
		_Mask ("RGBA", 2D) = "white" {}      
		_HitCutout ("HitCutout", Range (0, 1) ) = 1              
	}               
	
   SubShader 
   {
   	  Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass 
      {    
 		 Blend SrcAlpha OneMinusSrcAlpha 
 		 ZWrite Off
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         uniform sampler2D _MainTex;    
         uniform sampler2D _Mask;    
         float _HitCutout;
 
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
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            output.col = input.col;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	float2 uv =  input.tex.xy;
         	uv.y -= _Time.y * 2 ;
         	float4 rgbcolor = tex2D(_MainTex, uv); 
         	float alpha = 0;
         	if(input.tex.xy.y <= _HitCutout) {
         		alpha = tex2D(_Mask, input.tex.xy).w;
         	}
            return rgbcolor * input.col * alpha;
         }
 
         ENDCG 
      }
   }
}


