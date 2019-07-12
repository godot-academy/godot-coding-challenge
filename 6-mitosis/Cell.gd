extends Node2D

#Cells have a color and a size
const base_radius = 10
var color

#How fast to move (max)
const speed = 50
const child_color_shift = 0.2

# Called when the node enters the scene tree for the first time.
func _ready():
	
	#Do we have a color already?
	if not color:
		#Make a random color
		var hue = randf()
		var saturation = rand_range(0.5, 1.0)
		var value = rand_range(0.5, 1.0)
		color = Color.from_hsv(hue, saturation, value, 0.75)
	

func _draw():
	
	#Draw the circle
	draw_circle(Vector2(0, 0), base_radius, color)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	#Move just a little
	var move_x = rand_range(-speed, speed)
	var move_y = rand_range(-speed, speed)
	var movement = Vector2(move_x, move_y) * delta
	position += movement
	
