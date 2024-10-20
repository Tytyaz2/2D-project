using Godot;
using System;

public partial class FarmPierre : Area2D
{
	public override void _Ready()
	{
		// Connecter les signaux pour savoir quand un corps (joueur) entre ou sort de la zone collante
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
		Connect("body_exited", new Callable(this, nameof(OnBodyExited)));
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Player player)
		{
			player.SetIsFarmingStone(true); // Le joueur entre dans la zone de Farm
		}
	}

	private void OnBodyExited(Node body)
	{
		// Vérifie si le joueur sort de la zone collante
		if (body is Player player)
		{
			player.SetIsFarmingStone(false); // Le joueur quitte la zone collante
		}
	}
}
