extends Node2D

var Star = preload("res://Star.tscn")

var star_count = 1000

# Called when the node enters the scene tree for the first time.
func _ready():
	
	#Create all the stars
	for star in range(star_count):
		var new_star = Star.instance()
		add_child(new_star)


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
