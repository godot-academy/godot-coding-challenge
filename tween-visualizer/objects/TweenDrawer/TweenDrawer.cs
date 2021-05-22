using System;
using Godot;

public class TweenDrawer : Control
{
	private Image _brushImage;
	private Texture _brushTexture = ResourceLoader.Load("res://objects/TweenDrawer/brush.png") as Texture;
	private Control _dot;
	private Image _fadeMask;
	private Image _previousImage;

	private float _progress;

	private Tween _tween;
	private Viewport _viewport;
	public float DrawPosition;

	[Export(PropertyHint.Range, "0,100")]
	public float Progress
	{
		get => _progress;
		set
		{
			var adjusted = Math.Min(Math.Max(0, value), 100);
			_progress = adjusted;
			Update();
		}
	}

	[Export] public bool Fade { get; set; } = false;

	[Export] public float FadeSpeed { get; set; } = 0.05f;

	[Export] public float Radius { get; set; } = 3;
	[Export] public Color Color { get; set; } = Colors.Aqua;

	[Export] public Tween.TransitionType TransitionType { get; set; } = Tween.TransitionType.Expo;
	[Export] public Tween.EaseType EaseType { get; set; } = Tween.EaseType.In;

	public override void _Ready()
	{
		_tween = GetNode("Tween") as Tween;
		_dot = GetNode("Dot") as Control;
		_viewport = GetTree().Root.GetViewport();

		//Set up the brush
		_brushImage = _brushTexture.GetData();
	}

	public override void _Draw()
	{
		// Calculate the new position based on the progress
		var currentProgress = Progress / 100;
		_tween.StopAll();
		_tween.InterpolateProperty(this, "DrawPosition", 0, 1, 1, TransitionType, EaseType);
		_tween.Seek(currentProgress);

		// Adjust the position to be the full width of ourselves
		var adjustedPosition = new Vector2(currentProgress, DrawPosition) * RectSize;
		adjustedPosition.y = RectSize.y - adjustedPosition.y;

		//Move our Dot to this position
		_dot.RectPosition = adjustedPosition;

		if (_previousImage is null)
		{
			// Set up the image so we can draw on it
			_previousImage = new Image();
			_previousImage.Create((int) RectSize.x, (int) RectSize.y, false, Image.Format.Rgba8);
			_fadeMask = new Image();

			_fadeMask.Create((int) _previousImage.GetSize().x, (int) _previousImage.GetSize().y, false,
				Image.Format.Rgba8);
			_fadeMask.Fill(new Color(0, 0, 0, 0.1F));
		}

		// Fade the previous image if selected
		if (Fade)
		{
			_previousImage.Lock();
			var imageSize = _previousImage.GetSize();
			for (var x = 0; x < imageSize.x; ++x)
			{
				for (var y = 0; y < imageSize.y; ++y)
				{
					var previousPixel = _previousImage.GetPixel(x, y);
					previousPixel.a -= FadeSpeed;
					_previousImage.SetPixel(x, y, previousPixel);
				}
			}

			_previousImage.Unlock();
		}

		//Draw a circle at the given position
		_previousImage.BlitRect(_brushImage, _brushImage.GetUsedRect(), adjustedPosition);

		// Draw the image
		var imageTexture = new ImageTexture();
		imageTexture.CreateFromImage(_previousImage);
		DrawTexture(imageTexture, Vector2.Zero);
	}
}