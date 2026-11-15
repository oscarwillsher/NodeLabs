using Godot;
using System;

public partial class userInstructions : Node
{
	private void backButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/startMenu.tscn");
	}
}
