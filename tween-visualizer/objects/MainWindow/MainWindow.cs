using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class MainWindow : Control
{
	private HashSet<Tween.EaseType> _ignoredEaseTypes = new HashSet<Tween.EaseType>();

	private Tween.TransitionType _transitionType = Tween.TransitionType.Sine;

	private Tween _tween;
	private List<MultiTweenDrawer> _tweenDrawers;
	[Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;
	[Export(PropertyHint.Range, "0,5")] public float Speed = 5;

	[Export]
	public Tween.TransitionType TransitionType
	{
		get => _transitionType;
		set
		{
			_transitionType = value;
			UpdateDrawers();
		}
	}

	public override void _Ready()
	{
		_tween = GetNode("Tween") as Tween;

		var drawers = GetTree().GetNodesInGroup("multi_tween_drawer");
		_tweenDrawers = (from object drawer in drawers select drawer as MultiTweenDrawer).ToList();

		//Set up GUI

		//Transition types
		var transitionTypeOption = FindNode("TransitionTypeOption") as OptionButton;
		var transitionTypes = Enum.GetNames(typeof(Tween.TransitionType));
		foreach (var transitionTypeName in transitionTypes)
		{
			transitionTypeOption?.AddItem(transitionTypeName,
				(int) Enum.Parse(typeof(Tween.TransitionType), transitionTypeName));
		}

		//Easing types

		RunTweens();
	}

	private void RunTweens()
	{
		_tween.StopAll();

		// Interpolate the Progress of the drawer
		foreach (var multiTweenDrawer in _tweenDrawers)
			_tween.InterpolateProperty(multiTweenDrawer, "Progress", 0, 100, Speed, TransitionType, EaseType);

		_tween.Start();
	}

	private void UpdateDrawers()
	{
		foreach (var tweenDrawer in _tweenDrawers)
		{
			tweenDrawer.TransitionType = _transitionType;
			tweenDrawer.Clear();
		}
	}

	public void _on_TransitionTypeOption_item_selected(int index)
	{
		TransitionType = (Tween.TransitionType) index;
	}

	public void _on_EASE_toggled(bool enabled, int index)
	{
		foreach (var tweenDrawer in _tweenDrawers)
		{
			tweenDrawer.SetIgnoredEasingType(!enabled, (Tween.EaseType) index);
		}
	}

	public void _on_FadeCheck_toggled(bool enabled)
	{
		foreach (var tweenDrawer in _tweenDrawers)
		{
			tweenDrawer.Fade = enabled;
		}
	}
}