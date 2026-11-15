using Godot;
using System;
using System.Collections.Generic;
using System.IO; 
using System.Text.Json;

public partial class graphQuiz : Node2D
{
	[Export] private PackedScene nodeScene;     
	[Export] private PackedScene edgeScene;     
	[Export] private bool isWeighted= true;  
	[Export] private bool isDirected= false; 
	[Export] private int sepValue= 75;
	[Export] private double edgeProb= 0.35;
	[Export] private Label distanceTableLabel;
	[Export] private Label shortestPathLabel;
	[Export] private Panel errorMessagePanel;
	[Export] private Label errorMessageLabel;
	
	[Export] private Button startButton;
	[Export] private Label questionLabel;
	[Export] private LineEdit answerInput;
	[Export] private LineEdit distanceInput;
	[Export] private Panel correctPanel;
	[Export] private Panel incorrectPanel;
	[Export] private Label incorrectMessage;
	
	[Export] private PackedScene userAccess;
	

	public int nodeCount;
	private int nodeId=0;
	private bool creationMode= false;
	public int sourceNode=0;
	public int goalNode=4;
	public int score;

	public List<graphNode> nodeList = new List<graphNode>(); 
	public List<graphEdge> edgeList = new List<graphEdge>(); 
	private int[,] adjMatrix;
	
	private void generateGraph(double edgeProb)
	{
		if((GD.Randi())%2==0)
			isDirected=!isDirected;
		for(int i=0; i < nodeCount; i++)
		{
			createNode(i);
			// gives each node an ID from 0
		}
		for(int i=0;i<nodeList.Count; i++)
		{
			for(int j=0; j<nodeList.Count; j++)
			{
				if(j==i)
				{
					continue;
				}	
				//Edge creation now determined by edgeProb variable
				if(GD.Randi()%100<edgeProb*100)
				{
					//ensuring no edge is created between first and last node 
					if(i==0 &&j==nodeCount-1||j==0 &&i==nodeCount-1)
						continue;
					createEdge(nodeList[i], nodeList[j], isWeighted, isDirected);
					
					//GD.Print(nodeList[i].Number.Text,nodeList[j].Number.Text);
				}
			}
		}
		adjMatrix= formatAdjacencyMatrix(nodeList, edgeList, nodeCount);
	}

	private void createNode(int nodeNumber)
	{
		Vector2 position;
		bool positionValid;
		do 
//each node will need a position so using a do while loop is more efficient.
		{
			position= new Vector2(GD.Randi() % 900+50,GD.Randi() % 900+50); 
//sets position of nodes in a restricted area of pixels
			positionValid= true;
			foreach (graphNode node in nodeList)
//checks if the random position picked is not too near to another node.
			{
				if (node.Position.DistanceTo(position)<sepValue)  
				{
					positionValid=false;
					break;
				}
			}
		} 
		while (!positionValid);
		var newNode=(graphNode)nodeScene.Instantiate();
		newNode.initializeNode(position, nodeNumber, creationMode);
//GD.Print("node id:" nodeNumber+" position: "+position);
		AddChild(newNode);
		nodeList.Add(newNode);
	}

	private void createEdge(graphNode start, graphNode end, bool isWeighted, bool isDirected)
	{
		int weight = 0;
		var newEdge=(graphEdge)edgeScene.Instantiate();
		newEdge.initializeEdge(start, end, isWeighted, isDirected, weight);
		
		//Loop to compare midpoints of each edge ensuring that they are not the same
		foreach(graphEdge item in edgeList)
		{
			if(newEdge.newMidpoint== item.newMidpoint)
			{
				GD.Print(newEdge.newMidpoint);
				GD.Print(item.newMidpoint);
				GD.Print("graph edge already exists");
				return;
			}
		}
		
		AddChild(newEdge);
		edgeList.Add(newEdge);
	}
	
	private void onToggleWeight(bool toggledOn)
	{
		if(!isWeighted)
		{
			isWeighted= true;
			GD.Print("turned on weights");
		}
		else
		{
			isWeighted= false;
			GD.Print("turned off weights");
			
		}
	}
	
