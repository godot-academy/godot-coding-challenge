extends Spatial

#How big is this segment?
var current_size = 1.0
onready var multimesh = $MultiMeshInstance.multimesh
var child_transforms = []

# Called when the node enters the scene tree for the first time.
func _ready():
	
	child_transforms.append(Transform())
	_apply_instances()

func _apply_instances():
	
	#Reset the multimesh
	multimesh.instance_count = 0
	
	#Go through all child transforms
	multimesh.instance_count = child_transforms.size()
	for index in range(child_transforms.size()):
		
		#Add it to the multimesh
		
		var child_transform = child_transforms[index]
		multimesh.set_instance_transform(index, child_transform)

func subdivide():
	
	#Make things smaller
	current_size = current_size / 3
	
	#Go through each child
	var new_transforms = []
	for child_transform in child_transforms:
		
		#We want to subdivide that specific one
		var new_child_transforms = subdivide_child_transform(child_transform)
		for new_child_transform in new_child_transforms:
			new_transforms.append(new_child_transform)
	
	#Set up the next set of children
	child_transforms = new_transforms
	_apply_instances()


func subdivide_child_transform(child_transform):
	
	#We want to loop through all three dimensions
	var new_transforms = []
	for x in range(-1, 2):
		for y in range(-1, 2):
			for z in range(-1, 2):
				
				#Is this not a center piece?
				var sum = abs(x) + abs(y) + abs(z)
				if sum > 1:
					#Make a child
					var new_transform = Transform()
					new_transform.origin = child_transform.origin + (Vector3(x, y, z)*current_size)
					new_transform.basis = Basis().scaled(Vector3(1, 1, 1) * current_size)
					new_transforms.append(new_transform)
	
	return new_transforms