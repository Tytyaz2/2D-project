using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	private AnimatedSprite2D animatedSprite;
	private Ressources woodSlot; // Slot d'inventaire pour le bois
	private Ressources stoneSlot; // Slot d'inventaire pour la pierre
	private bool isFarmingWood = false;
	private bool isFarmingStone = false;

	public override void _Ready()
	{
		base._Ready();
		AddToGroup("player");
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		// Crée des slots d'inventaire pour le bois et la pierre
		woodSlot = new Ressources("Bois");
		stoneSlot = new Ressources("Pierre");
	}

	public void SetIsFarmingWood(bool value)
	{
		isFarmingWood = value;

		if (isFarmingWood)
		{
			GD.Print("Farming started for: bois");
			StartFarmingWood();
		}
		else
		{
			GD.Print("Farming stopped for: bois");
			StopFarmingWood();
		}
	}

	public void SetIsFarmingStone(bool value)
	{
		isFarmingStone = value;

		if (isFarmingStone)
		{
			GD.Print("Farming started for: pierre");
			StartFarmingStone();
		}
		else
		{
			GD.Print("Farming stopped for: pierre");
			StopFarmingStone();
		}
	}

	private async void StartFarmingWood()
	{
		 GD.Print("Playing bois_hache animation");
		animatedSprite.Play("bois_hache"); // Joue l'animation de farming de bois

		while (isFarmingWood)
		{
			woodSlot.AddResources(1); // Ajoute du bois
			GD.Print("Farming bois...");

			// Attends 1 seconde avant de continuer
			await ToSignal(GetTree().CreateTimer(1.5), "timeout");
		}

		animatedSprite.Play("default"); // Reviens à l'animation par défaut
	}

	private async void StartFarmingStone()
	{
		animatedSprite.Play("pierre_pioche"); // Joue l'animation de farming de pierre

		while (isFarmingStone)
		{
			stoneSlot.AddResources(1); // Ajoute de la pierre
			GD.Print("Farming pierre...");

			// Attends 1 seconde avant de continuer
			await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		}

		animatedSprite.Play("default"); // Reviens à l'animation par défaut
	}

	private void StopFarmingWood()
	{
		isFarmingWood = false; // Cela arrêtera la coroutine
	}

	private void StopFarmingStone()
	{
		isFarmingStone = false; // Cela arrêtera la coroutine
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Ajoute la gravité.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Gérer le saut.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			animatedSprite.Play("jump");
		}

		// Gérer le mouvement.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			animatedSprite.Play("run");
			animatedSprite.FlipH = direction.X > 0;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		if (!Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right") &&
			!Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_down") &&
			!Input.IsActionPressed("ui_accept") && IsOnFloor() && isFarmingWood == false && isFarmingStone ==false)
		{
			animatedSprite.Play("default");
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