	private void onDirectionToggled(bool toggledOn)
	{
		if(!isDirected)
		{
			isDirected= true;
			GD.Print("turned on directions");
		}
		else
		{
			isDirected= false;
			GD.Print("turned off directions");
		}
	}
	
	
	private int[,] formatAdjacencyMatrix(List<graphNode> nodeList, List<graphEdge> edgeList, int nodeCount)
	{
		int[,] adjacencyMatrix = new int[nodeCount, nodeCount];
//creates an empty 2d array representing a square matrix of size n * n, equal to node number
		for(int i=0; i<nodeCount; i++)
		{
			for(int j=0; j<nodeCount;j++)
			{
				if(i==j)
					adjacencyMatrix[i,j]= 0;
// when a calculating a node's distance to itself, the value is 0
				else
				{
					adjacencyMatrix[i,j]=-1;
//If there is no direct connection, the default value -1 is used to show this
				}
			}
		}
		foreach(graphEdge edge in edgeList)
		{
			int start= edge.start.nodeId;
			int end= edge.end.nodeId;
			int weight= edge.weight;
			adjacencyMatrix[start, end]= weight; 
			if (!edge.isDirected)
			{
				adjacencyMatrix[end, start] = weight;
			}
//If the graph is unidrected, then the edge is bidirectional
//the distances between the 2 nodes are the same both ways
			}
		//for(int k=0;k<nodeCount;k++)
		//{
			//string printedRowArray= "[";
			//for(int l=0;l<nodeCount;l++)
			//{
				//printedRowArray= printedRowArray+ Convert.ToString(adjacencyMatrix[k,l])+", ";
			//}
			//printedRowArray= printedRowArray+"]";
			//GD.Print(printedRowArray);
		//}
//Testing code to print the adjacency matrix to the output ^^

		//Stores everything in the 2d array into matrixText which is then used to display the matrix to the user
		string matrixText="";
		for(int i=0;i<nodeCount;i++)
		{
			matrixText= matrixText+"{";
			for(int j=0;j<nodeCount;j++)
			{
				matrixText= matrixText+$"{Convert.ToString(adjacencyMatrix[i,j])}, ";
			}
			matrixText= matrixText+"}\n";
		}
		return adjacencyMatrix;
	}

	private void runDijkstra(int[, ] graph, int source, int goal, int nodeCount, string userAnswer, int userDistance)
	{
		bool[] visited= new bool[nodeCount];
		Array.Fill(visited, false); // No nodes visited yet, so all values are false
		int[] distance= new int[nodeCount];
		 //distance from source
		Array.Fill(distance, int.MaxValue); 
		//sets all values to infinity
		distance[source]= 0;
		 //assigns distance to source node as 0
		int[] previous= new int[nodeCount];
		Array.Fill(previous, -1); 
		//initialize all previous nodes to -1 as they are unvisted
		
		int currentNode= source;
		//First node to be visited is the source node
		while (currentNode!= goal)
		{//loops until it finds the goal
			currentNode= goal;
			for (int i=0; i < nodeCount; i++)
			{
				//Finds the shortest distance from the current node
				if (!visited[i]&& (distance[i] < distance[currentNode]))
				{
					currentNode= i;
				}
			}
			visited[currentNode]= true; 
			//Loops through connected nodes and updates them
			for (int i= 0; i<nodeCount; i++)
			{
				//checks if and edge exists and if the new distance is shorter than the existing
				if (graph[currentNode, i] > 0 && distance[currentNode] + graph[currentNode, i] < distance[i])
				{
					//Update distances
					distance[i]= distance[currentNode]+ graph[currentNode, i];
					//Update previous nodes
					previous[i]= currentNode;
				}
			}
		}
		//Reconstructs the path once finished.
		List<int> path= new List<int>();
		int step= goal;
		while (step!= -1)
		{
			path.Add(step);
			step= previous[step];
		}
		path.Reverse();
		if(distance[goal]== int.MaxValue)
		{
			if(userDistance==-1&&userAnswer==null)
			{
				score++;
				correctPanel.Visible= true;
			}
			else
			{
				gameOver();
			}
			GD.Print("Goal is unreachable");
			return;
		}
		//GD.Print($"Shortest path length: {distance[goal]}");
		//GD.Print($"Path: {string.Join(">", path)}");
		//GD.Print($"Previous: [{string.Join(",", previous)}]");
		//GD.Print($"Distance: [{string.Join(",", distance)}]");
		//GD.Print($"Visited: [{string.Join(",", visited)}]");
		//Lines to test if everything works ^
		formatDistanceTable(distance, previous);
		
		//Shows path and total distance in text format
		shortestPathLabel.Visible= true;
		shortestPathLabel.Text= $"Path: {string.Join(">", path)}\n Total distance: {distance[goal]}";
		for(int i=0;i<path.Count-1;i++)
		{
			int nodeOne=path[i];
			int nodeTwo=path[i+1];
			foreach(graphEdge edge in edgeList)
			{
				if((edge.start.nodeId==nodeOne && edge.end.nodeId==nodeTwo)||(edge.start.nodeId==nodeTwo && edge.end.nodeId==nodeOne))
				{
					edge.DefaultColor= Colors.LightBlue; 
				}
			}
		}
		
		if(userDistance== distance[goal]&&userAnswer==$"{string.Join(">", path)}")
		{
			GD.Print("Correct!");
			score+=2;
			correctPanel.Visible= true;
		}
		else
		{
			GD.Print("Incorrect!");
			gameOver();
		}
	}
	private void exitErrorPressed()
	{
		errorMessagePanel.Visible=false;
	}
	
