using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


struct IntRange
{
	public IntRange(int start, int stop, int step)
	{
		Start = start;
		Stop = stop;
		Step = step;
	}

	public int Start { get; }
	public int Stop { get; }
	public int Step { get; }

	public bool IsInRange(int x) => x < Stop && x >= Start && x % Step == 0;
}

[DebuggerDisplay("R={Red} G={Green}, B={Blue}")]
struct IntColor
{
	private static readonly int MaxColor = 255;

	public IntColor(int red, int green, int blue)
	{
		Red = red % MaxColor;
		Green = green % MaxColor;
		Blue = blue % MaxColor;
	}

	public int Red { get; }
	public int Green { get; }
	public int Blue { get; }

	public static bool[,,] BuildUsedColorMap() => new bool[MaxColor, MaxColor, MaxColor];

	public HashSet<IntColor> Neighbors(bool[,,] usedColorMap, int stepWidth = 1, IntRange? redRange = null, IntRange? greenRange = null,
		IntRange? blueRange = null)
	{
		if (redRange is null) redRange = new IntRange(0, MaxColor, 1);
		if (greenRange is null) greenRange = new IntRange(0, MaxColor, 1);
		if (blueRange is null) blueRange = new IntRange(0, MaxColor, 1);

		var neighbors = new HashSet<IntColor>();
		foreach (var redOperation in Enumerable.Range(-stepWidth, stepWidth * 2 + 1))
		{
			var newRed = Red + (redOperation * redRange.Value.Step);
			if (!redRange.Value.IsInRange(newRed)) continue;
			foreach (var greenOperation in Enumerable.Range(-stepWidth, stepWidth * 2 + 1))
			{
				var newGreen = Green + (greenOperation * greenRange.Value.Step);
				if (!greenRange.Value.IsInRange(newGreen)) continue;
				foreach (var blueOperation in Enumerable.Range(-stepWidth, stepWidth * 2 + 1))
				{
					var newBlue = Blue + (blueOperation * blueRange.Value.Step);
					if (!blueRange.Value.IsInRange(newBlue)) continue;
					if (redOperation == 0 && greenOperation == 0 && blueOperation == 0) continue;
					if (usedColorMap[newRed, newGreen, newBlue]) continue;

					// We can finally build a color to mark as a potential neighbor
					neighbors.Add(new IntColor(newRed, newGreen, newBlue));
				}
			}
		}

		return neighbors;
	}

	public override bool Equals(object obj)
	{
		if (obj is not IntColor color) return false;
		return Red == color.Red && Green == color.Green && Blue == color.Blue;
	}

	public override int GetHashCode() => Tuple.Create(Red, Green, Blue).GetHashCode();
}

public struct IntPosition
{
	public readonly int X;
	public readonly int Y;
	private static readonly int[] Range = {-1, 0, 1};

	public IntPosition(int x, int y)
	{
		X = x;
		Y = y;
	}

	public static bool[,] BuildUsedPositionsMap(IntPosition size) => new bool[size.X, size.Y];

	public static IntPosition operator /(IntPosition numerator, int divisor)
	{
		return new(numerator.X / divisor, numerator.Y / divisor);
	}

	public HashSet<IntPosition> Neighbors(bool[,] usedPositions)
	{
		var neighbors = new HashSet<IntPosition>();
		foreach (var xOff in Range)
		{
			var newX = X + xOff;
			if (newX < 0 || newX > usedPositions.GetLength(0)) continue;
			foreach (var yOff in Range)
			{
				var newY = Y + yOff;
				if (newY < 0 || newY > usedPositions.GetLength(1)) continue;
				if (usedPositions[newX,newY]) continue;
				neighbors.Add(new IntPosition(newX, newY));
			}
		}

		return neighbors;
	}


	public override bool Equals(object obj)
	{
		if (obj is not IntPosition pos) return false;
		return X == pos.X && Y == pos.Y;
	}

	public override int GetHashCode() => Tuple.Create(X, Y).GetHashCode();
}

public class RgbWorm : Node2D
{
	public class RgbWormSettings
	{
		public int MaxWorms = 5;
		public bool ShuffleStep = false;
		public bool ShuffleNewPosition = true;
		public bool ShuffleNewColor = true;
	}

	// Collection of all currently used colors
	private bool[,,] _usedColors;
	private bool[,] _usedPositions;
	private Dictionary<IntColor, IntPosition> _currentColors = new Dictionary<IntColor, IntPosition>();
	private Random _rng = new();

	public IntPosition Size = new(800, 600);
	public IntPosition Center => Size / 2;
	private Image _image;
	private ImageTexture _texture = new ImageTexture();
	private RgbWormSettings _settings;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Setup();
	}

	public override void _Process(float delta)
	{
		base._Process(delta);
		Step();
	}

	public void Setup()
	{
		_settings = new RgbWormSettings();
		_rng = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
		_usedColors = IntColor.BuildUsedColorMap();
		_usedPositions = IntPosition.BuildUsedPositionsMap(Size);

		//Create a random starting color
		//TODO: Starting parameters
		var color = new IntColor(_rng.Next(), _rng.Next(), _rng.Next());
		var position = Center;
		_currentColors.Add(color, position);
		
		SetUsed(color, position);

		_image = new Image();
		_image.Create(Size.X, Size.Y, false, Image.Format.Rgb8);
	}

	private void SetUsed(IntColor intColor, IntPosition intPosition)
	{
		_usedColors[intColor.Red, intColor.Green, intColor.Blue] = true;
		_usedPositions[intPosition.X, intPosition.Y] = true;
	}
	
	public void Step()
	{
		// Go over each active head
		_image.Lock();

		var activeEntries = _currentColors.ToList();
		if (_settings.ShuffleStep) ShuffleList(activeEntries);
		foreach (var wormIndex in Enumerable.Range(0, _settings.MaxWorms))
		{
			if (activeEntries.Count == 0) break;
			var entry = activeEntries.First();
			var color = entry.Key;
			var position = entry.Value;
			_currentColors.Remove(color);

			//Draw this one
			var drawColor = new Godot.Color(color.Red / 255f, color.Green / 255f, color.Blue / 255f);
			_image.SetPixel(position.X, position.Y, drawColor);

			//

			//Find all open neighboring spots to put new colors into
			var neighboringColors = color.Neighbors(_usedColors).ToList();
			if (_settings.ShuffleNewColor) ShuffleList(neighboringColors);
			
			var neighboringPositions = position.Neighbors(_usedPositions).ToList();
			if (_settings.ShuffleNewPosition) ShuffleList(neighboringPositions);
			
			foreach (var newPosition in neighboringPositions)
			{
				//Pick a color for the position
				if (neighboringColors.Count == 0) break;
				var newColor = neighboringColors.First();
				neighboringColors.RemoveAt(0);

				SetUsed(newColor, newPosition);
				_currentColors.Add(newColor, newPosition);
			}
		}

		_image.Unlock();
		Update();
	}

	public override void _Draw()
	{
		base._Draw();
		_texture.CreateFromImage(_image);
		DrawTexture(_texture, Vector2.Zero);
	}

	public void ShuffleList<T>(IList<T> list)
	{
		var n = list.Count;
		while (n > 1)
		{
			n--;
			var k = _rng.Next(n + 1);
			var value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}