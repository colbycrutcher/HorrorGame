Shader "Custom/DashLine"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Speed("Speed", float) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			sampler2D		_MainTex;
			uniform float4	_Color;

			fixed _Speed;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float2 texCoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float2 texCoord : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				output.pos = UnityObjectToClipPos(input.vertex);
				output.texCoord = input.texCoord + float2(_Time.x * -_Speed, 0);

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return tex2D(_MainTex, input.texCoord.xy) * _Color;
			}

			ENDCG
		}
	}
}