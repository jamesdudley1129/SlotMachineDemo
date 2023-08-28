using Godot;
using System;
using System.Collections.Generic;
public partial class MainMenu : Control
{
	
	public ReferenceRect newsboard_Menu_Node;
	public Program program;
	public override void _Ready()
	{
		//assign nodes
		newsboard_Menu_Node = GetNode<ReferenceRect>("newsboard");
		//assign scripts
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.

}
