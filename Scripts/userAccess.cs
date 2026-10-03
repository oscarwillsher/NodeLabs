using Godot;
using System; 
// Default Libraries necessary for running code in Godot
using System.IO; 
using System.Collections.Generic;
using System.Text.Json;
//Added libraries for manipulating JSON files, using dictionaries & reading and writing files

public partial class userAccess : Node
{
	[Export] public LineEdit emailRegBox;
	[Export] public LineEdit usernameRegBox;
	[Export] public LineEdit passwordRegBox;
	[Export] public LineEdit userTagLoginBox;
	[Export] public LineEdit passwordLoginBox;
	[Export] public Panel errorMessage;
	[Export] public Label message;
	
	private string jsonPath= "Databases/userData.json";
	
	public override void _Ready()
	{
		errorMessage.Visible= false;
	}
	
	//Loads data from json file
	public List<Dictionary<string, string>> loadData()
	{
		string userData= File.ReadAllText(jsonPath);
		//reads the data as a string
		return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(userData);
		//returns deserialized data
	}
	
	public bool registerUser(string username, string email, string password)
	{
		
		bool hasNumber=false;
		bool hasSymbol= false;
		bool hasUpper= false;
		bool hasLower= false;
		//boolean values for conditions that need to be fulfilled. 
		
		for(int i = 0; i<password.Length;i++)
		{
			if(Char.IsUpper(password[i]))
				hasUpper= true;
			if(Char.IsLower(password[i]))
				hasLower= true;
			if(Char.IsSymbol(password[i])||Char.IsPunctuation(password[i]))
				hasSymbol= true;
			if(Char.IsNumber(password[i]))
				hasNumber= true;
			//Checks whether the password has all the necessary requirements of a strong password.
		}
		if(!hasNumber||!hasSymbol||!hasUpper||!hasLower||password.Length<6) 
		//checks password length as well as the booleans
		{
			message.Text="Invalid password. \n Password must contain a \n number, symbol, upper \nand lower case letter \n while being 6 characters.";
			errorMessage.Visible= true;
			return false;
		}
		
		try
		{
			int emailLength= email.Length;
			if(email.Substring(emailLength-4, 4)==".com")
			 //Checks the last 4 characters are of a standard .com email
			{
				GD.Print("Contains dot com");
			}
			else
			{
				message.Text="Does not have dot com";
				errorMessage.Visible= true;
				GD.Print(email.Substring(email.Length-4, email.Length-2));
				return false;
			}
		}
		catch(Exception e)
		{
			message.Text="Email is invalid.";
			errorMessage.Visible= true;
			GD.Print(e.Message);
			return false;
		}
		
		bool hasAddress= false;
		for(int j=email.Length-1; j>-1;j--)
		 //starts from the end as it is the more likely place to find the address symbol
		{
			if(email[j]=='@')
			{
				hasAddress= true;
				break;
				 //Breaks as there is no need to check the other characters
			}
		}
		if(hasAddress== false)
			return false;
		
		List<Dictionary<string, string>> userData = loadData();
		foreach(var user in userData)
		{
			if(user["username"] == username)
			{
				message.Text="Username already taken.";
				errorMessage.Visible= true;
				return false;
			}
			else if(user["email"] == email)
			{
				message.Text="Email already taken.";
				errorMessage.Visible= true;
				return false;
			}
			else if(user["password"] == password)
			{
				message.Text=" Password already taken.";
				errorMessage.Visible= true;
				return false;
			}
		//Checks the username, email and password that are being registered with each database value and ensures there are no duplicates.
		}
		userData.Add(new Dictionary<string, string>{{"email", email}, {"username", username}, {"password", password}});
		string userCreds= JsonSerializer.Serialize(userData, new JsonSerializerOptions {WriteIndented= true});
		//Serialization formats the data for the JSON file
		File.WriteAllText(jsonPath, userCreds);
		//Writes the credentials to the JSON
		GD.Print(username + " is now a NodeLabs member!");
		return true;
		
	}
	

	public void logUserIn(string userTag, string password)
	{
		List<Dictionary<string, string>> userData = loadData();
		foreach(var user in userData)
		{
			if((user["username"] == userTag||user["email"]== userTag)&&user["password"]== password)
			{
				GD.Print("Logged in.");
				GetTree().ChangeSceneToFile("res://Scenes/startMenu.tscn");
				return;
			}
			GD.Print("Wrong password or username.");
		}
	}
	
	private void regSubmitButtonPressed()
	{
		string email= emailRegBox.Text;
		string username= usernameRegBox.Text;
		string password= passwordRegBox.Text;
		bool accessToProgram= registerUser(username, email, password);
		if(accessToProgram)
		{
			GD.Print("registered!");
			GetTree().ChangeSceneToFile("res://Scenes/startMenu.tscn");
		}
	}
	
	private void loginSubmitButtonPressed()
	{
		string userTag= userTagLoginBox.Text;
		string  password= passwordLoginBox.Text;
		logUserIn(userTag, password);
	}

	private void onCloseErrorPressed()
	{
		errorMessage.Visible= false;
	}

}
