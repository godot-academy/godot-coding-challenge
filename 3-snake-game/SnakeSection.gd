extends Area2D

signal hit_apple(apple)
signal hit_section(section)

#How big is the grid scale?
const GRID_SCALE = 10 #(10x10 per square)

#Is this the head?
var is_head = false

#What direction is it facing?
enum DIRECTION {
	UP,
	RIGHT,
	DOWN,
	LEFT
}
var direction = DIRECTION.RIGHT

#Does it have any segments after it or before?
var previous_segment = null
var next_segment = null

#Where is this segment?
var location = Vector2(0, 0)

# Called when the node enters the scene tree for the first time.
func _ready():
	
	# Process unhandled input events
	set_process_unhandled_key_input(true)
	
func _unhandled_key_input(event):
	
	#Is it not even the head?
	if not is_head:
		# Shouldn't even be listening for input events
		set_process_unhandled_key_input(false)
		return
	
	#Which key was hit?
	if event.is_action_pressed("player_up"):
		direction = DIRECTION.UP
	elif event.is_action_pressed("player_right"):
		direction = DIRECTION.RIGHT
	elif event.is_action_pressed("player_down"):
		direction = DIRECTION.DOWN
	elif event.is_action_pressed("player_left"):
		direction = DIRECTION.LEFT

func _get_last_segment():
	#Does it have a segment after this?
	if next_segment:
		return next_segment._get_last_segment()
	else:
		return self

func _get_first_segment():
	#Should just give the head but we'll have this function anyways
	if previous_segment:
		return previous_segment._get_first_segment()
	else:
		return self

func move_to_location(_location):
	
	#Are we the head?
	if not is_head:
		#We can't move directly
		return
	
	#Update everything else's position
	if next_segment:
		next_segment.move_to_previous()
	
	#Update our position
	location = _location
	var final_location = location*GRID_SCALE
	position = final_location

func move_to_previous():
	#We want to move everyone after us first
	if next_segment:
		next_segment.move_to_previous()
	
	#Now we copy our parent's location
	if previous_segment:
		location.x = previous_segment.location.x
		location.y = previous_segment.location.y
	
	#Finally, update our transform
	var final_location = location*GRID_SCALE
	position = final_location

func _on_SnakeSection_area_entered(area):
	
	#Was it an apple or another segment?
	if area.is_in_group("section"):
		#We hit ourselves!
		emit_signal("hit_section", area)
		
	elif area.is_in_group("apple"):
		#Yummy apple!
		emit_signal("hit_apple", area)
