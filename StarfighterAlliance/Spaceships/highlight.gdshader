shader_type canvas_item;

uniform vec4 original_color : source_color  = vec4(0.0);
uniform vec4 swap_color : source_color  = vec4(0.0);

void fragment() {
	COLOR = texture(TEXTURE, UV);
	if (COLOR.a > 0.0)
	{
		if(distance(COLOR, original_color) < 0.1)
		{
			COLOR = swap_color;
		}
	}
}