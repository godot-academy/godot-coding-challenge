using Godot;

public class Dot : Control
{
	public Color Color = Colors.Aqua;

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, RectSize.x, Color);
	}
}