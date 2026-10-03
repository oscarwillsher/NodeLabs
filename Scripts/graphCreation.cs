using Godot;
using System;
using System.Collections.Generic;

public partial class graphCreation : Node2D
{
	[Export] private PackedScene nodeScene;     
	[Export] private PackedScene edgeScene;       
	[Export] private bool isWeighted= false;  
	[Export] private bool isDirected= false; 
	[Export] private int sepValue=150;
	[Export] private bool addNodes= false;
	[Export] private bool addEdges= false;  
	[Export] private bool editable= false;  

	//added from graph generation
	[Export] private int nodeCount=0;      
	[Export] private LineEdit sourceNodeInput;
	[Export] private LineEdit goalNodeInput;
	[Export] private Label adjacencyLabel;
	[Export] private Panel formatPanel;
	[Export] private Label distanceTableLabel;
	[Export] private Label shortestPathLabel;
	
	public List<graphNode> nodeList = new List<graphNode>(); 
	public List<graphEdge> edgeList = new List<graphEdge>(); 
	private int nodeId=0;
	public bool creationMode = true;
	public int sourceNode=0;
	public int goalNode=4;
	private int[,] adjMatrix;
	Dictionary<int, List<(int,int)>> adjList;

	
	private graphNode selectNode()
	{
		graphNode nearestNode= null;
		double nearestDistance= Convert.ToDouble(sepValue);
		Vector2 mousePos=GetGlobalMousePosition();
		//Gets the current cursor position of the mouse
		foreach(graphNode item in nodeList)
		{
			double distanceToMouse = mousePos.DistanceTo(item.GlobalPosition);
			if(distanceToMouse<nearestDistance)
			{
				nearestDistance= distanceToMouse;
				nearestNode= item;
			}
		}
		//Loops through every existing node and finds the one nearest to the cursor position. 
		if(nearestNode==null)
		{
			GD.Print("No node could be found.");
			return null;
		}
		//If there are none, then it prints an error message and returns null.
		//If one is found, then the nearest node is returned.
		return nearestNode;
	}
	
	private graphNode firstNode= null;
	private graphNode secondNode= null;
	//variables to store selected nodes
	private void setEdge(graphNode selectedNode)
	{
		if(firstNode== null)
		{
			//if the user selects their first node
			firstNode= selectedNode;
			selectedNode.changeColor();
		}
		else if(secondNode== null && selectedNode!=firstNode)
		{
			//If the user selects to different nodes
			secondNode= selectedNode;
			secondNode.changeColor();
			createEdge(firstNode, secondNode, isWeighted, isDirected);
			firstNode.changeColor();
			secondNode.changeColor();
			firstNode= null;
			secondNode=null;
			//changes back the colour of each node to red. and resets the first and second node variables to null. 
		}
		else if(selectedNode== firstNode)
		{
			//if the user reselects the same node- to unselect it.
			firstNode= null;
			selectedNode.changeColor();
			//resets the first node properties
		}
		else
		{
			GD.Print("setting nodes for edges has gone wrong.");
			//In case none of these occur, an error message is displayed.
		}
	}
	
	private void createNode(int nodeNumber, Vector2 position)
	{
		GD.Print("Creating node");
		foreach (graphNode node in nodeList)
//checks if the random position picked is not too near to another node.
		{
			if (node.Position.DistanceTo(position)<sepValue)  
			{
				GD.Print("too close to another node");
				return;
			}
		}
		var newNode=(graphNode)nodeScene.Instantiate();
		newNode.initializeNode(position, nodeNumber, creationMode);
//GD.Print("node id:" nodeNumber+" position: "+position);
		AddChild(newNode);
		nodeList.Add(newNode);
		nodeId++;
		nodeCount++;
	}

	private void createEdge(graphNode start, graphNode end, bool isWeighted, bool isDirected)
	{
		int weight = 0;
		var newEdge=(graphEdge)edgeScene.Instantiate();
		newEdge.initializeEdge(start, end, isWeighted, isDirected, weight);
		
		foreach(graphEdge item in edgeList)
		{
			if(newEdge.newMidpoint== item.newMidpoint)
			{
				GD.Print("Edge is already created here!");
				return;
			}
		}
		AddChild(newEdge);
		edgeList.Add(newEdge);
	}	
	
	private void validAreaEntered()
	{
		editable= true;
	}
	
	private void validAreaExited()
	{
		editable= false;
	}
	private void addNodesPressed()
	{
		addNodes= true;
		addEdges= false;
	}

	private void onAddEdgesPressed()
	{
		addNodes= false;
		addEdges= true;
	}

