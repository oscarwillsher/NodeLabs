using Godot;
using System; 
// Default Libraries necessary for running code in Godot
using System.IO; 
using System.Collections.Generic;
using System.Text.Json;
//Added libraries for manipulating JSON files, using dictionaries & reading and writing files

public partial class userLeaderboard : Node
{
	private string userDataJsonPath= "Databases/userData.json";
	private string leaderboardJsonPath= "Databases/leaderboardData.json";
	
	public List<Dictionary<string, string>> loadData()
	{
		string userData= File.ReadAllText(userDataJsonPath);
		//reads the data as a string
		return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(userData);
		//returns deserialized data
	}
	
	
}
