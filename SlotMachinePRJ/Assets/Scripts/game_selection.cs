using Godot;
using System;
using System.Collections.Generic;
public partial class game_selection : Control
{
	SlotMachineSampler sampler;
	Interface i_face;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		i_face = GetNode<Interface>("/root/Main/Interface");
		sampler = new SlotMachineSampler(GetNode<ReferenceRect>("Rect"));
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		sampler.UpdatePos();
	}
	public void UpdateGameList(List<API.GameDataObj> games){
		SlotMachineSampler.games = games;
		sampler.FirstRender();
	}
	private void _on_select_btn_pressed()
	{
		API.GameDataObj game = sampler.GetCenterSample();
		if( game != null){
			i_face.program.EventGameSelect(game);
		}
	}	
}

