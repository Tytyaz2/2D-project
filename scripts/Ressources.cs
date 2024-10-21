using Godot;
using System;

public partial class Ressources
{
	public string ItemName { get; private set; }
	public int Quantity { get; private set; }

	public Ressources(string itemName)
	{
		ItemName = itemName;
		Quantity = 0; // Initialement, il n'y a pas de ressources
	}

	public void AddResources(int amount)
	{
		Quantity += amount; // Ajoute des ressources
		GD.Print($"{amount} {ItemName} ajouté. Quantité totale : {Quantity}");
	}

	public int GetQuantity()
	{
		return Quantity; // Retourne la quantité actuelle
	}
}
