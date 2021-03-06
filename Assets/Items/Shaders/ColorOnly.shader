﻿Shader "ColorOnly" {
Properties {
    _Color ("Color (RGB)", Color) = (1,1,1,1)
}
SubShader {

	Tags {"Queue"="Transparent" "RenderType"="Transparent"}
	Cull Off

    Pass {
		name "DefaultPass"

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

float4 _Color;

struct v2f {
    float4  pos : SV_POSITION;
};

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    return o;
}


half4 frag (v2f i) : COLOR
{
    return _Color;
}

ENDCG

    }
}


}