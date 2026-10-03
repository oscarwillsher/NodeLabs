using Godot;
using System;

public partial class startMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	private void onGraphGenButtonPressed()
	{
		GD.Print("Working");
		GetTree().ChangeSceneToFile("res://Scenes/graphGen.tscn");
	}
	
	private void onGraphCreateButtonPressed()
	{
		GD.Print("Working");
		GetTree().ChangeSceneToFile("res://Scenes/graphCreation.tscn");
	}
	private void onGraphQuizButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/graphQuiz.tscn");
	}
	private void onQuitButtonPressed()
	{
		GetTree().Quit();
	}
	private void onUserStatsPressed()
	{
		// Replace with function body.
	}
	private void onLeaderboardPressed()
	{
		// Replace with function body.
	}
}
