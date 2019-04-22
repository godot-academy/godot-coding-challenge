extends Spatial

var SegmentScene = preload("res://Segment.tscn")
var rotate_speed = Vector3(2, 1, 1)

# Called when the node enters the scene tree for the first time.
func _ready():
	set_process(true)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	#Rotate
	var rotate_amount = rotate_speed * delta
	$Segment.rotation += rotate_amount

func _restart():
	
	#Kill the old one
	$Segment.queue_free()
	remove_child($Segment)
	
	#Make and add the new one
	var new_segment = SegmentScene.instance()
	new_segment.inverted = $CheckButton.pressed
	add_child(new_segment, true)

func _on_Button_pressed():
	
	#Tell the segment to subdivide
	$Segment.subdivide()

func _on_Button2_pressed():
	
	#Kill the old one, make a new one
	_restart()


func _on_CheckButton_toggled(button_pressed):
	#Simply restart
	_restart()