	private void validAreaInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton mouseInp && editable)
		//Checks whether there is a mouse event and the area is editable
		{
			
			if(mouseInp.Pressed && mouseInp.ButtonIndex== MouseButton.Left)
			{
				if(addNodes)
				{
					Vector2 mousePosition = GetGlobalMousePosition();
					createNode(nodeId, mousePosition);
				}
				if(addEdges)
				{
					graphNode selectedNode=selectNode();
					setEdge(selectedNode);
				}
			}
		}
	}
	
	private void onWeightToggled(bool toggled_on)
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
		
	private void onDirectionToggled(bool toggled_on)
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
	
	private void onClearButtonPressed()
	{
		if(nodeList.Count!=0)
		{
			for(int i=0;i<nodeList.Count;i++)
			{
				nodeList[i].QueueFree();
			}
			nodeCount++;
			nodeList.Clear();
		}
		if(edgeList.Count!=0)
		{
			for(int j=0;j<edgeList.Count;j++)
			{
				edgeList[j].QueueFree();
			}
			edgeList.Clear();
		}
	}
	private void onDijsktraButtonPressed()
	{
		foreach(graphEdge edge in edgeList)
		{
			edge.DefaultColor= Colors.Black;
		}
		adjMatrix= formatAdjacencyMatrix(nodeList, edgeList, nodeCount);
		adjList= formatAdjacencyList(nodeList, edgeList, nodeCount);
		runDijkstra(adjMatrix, sourceNode, goalNode, nodeCount);
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
			adjacencyLabel.Text="Adjacency Matrix: \n"+ matrixText;
		}
		return adjacencyMatrix;
	}
	
	private Dictionary<int, List<(int,int)>> formatAdjacencyList(List<graphNode> nodeList, List<graphEdge> edgeList, int nodeCount)
	{
		//Dictionary with kvps of integers and lists of tuples
		Dictionary<int, List<(int,int)>> adjacencyList= new Dictionary<int, List<(int,int)>>();
		for(int i=0;i<nodeCount;i++)
		{
	//i is the nodeId as the key, value is the list of connections with tuples with connecting node and weight
			adjacencyList.Add(i, new List<(int,int)>());
		}
		
		foreach(graphEdge edge in edgeList)
		{
			int weight=edge.weight;
			int endNodeId=edge.end.nodeId;
			int startNodeId=edge.start.nodeId;
			//Adds tuples of connecting node and weight
			adjacencyList[startNodeId].Add((endNodeId, weight));
			//If the edge is undirected, it add connections from both nodes instead of just the start node.
			if(!edge.isDirected)
				adjacencyList[endNodeId].Add((startNodeId, weight));
		}
		
		//Formatting the adjacency list for text
		string aListText="";
		foreach(KeyValuePair<int, List<(int,int)>> node in adjacencyList)
		{
			aListText+=$"Node {node.Key}"+": {";
			for(int j=0;j<node.Value.Count;j++)
			{
				aListText=aListText+"["+$"{node.Value[j].Item1},{node.Value[j].Item2}"+"]";
				//only adds a comma if not last list.
				if(j==node.Value.Count-1)
					continue;
				aListText+=", ";
			}	
			aListText+="}\n";
		}
		adjacencyLabel.Text="Adjacency List: \n"+ aListText;
		return adjacencyList;
	}

	private void runDijkstra(int[, ] graph, int source, int goal, int nodeCount)
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
			GD.Print("Goal is unreachable");
			return;
		}
		GD.Print($"Shortest path length: {distance[goal]}");
		GD.Print($"Path: {string.Join(">", path)}");
		//GD.Print($"Previous: [{string.Join(",", previous)}]");
		//GD.Print($"Distance: [{string.Join(",", distance)}]");
		//GD.Print($"Visited: [{string.Join(",", visited)}]");
		//Lines to test if everything works
		
		formatDistanceTable(distance, previous);
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
	}
	
	private void formatDistanceTable(int[] distance, int[] previous)
	{
		string distanceTable= "Node Distance Previous";
		for(int i=0;i<distance.Length;i++)
		{
			distanceTable+=$"\n{i}    {distance[i]}         {previous[i]}";
		}
		distanceTableLabel.Text= distanceTable;
	}
	
	private void sourceNodeSubmitted(string sourceInput)
	{
		bool isNum= int.TryParse(sourceInput, out sourceNode);
		if(!isNum)
		{
			GD.Print("Invalid source node");
			sourceNodeInput.Text="";
			return;
		}
		if(sourceNode<0 || sourceNode>=nodeCount)
		{
			GD.Print("Invalid source node");
			sourceNodeInput.Text="";
			sourceNode= 0;
		}
	}
	private void goalNodeSubmitted(string goalInput)
	{
		//when the goal is submitted, this code checks if the goal is valid.
		bool isNum= int.TryParse(goalInput, out goalNode);
		if(!isNum)
		{
			GD.Print("Invalid goal node");
			goalNodeInput.Text="";
			return;
		}
		if(goalNode<0 || goalNode>=nodeCount)
		{
			GD.Print("Invalid goal node");
			goalNode= nodeCount-1;
			goalNodeInput.Text="";
		}
	}
	private void adjMatrixButtonPressed()
	{
		//When the adj matrix button is pressed, the matrix is shown and the panel becomes visible.
		adjMatrix= formatAdjacencyMatrix(nodeList, edgeList, nodeCount);
		formatPanel.Visible= true;
	}
	
	private void onAdjListButtonPressed()
	{
		//When the adj matrix button is pressed, the matrix is shown and the panel becomes visible.
		adjList= formatAdjacencyList(nodeList, edgeList, nodeCount);
		formatPanel.Visible= true;
	}

	private void onQuitPanelButtonPressed()
	{
		formatPanel.Visible= false;
	}
	
}
