extends Node2D

const Cell = preload("res://Cell.tscn")

#How many cells to spawn?
export(int, 0, 10000) var cell_count = 500

#How big is the screen?
onready var screen_size = get_viewport().get_visible_rect().size

# Called when the node enters the scene tree for the first time.
func _ready():
	
	#Create the cells
	for cell in range(cell_count):
		_create_cell()

func _create_cell(previous_cell=null):
	
	#Create a new cell
	var new_cell = Cell.instance()
	
	#Based off a previous one?
	if previous_cell:
		
		#Take position and color
		new_cell.position = previous_cell.position
		new_cell.scale = previous_cell.scale * 0.75
		
		#Give it a new color
		var hue = previous_cell.color.h + rand_range(-previous_cell.child_color_shift, previous_cell.child_color_shift)
		var saturation = previous_cell.color.s + rand_range(-previous_cell.child_color_shift, previous_cell.child_color_shift)
		var value = previous_cell.color.v + rand_range(-previous_cell.child_color_shift, previous_cell.child_color_shift)
		new_cell.color = Color.from_hsv(hue, saturation, value)
		
	else:
		
		#Random position in the screen
		new_cell.position = Vector2(rand_range(0, screen_size.x), rand_range(0, screen_size.y))
		
		#Random size
		var new_scale = rand_range(1, 4)
		new_cell.scale.x = new_scale
		new_cell.scale.y = new_scale
	
	#Listen for any events
	new_cell.connect("input_event", self, "_on_Cell_input_event", [new_cell])
		
	#Add the child
	add_child(new_cell)

func _split(cell):
	
	#We need it to go away
	cell.queue_free()
	
	#Create two children with slightly different colors
	for child in range(2):
		_create_cell(cell)
	
func _on_Cell_input_event(viewport, input_event, shape_index, cell):
	
	#Was the cell clicked?
	if input_event is InputEventMouseMotion and input_event.speed.length_squared() > 0.1:
		_split(cell)
