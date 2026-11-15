using Godot;
using System; 
// Default Libraries necessary for running code in Godot
using System.IO; 
using System.Collections.Generic;
using System.Text.Json;
//Added libraries for manipulating JSON files, using dictionaries & reading and writing files

public partial class userLeaderboard : Node
{
	[Export] Label leaderboardLabel;
	
	private string leaderboardDataPath= "Databases/leaderboardData.json";
	
	private List<Dictionary<string, string>> loadLeaderboard()
	{
		string jsonLeaderboard= File.ReadAllText(leaderboardDataPath);
		return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonLeaderboard);
	}
	
	private void displayLeaderboard()
	{
		List<Dictionary<string, string>> leaderboard= loadLeaderboard();
		//converts it to a list of kvps to compare instead of list of dictionaries
		List<KeyValuePair<string, string>> kvpLeaderboard= new List<KeyValuePair<string, string>>();
		foreach (var entry in leaderboard)
		{
			foreach (var userScore in entry)
			{
				kvpLeaderboard.Add(userScore);
			}
		}
		kvpLeaderboard.Sort((entry1, entry2)=>
		{
			int score1 = int.Parse(entry1.Value);  
			int score2 = int.Parse(entry2.Value);  
			return score2.CompareTo(score1);
			//compares values in desc order
		});
		string leaderboardText= "Leaderboard:\n";
		for(int i=0;i<leaderboard.Count;i++)
		{
			//makes leaderboard
			leaderboardText+= $"Position: {i+1}  \nUsername: {kvpLeaderboard[i].Key} \nScore: {kvpLeaderboard[i].Value}\n\n\n";
		}
		//displays leaderboard
		leaderboardLabel.Text=leaderboardText;
	}
	public override void _Ready()
	{
		displayLeaderboard();
	}
	private void backButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/startMenu.tscn");
	}
}
