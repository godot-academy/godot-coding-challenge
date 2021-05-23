using System;
using System.Linq;
using Godot;

public class TweenDrawer : Control
{
	private Image _brushImage;

	private Color _color = Colors.Aqua;
	private Dot _dot;
	private ImageTexture _imageTexture = new ImageTexture();
	private Image _previousImage;

	private float _progress;

	private Tween _tween;

	// private Texture _brushTexture = ResourceLoader.Load("res://objects/TweenDrawer/brush.png") as Texture;
	[Export(PropertyHint.Range, "1,10")] public int BrushSize = 3;
	public float DrawPosition;
	private float DrawPadding => 0.9f;

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

	[Export] public bool Fade { get; set; } = true;

	// [Export] public float FadeSpeed { get; set; } = 0.05f;
	[Export] public byte FadeSpeed { get; set; } = 1;

	[Export]
	public Color Color
	{
		get => _color;
		set
		{
			_color = value;
			if (_dot != null) _dot.Color = _color;
		}
	}

	[Export] public Tween.TransitionType TransitionType { get; set; } = Tween.TransitionType.Expo;
	[Export] public Tween.EaseType EaseType { get; set; } = Tween.EaseType.In;

	public override void _Ready()
	{
		_tween = GetNode("Tween") as Tween;
		_dot = GetNode("Dot") as Dot;

		//Set up the brush
		_brushImage = new Image();
		var brushData = Enumerable.Repeat<byte>(255, BrushSize * BrushSize * 4).ToArray();
		_brushImage.CreateFromData(BrushSize, BrushSize, false, Image.Format.Rgba8, brushData);
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
		adjustedPosition *= DrawPadding;

		//Move our Dot to this position
		_dot.RectPosition = adjustedPosition;

		if (_previousImage is null)
		{
			// Set up the image so we can draw on it
			_previousImage = new Image();
			_previousImage.Create((int) RectSize.x, (int) RectSize.y, false, Image.Format.Rgba8);
		}

		// Fade the previous image if selected
		if (Fade)
		{
			var imageSize = _previousImage.GetSize();

			// This is slow! We don't wanna do this

			// ReSharper disable once InvalidXmlDocComment
			/**
			_previousImage.Lock();
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
			*/

			var imageData = _previousImage.GetData();

			//Every 4th byte is the alpha (RGBA)
			for (var i = 3; i < imageData.Length; i += 4)
			{
				var data = imageData[i];
				if (data < FadeSpeed) imageData[i] = 0;
				else imageData[i] -= FadeSpeed;
			}

			// _previousImage = new Image();
			_previousImage.CreateFromData((int) imageSize.x, (int) imageSize.y, false, Image.Format.Rgba8, imageData);
		}

		//Draw a circle at the given position
		_previousImage.BlendRect(_brushImage, _brushImage.GetUsedRect(),
			adjustedPosition - _brushImage.GetUsedRect().Size / 2);

		// Draw the image
		_imageTexture.CreateFromImage(_previousImage);
		DrawTexture(_imageTexture, Vector2.Zero, Color);
	}
}