using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 150.0f;
	public const float JumpVelocity = -400.0f;

	private AnimatedSprite2D animatedSprite;
	private Ressources woodSlot; // Slot d'inventaire pour le bois
	private Ressources stoneSlot; // Slot d'inventaire pour la pierre
	private bool isFarmingWood = false;
	private bool isFarmingStone = false;
	private bool isWoodFarmingCoroutineRunning = false;
	private bool isStoneFarmingCoroutineRunning = false;
	
	private Label woodLabel;
	private Label StoneLabel;

	public override void _Ready()
	{
		base._Ready();
		AddToGroup("player");
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		// Crée des slots d'inventaire pour le bois et la pierre
		woodSlot = new Ressources("Bois");
		stoneSlot = new Ressources("Pierre");
		
		// Get the WoodLabel node from the scene
		woodLabel = GetNode<Label>("InventoryPanel/slot_bois/WoodLabel");
		StoneLabel = GetNode<Label>("InventoryPanel/slot_pierre/StoneLabel");
		
		woodLabel.Text = "0";
		StoneLabel.Text ="0";

		// Set the initial value of the wood label
		UpdateWoodLabel();
		UpdateStoneLabel();
	}
	private void UpdateWoodLabel()
	{
		if (woodLabel != null)
		{
			woodLabel.Text = woodSlot.GetQuantity().ToString();
		}
		else
		{
			GD.Print("WoodLabel not found!");
		}
	}

	// Updates the stone label to show the current amount of stone
	private void UpdateStoneLabel()
	{
		if (StoneLabel != null)
		{
			StoneLabel.Text = stoneSlot.GetQuantity().ToString();
		}
		else
		{
			GD.Print("StoneLabel not found!");
		}
	}

	// Fonction pour indiquer que le joueur peut farmer du bois
	public void SetIsFarmingWood(bool value)
	{
		isFarmingWood = value;

		if (!isFarmingWood)
		{
			StopFarmingWood(); // Arrête le farming si on sort de la zone
		}
	}

	// Fonction pour indiquer que le joueur peut farmer de la pierre
	public void SetIsFarmingStone(bool value)
	{
		isFarmingStone = value;

		if (!isFarmingStone)
		{
			StopFarmingStone(); // Arrête le farming si on sort de la zone
		}
	}

	// Démarre le farming de bois
	private async void StartFarmingWood()
	{
		if (isWoodFarmingCoroutineRunning) return; // Ne pas démarrer si déjà en cours

		isWoodFarmingCoroutineRunning = true;

		while (isFarmingWood && Input.IsActionPressed("farm"))
		{
			GD.Print("Playing bois_hache animation");
			animatedSprite.Play("bois_hache"); // Joue l'animation de farming de bois
			woodSlot.AddResources(1); // Ajoute du bois
			GD.Print("Farming bois...");
			
			UpdateWoodLabel();

			// Attends 1.5 secondes avant de continuer
			await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		}

		animatedSprite.Play("default"); // Reviens à l'animation par défaut
		isWoodFarmingCoroutineRunning = false;
	}

	// Démarre le farming de pierre
	private async void StartFarmingStone()
	{
		if (isStoneFarmingCoroutineRunning) return; // Ne pas démarrer si déjà en cours

		isStoneFarmingCoroutineRunning = true;

		while (isFarmingStone && Input.IsActionPressed("farm"))
		{
			GD.Print("Playing pierre_pioche animation");
			animatedSprite.Play("pierre_pioche"); // Joue l'animation de farming de pierre
			stoneSlot.AddResources(1); // Ajoute de la pierre
			GD.Print("Farming pierre...");
			
			UpdateWoodLabel();

			// Attends 1.5 secondes avant de continuer
			await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		}

		animatedSprite.Play("default"); // Reviens à l'animation par défaut
		isStoneFarmingCoroutineRunning = false;
	}

	// Arrêter le farming de bois
	private void StopFarmingWood()
	{
		isFarmingWood = false; // Cela arrêtera la coroutine
	}

	// Arrêter le farming de pierre
	private void StopFarmingStone()
	{
		isFarmingStone = false; // Cela arrêtera la coroutine
	}

	// Gestion de la physique et des entrées utilisateur
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
		Vector2 direction = Input.GetVector("left", "right", "ui_up", "ui_down");
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

		// Vérifier si on est dans la zone de farming de bois et appuie sur la touche farm
		if (isFarmingWood && Input.IsActionPressed("farm"))
		{
			StartFarmingWood(); // Commencer à farmer si on est dans la zone et la touche est appuyée
		}

		// Vérifier si on est dans la zone de farming de pierre et appuie sur la touche farm
		if (isFarmingStone && Input.IsActionPressed("farm"))
		{
			StartFarmingStone(); // Commencer à farmer de la pierre si on est dans la zone et la touche est appuyée
		}

		// Arrêter l'animation si on est inactif et au sol
		if (!Input.IsActionPressed("left") && !Input.IsActionPressed("right") &&
			!Input.IsActionPressed("ui_accept") && IsOnFloor() && !Input.IsActionPressed("farm"))
		{
			animatedSprite.Play("default");
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
