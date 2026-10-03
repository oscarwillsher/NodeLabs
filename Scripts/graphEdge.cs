using Godot;
using System;

public partial class graphEdge : Line2D //Pre-generated class
{
	[Export] public Sprite2D arrowHead; //Allows the graphEdge to access it's child node, arrowHead
	[Export] public LineEdit weightLabel;
	public graphNode start {get;set;}
	public graphNode end {get;set;}
	public bool isWeighted {get;set;}
	public bool isDirected {get;set;}
	public int weight {get;set;}
	public Vector2 newMidpoint{get;set;}
	private Vector2 adjustWeightLabel= new Vector2(0,10); 
	

	public void initializeEdge(graphNode newStart, graphNode newEnd, bool newIsWeighted, bool newIsDirected, int newWeight)
	{
		//constructor method
		start= newStart;
		end= newEnd;
		isWeighted= newIsWeighted;
		isDirected= newIsDirected;
		weight= newWeight;
		
		DefaultColor= Colors.Black; 
		Vector2 startPos= start.Position;
		Vector2 endPos= end.Position;
		Vector2 midpoint= new Vector2((startPos.X+endPos.X)/2,(startPos.Y+endPos.Y)/2);
		newMidpoint = midpoint;
		Points= new Vector2[] { start.Position, end.Position};
		
		if(isWeighted)
		{
			weight= Convert.ToInt32(GD.Randi()%49+1);
			weightLabel.Text=Convert.ToString(weight);
			weightLabel.Position= midpoint-adjustWeightLabel; 
			weightLabel.Visible= true;
			//GD.Print(weight);
		}
		else
		{
			weightLabel.Visible= false;
			weight= 0;
		}
		if(isDirected)
		{
			Vector2 edgeVector= endPos-startPos;
			Vector2 horizontalVector= new Vector2(1000,0);
			float dotProduct= edgeVector.Dot(horizontalVector); //dot product -> a.b
			float lengthProduct= edgeVector.Length()*horizontalVector.Length(); //finds cos theta
			float angle= Convert.ToSingle(Math.Acos(dotProduct/lengthProduct)); 
			GD.Print(angle);
			GD.Print("Before: " + arrowHead.Rotation);
			
			if(endPos.X<startPos.X && endPos.Y>startPos.Y) //quadrant 3: B is bottom left of A
			{
				arrowHead.Rotation=angle;
				GD.Print("option 3");
			}
				
			else if(endPos.X<startPos.X && endPos.Y<startPos.Y)  //quadrant 2: B is top left of A
			{
				GD.Print("Before: " + arrowHead.Rotation);
				arrowHead.Rotation=-angle;
				GD.Print("option 2");
			}
				
			else if(endPos.X>startPos.X && endPos.Y>startPos.Y) //quadrant 4: B is bottom right of A
			{
				arrowHead.Rotation=angle; 
				GD.Print("option 4");
			}
				
			else if(endPos.X>startPos.X && endPos.Y<startPos.Y) //quadrant 1: B is top right of A
			{
				GD.Print("Before: " + arrowHead.Rotation);
				arrowHead.Rotation=-angle;
				GD.Print("option 1");
			}

			GD.Print("After: "+ arrowHead.Rotation);
			arrowHead.Position= midpoint; //sets arrowhead's position to the midpoint of the edge
			arrowHead.ZIndex= ZIndex+1;
			
		}
		else
		{
			arrowHead.Visible= false;
		}
	}
	
	private void weightLabelUpdated(string updatedText)
	{
		if(isWeighted)
		{
			//try catch statement in case the user inputs a non-integer as a weight
			try
			{
				//The new weight updated by the user.
				int updatedWeight= Convert.ToInt32(updatedText);
				if(updatedWeight>0)
				{
					this.weight=updatedWeight;
				}
				else
				{
					//Reverts the displayed weight back to it's original value
					GD.Print(" Weight must be positive!");
					weightLabel.Text=Convert.ToString(this.weight);
				}
			}
			//Case where user inputs non-integer as weight
			catch
			{
				GD.Print(" Weight must be an integer!");
				weightLabel.Text=Convert.ToString(this.weight);
			}
		}
	}
	
}
