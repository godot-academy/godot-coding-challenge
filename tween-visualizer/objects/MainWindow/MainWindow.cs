using Godot;

public class MainWindow : Control
{
	private Tween _tween;
	private TweenDrawer _tweenDrawer;
	[Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;

	[Export(PropertyHint.Range, "0,5")] public float Speed = 2;
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	[Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Bounce;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tween = GetNode("Tween") as Tween;
		_tweenDrawer = GetNode("TweenDrawer") as TweenDrawer;

		// Interpolate the Progress of the drawer
		_tween.InterpolateProperty(_tweenDrawer, "Progress", 0, 100, Speed, TransitionType, EaseType);
		_tween.Start();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}