using Godot;

public class MainWindow : Control
{
	private Tween _tween;
	private MultiTweenDrawer _tweenDrawer;
	[Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;

	[Export(PropertyHint.Range, "0,5")] public float Speed = 5;
	[Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Linear;

	public override void _Ready()
	{
		_tween = GetNode("Tween") as Tween;
		_tweenDrawer = GetNode("MultiTweenDrawer") as MultiTweenDrawer;

		// Interpolate the Progress of the drawer
		_tween.InterpolateProperty(_tweenDrawer, "Progress", 0, 100, Speed, TransitionType, EaseType);
		_tween.Start();
	}
}