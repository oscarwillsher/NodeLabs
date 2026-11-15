using Godot;
using System;

public partial class startMenu : Control
{
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
		GetTree().ChangeSceneToFile("res://Scenes/userLeaderboard.tscn");
	}
	private void logoutPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/userAccess.tscn");
	}
	private void onInstructionsPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/userInstructions.tscn");
	}
}












