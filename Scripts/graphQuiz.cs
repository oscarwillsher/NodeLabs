using Godot;
using System;

public partial class graphQuiz : Node2D
{
	private void backButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/startMenu.tscn");
	}
	bool wrong= false;
	private bool generateQuestion()
	{
		return false;
	}
	
}
