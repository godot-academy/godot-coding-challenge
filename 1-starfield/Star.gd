extends Node2D

var x = 0 #Where this star 'lives'
var y = 0
var z = 0 #'Depth' position. 1 is away, 0 is near

#Where to draw the star based on depth and position
var draw_x = 0
var draw_y = 0

var previous_x = x
var previous_y = y

var speed = 200 #Speed that the star comes towards the camera
onready var window_size = get_viewport_rect().size.x

# Called when the node enters the scene tree for the first time.
func _ready():
	
	#Generate a random starting position
	x = rand_range(-window_size/2, window_size/2)
	y = rand_range(-window_size/2, window_size/2)
	z = rand_range(0, window_size)
	_compute_location()
	
	#Process every frame
	set_process(true)

func _compute_location():
	
	draw_x = (x/z) * window_size + window_size/2
	draw_y = (y/z) * window_size + window_size/2


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	
	#Increase the Z value by the speed and the frame time
	var distance = speed * delta
	z -= distance
	
	#Did it go over size?
	if z < 1:
		#We reset it's position
		z = window_size
		
		draw_x = window_size/2
		draw_y = window_size/2
		_compute_location()
		
	#What is the new position based on this?
	previous_x = draw_x
	previous_y = draw_y
	
	_compute_location()
	
	
	
	#Draw
	update()
	
func _draw():
	
	#Come up with the real position
	
	#We want to draw this as a line from the previous position to the current position
	draw_line(Vector2(previous_x, previous_y), Vector2(draw_x, draw_y), Color(1, 1, 1))