extends Node

signal new_score(score)

const Segment = preload("res://SnakeSection.tscn")
const Apple = preload("res://Apple.tscn")

#How big is the game board (origin is top-left)
const BOARD_SIZE = 10 #10x10

#How fast to play the game
export(float, 0, 1000) var start_minimum_frame_time = 500
var minimum_frame_time = start_minimum_frame_time
const score_frame_adjust = 10
var active_frame_time = 0


#Track the head segment
var head_segment = null
var tail_segment = null

#For adding to the tails
var new_tails = 0
var score = 0

#Where to start?
var start_location = Vector2(3, 5)

# Called when the node enters the scene tree for the first time.
func _ready():
	
	#Randomize RNG
	randomize()
	
	#Process per frame
	set_process(true)
	
	#Spawn the head
	head_segment = Segment.instance()
	head_segment.is_head = true
	tail_segment = head_segment
	$Board.add_child(head_segment)
	head_segment.move_to_location(start_location)
	
	#Listen for signals
	head_segment.connect("hit_section", self, "_on_Head_hit_segment")
	head_segment.connect("hit_apple", self, "_on_Head_hit_apple")
	
	#Listen for relevant inputs
	set_process_unhandled_key_input(true)
	
	#No score to start
	emit_signal("new_score", score)
	spawn_apple()

func _process(delta):
	
	#Update the time delta
	active_frame_time += delta * 1000
	
	#Has enough time passed?
	if active_frame_time < minimum_frame_time:
		return
	
	#We can reset the frame time
	active_frame_time -= minimum_frame_time
	
	#Should we be adding a new tail?
	if new_tails:
		new_tails -= 1
		score += 1
		minimum_frame_time -= score_frame_adjust
		emit_signal("new_score", score)
		
		#Make a new tail and fit it in to the segments
		var new_tail = Segment.instance()
		tail_segment.next_segment = new_tail
		new_tail.previous_segment = tail_segment
		new_tail.location = tail_segment.location
		$Board.add_child(new_tail)
		
		#Update the tail pointer
		tail_segment = new_tail
	
	#We can update the location of the head segment
	var next_location = head_segment.location
	var adjustment = Vector2(0, 0)
	match head_segment.direction:
		0:
			adjustment.y = -1
		1:
			adjustment.x = 1
		2:
			adjustment.y = 1
		3:
			adjustment.x = -1
	next_location += adjustment
	
	#Tell it to move
	head_segment.move_to_location(next_location)
	
	#Is the head out of bounds?
	if head_segment.location.x < 0 or head_segment.location.y < 0 or head_segment.location.x >= BOARD_SIZE or head_segment.location.y >= BOARD_SIZE:
		#We're dead
		game_over()

func game_over():
	#We can just reload the scene since we're lazy
	_restart()

func _restart():
	get_tree().reload_current_scene()

func _unhandled_key_input(event):
	
	#Was it the reset action
	if event.is_action_pressed("player_reset"):
		_restart()

func spawn_apple():
	
	#Get a collection of all available spaces
	var spaces = []
	for x in range(BOARD_SIZE):
		for y in range(BOARD_SIZE):
			spaces.append(Vector2(x, y))
	
	#Remove the spaces from the snake
	var segment = head_segment
	while segment:
		var segment_index = spaces.find(segment.location)
		spaces.remove(segment_index)
		segment = segment.next_segment
	
	#Now pick a random spot
	var count = spaces.size()
	var index = randi() % count
	var location = spaces[index]
	location *= 10
	
	#Now make the apple
	var new_apple = Apple.instance()
	new_apple.position = location
	$Board.call_deferred("add_child", new_apple)

func _on_Head_hit_segment(segment):
	#We died
	_restart()

func _on_Head_hit_apple(apple):
	#Kill the apple
	apple.queue_free()
	
	#Add one for next frame
	new_tails += 1
	
	#Make a new apple
	spawn_apple() # We can potentially spawn an apple on the existing head oh well
