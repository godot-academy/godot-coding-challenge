using Godot;
using Godot.Collections;

public class MultiTweenDrawer : Control
{
	private readonly PackedScene _drawerScene =
		ResourceLoader.Load("res://objects/TweenDrawer/TweenDrawer.tscn") as PackedScene;

	private readonly Dictionary<Tween.EaseType, Color> _easingColors = new Dictionary<Tween.EaseType, Color>
	{
		{Tween.EaseType.In, Colors.Green},
		{Tween.EaseType.Out, Colors.Orange},
		{Tween.EaseType.InOut, Colors.Purple},
		{Tween.EaseType.OutIn, Colors.Pink}
	};

	private Tween.TransitionType _transitionType = Tween.TransitionType.Sine;

	[Export]
	private Tween.TransitionType TransitionType
	{
		get => _transitionType;
		set
		{
			_transitionType = value;
			SetTweenDrawers();
		}
	}

	public float Progress
	{
		get => ((TweenDrawer) GetChild(0)).Progress;
		set
		{
			foreach (var child in GetChildren()) ((TweenDrawer) child).Progress = value;
		}
	}

	public override void _Ready()
	{
		foreach (var easeType in _easingColors)
		{
			var drawer = _drawerScene.Instance() as TweenDrawer;
			AddChild(drawer);
			drawer.EaseType = easeType.Key;
			drawer.Color = easeType.Value;
		}
	}

	private void SetTweenDrawers()
	{
		//Update all of the children to use the selected TransitionType
		foreach (var child in GetChildren())
			if (child is TweenDrawer drawer)
				drawer.TransitionType = TransitionType;
	}
}