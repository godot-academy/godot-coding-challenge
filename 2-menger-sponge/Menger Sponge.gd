extends Spatial

var SegmentScene = preload("res://Segment.tscn")
var rotate_speed = Vector3(2, 1, 1)

# Called when the node enters the scene tree for the first time.
func _ready():
	set_process(true)
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	#Rotate
	var rotate_amount = rotate_speed * delta
	$Segment.rotation += rotate_amount
	pass


func _on_Button_pressed():
	
	#Tell the segment to subdivide
	$Segment.subdivide()
	
	pass # Replace with function body.


func _on_Button2_pressed():
	
	#Kill the old one, make a new one
	$Segment.queue_free()
	remove_child($Segment)
	add_child(SegmentScene.instance(), true)
	pass # Replace with function body.