	private void formatDistanceTable(int[] distance, int[] previous)
	{
		//Adds rows to create a table in the form of one big string
		string distanceTable= "Node Distance Previous";
		for(int i=0;i<distance.Length;i++)
		{
			if(distance[i]==int.MaxValue)
			{
				distanceTable+=$"\n{i}          ∞                 {previous[i]}";
			}
			else
			{
				distanceTable+=$"\n{i}          {distance[i]}                 {previous[i]}";
			}
		}
		distanceTableLabel.Text= distanceTable;
		distanceTableLabel.Visible= true;
	}
	
	private void backButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/startMenu.tscn");
	}
	private void onStartPressed()
	{
		//hides all elements from previous game, resets score and generates a new question
		score=0;
		generateQuestion();
		startButton.Visible= false;
		distanceTableLabel.Visible=false;
		shortestPathLabel.Visible=false;
	}
	
	private void generateQuestion()
	{
		//random integer between 6 and 8 for the nodeCount;
		nodeCount= Convert.ToInt32(GD.Randi() % 3+6);
		generateGraph(edgeProb);
		questionLabel.Text= $"Find the shortest path and distance between: \n {0} and {nodeCount-1}.";
		answerInput.Text="";
		distanceInput.Text="";
	}
	
	private void onAnswerButtonPressed()
	{
		string userAnswer= answerInput.Text;
		int userDistance;
		try
		{
			userDistance=Convert.ToInt32(distanceInput.Text);
		}
		catch
		{
			GD.Print("Invalid distance inputted");
			//error for invalid distance inputted
			errorMessageLabel.Text="Invalid distance inputted";
			errorMessagePanel.Visible= true;
			distanceInput.Text="";
			return;
		}
		adjMatrix= formatAdjacencyMatrix(nodeList, edgeList, nodeCount);
		runDijkstra(adjMatrix, 0, nodeCount-1, nodeCount, userAnswer, userDistance);
	}

	private void nextQuestionPressed()
	{
		//hides previous table and path
		distanceTableLabel.Visible=false;
		shortestPathLabel.Visible=false;
		correctPanel.Visible=false;
		//code from clear subroutine in graph creation module
		for(int i=0;i<nodeList.Count;i++)
		{
			nodeList[i].QueueFree();
		}
		nodeList.Clear();
		nodeCount=0;
		for(int j=0;j<edgeList.Count;j++)
		{
			edgeList[j].QueueFree();
		}
		edgeList.Clear();
		
		generateQuestion();
	}
	private void exitQuizPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/startMenu.tscn");
	}
	
	private string currentUserDataPath= "Databases/currentUserData.json";
	
	public string loadUsername()
	{
		string currentUser= File.ReadAllText(currentUserDataPath);
		return currentUser;
	}
	
	private void gameOver()
	{
		//shows gameover screen
		string username= loadUsername();
		incorrectPanel.Visible=true;
		incorrectMessage.Text= $"Incorrect!\n Game over.\nUsername: {username} \nScore: {score}";
		storeScore(username, score);
	}
	private void noPathPressed()
	{
		adjMatrix= formatAdjacencyMatrix(nodeList, edgeList, nodeCount);
		runDijkstra(adjMatrix, 0, nodeCount-1, nodeCount, null, -1);
	}
	
	private string leaderboardDataPath= "Databases/leaderboardData.json";
	
	private List<Dictionary<string, string>> loadLeaderboard()
	{
		string jsonLeaderboard= File.ReadAllText(leaderboardDataPath);
		return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonLeaderboard);
	}
	
	private void storeScore(string username, int score)
	{
		List<Dictionary<string, string>> leaderboard= loadLeaderboard();
		bool existingScore= false;
		foreach(Dictionary<string, string> kvp in leaderboard)
		{
			if(kvp.ContainsKey(username))
			{
				if(int.Parse(kvp[username])<score)
				{
					//looks for existing scores and if the current score is higher, updates it.
					kvp[username]=Convert.ToString(score);
					break;
				}
				existingScore=true;
			}
		}
		if(!existingScore)
		{
			username= username.Replace("\u0022", "");
			leaderboard.Add(new Dictionary<string, string> {{ username, Convert.ToString(score)}});
		}
		//writes everything back to leaderboard
		string jsonLeaderboard=JsonSerializer.Serialize(leaderboard, new JsonSerializerOptions{WriteIndented= true });
		File.WriteAllText(leaderboardDataPath, jsonLeaderboard);
	}
}


