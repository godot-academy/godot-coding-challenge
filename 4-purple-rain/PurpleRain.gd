tool
extends Node2D

const Rain = preload("res://Rain.tscn")

#How many drops to spawn
export(int, 0, 500) var drops = 200 setget set_drops
var _all_drops = []

func set_drops(_drops):
	drops = _drops
	_reset()

# Called when the node enters the scene tree for the first time.
func _ready():
	_reset()

func _reset():
	
	#Clear out any old drops
	for drop in _all_drops:
		drop.queue_free()
	_all_drops = []
	
	#Make a bunch more drops!
	for drop in range(drops):
		#Make the new object
		var new_drop = Rain.instance()
		
		#Give it a location
		new_drop.distance = rand_range(new_drop.distance_range.x, new_drop.distance_range.y)
		var new_position = Vector2(rand_range(-10, 360), rand_range(-10, -50))
		new_drop.start_position = new_position
		
		#Add the new droplet
		_all_drops.append(new_drop)
		add_child(new_drop)
