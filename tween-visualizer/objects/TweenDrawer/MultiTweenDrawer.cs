using System.Collections.Generic;
using Godot;

public class MultiTweenDrawer : Control
{
	private readonly PackedScene _drawerScene =
		ResourceLoader.Load("res://objects/TweenDrawer/TweenDrawer.tscn") as PackedScene;

	private readonly Godot.Collections.Dictionary<Tween.EaseType, Color> _easingColors =
		new Godot.Collections.Dictionary<Tween.EaseType, Color>
		{
			{Tween.EaseType.In, Colors.Green},
			{Tween.EaseType.Out, Colors.Orange},
			{Tween.EaseType.InOut, Colors.Purple},
			{Tween.EaseType.OutIn, Colors.Pink}
		};

	private readonly HashSet<Tween.EaseType> _ignoredEasingTypes = new HashSet<Tween.EaseType>();

	Tween.TransitionType _transitionType = Tween.TransitionType.Sine;

	[Export]
	public Tween.TransitionType TransitionType
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

	public bool Fade
	{
		set
		{
			foreach (var child in GetChildren())
			{
				var node = child as TweenDrawer;
				node.Fade = value;
			}
		}
	}

	public override void _Ready()
	{
		ResetDrawers();
	}

	private void SetTweenDrawers()
	{
		//Update all of the children to use the selected TransitionType
		foreach (var child in GetChildren())
			if (child is TweenDrawer drawer)
				drawer.TransitionType = TransitionType;
	}

	public void Clear()
	{
		foreach (var child in GetChildren())
			if (child is TweenDrawer drawer)
				drawer.Clear();
	}

	public void ResetDrawers()
	{
		foreach (var child in GetChildren())
		{
			var node = child as Node;
			RemoveChild(node);
			node.QueueFree();
		}

		foreach (var easeType in _easingColors)
		{
			if (_ignoredEasingTypes.Contains(easeType.Key)) continue;

			var drawer = _drawerScene.Instance() as TweenDrawer;
			AddChild(drawer);
			drawer.EaseType = easeType.Key;
			drawer.Color = easeType.Value;
		}
	}

	public void SetIgnoredEasingType(bool isIgnored, Tween.EaseType easeType)
	{
		if (isIgnored) _ignoredEasingTypes.Add(easeType);
		else _ignoredEasingTypes.Remove(easeType);
		ResetDrawers();
	}
}