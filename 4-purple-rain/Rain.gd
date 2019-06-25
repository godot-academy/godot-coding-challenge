tool
extends Node2D

#Color of the rain
export(Color) var color = Color(0.54, 0.17, 0.89)
const base_width = 2
const base_height = 15

#Starting position and 'distance' away
export(Vector2) var start_position = Vector2(-10, -30) setget set_start_position
var distance_range = Vector2(0, 100)
export(float, 0, 100) var distance = 10 setget set_distance
export(float) var max_fall = 350

#Variables to determine how fast to go and how big to be
var _gravity = 10
var _gravity_range = Vector2(400, 80) #'Nearest' to 'Furthest'
var _scale = 1
var _scale_range = Vector2(1.5, 0.5) #'Nearest' to 'Furthest
var _speed = 0


func set_start_position(_start_position):
	start_position = _start_position
	_reset()

func set_distance(_distance):
	distance = _distance
	_reset()

# Called when the node enters the scene tree for the first time.
func _ready():
	
	#Process every frame
	_reset()
	set_process(true)
	
func _reset():
	
	#Set to the position
	position = start_position
	
	#Calculate size and speed
	_scale = lerp(_scale_range.x, _scale_range.y, distance/100)
	_speed = 0
	_gravity = lerp(_gravity_range.x, _gravity_range.y, distance/100)
	

func _process(delta):
	
	#Draw
	update()
	
	#Figure out the new speed
	_speed += _gravity*delta
	
	#Update position
	position.y += _speed*delta
	
	#Did we go out of bounds?
	if position.y > max_fall:
		_reset()

func _draw():
	
	#Draw a rectangle
	var size = Vector2(base_width, base_height) * _scale
	var rectangle = Rect2(Vector2(0, 0), size)
	draw_rect(rectangle, color)
	