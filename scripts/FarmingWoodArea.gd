extends Area2D

signal material_farmed

func _ready():
	connect("body_entered", self, "_on_Area2D_body_entered")
	connect("body_exited", self, "_on_Area2D_body_exited")

func _on_Area2D_body_entered(body):
	GD.Print("Un corps est entré : ", body.name)  
	if body.is_in_group("player"):
		GD.Print("Le joueur a été détecté.")
		body.start_farming()

func _on_Area2D_body_exited(body):
	GD.Print("Un corps est sorti : ", body.name)  
	if body.is_in_group("player"):
		body.stop_farming()
