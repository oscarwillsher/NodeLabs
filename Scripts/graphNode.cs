using Godot;
using System;

public partial class graphNode : Node2D // not to be confused with the graph node
{
	[Export] public Label Number; 
	public int nodeId;
	public bool CreationMode= false;
	public bool Selected= true;
	
	[Export] public Sprite2D nodeImage;
	public Texture2D orangeTexture = ResourceLoader.Load<Texture2D>("res://Images/orange circle.png");
	public Texture2D redTexture = ResourceLoader.Load<Texture2D>("res://Images/red circle.png");
	
	// allows access to the Label nodeNumber to allow the node to be individually labelled.
	//string[] letters= 
	//{"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L","M", "N", "0", "P", "Q","R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
	
	public void initializeNode(Vector2 position, int nodeNumber,bool creationMode)
	//constructor method for the node
	{
		nodeId= nodeNumber;
		Position=position;
		//Number.Text= letters[nodeId]; 
		Number.Text= $"{nodeId}";
		Number.ZIndex=ZIndex+3;
		//sets the Label to a unique number. 
		Vector2 adjust=new Vector2(5,10);
		Number.Position=Number.Position-adjust;
		CreationMode= creationMode;
	}
	
	public void changeColor()
	{
		if(CreationMode)
		{
			if(nodeImage.Texture== redTexture)
			{
				nodeImage.Texture= orangeTexture;
			}
			else
			{
				nodeImage.Texture= redTexture;
			}
			//changes color of the node depending on if the node is selected or not.
		}
	}
	
}
