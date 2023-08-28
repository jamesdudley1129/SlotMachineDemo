using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
	public static GameManager instance;
	public Program program;
	public object machine;
	public API.GameDataObj game;
	
	//list of game types by asset names
	public Dictionary<string,object> gamelookup = new Dictionary<string, object>(){
		["SlotMachineFrame001.tscn"] = typeof(slot_machine),
	};

	public override void _Ready(){
		GameManager.instance = this;
	}
	public void StartGame(API.GameDataObj game){
		this.game = game;

		var pkg = GD.Load<PackedScene>(IO.assetPath + game.asset_name);
		var instance = pkg.Instantiate();
		AddChild(instance);
		gamelookup.TryGetValue(game.asset_name, out object x);//getstype
		
		//checks if its a slot machine game
		if(x == typeof(slot_machine)){
			machine = (slot_machine)instance;
			slot_machine tempCast = (slot_machine)machine;
			tempCast.Assign();
			GD.Print("Flex Code works");
		}
	}
	public void StopGame(){
		gamelookup.TryGetValue(game.asset_name, out object x);
		if(x == typeof(slot_machine)){
			slot_machine tempCast = (slot_machine)machine;
			tempCast.QueueFree();
		}
	}
	public void UpdateOutput(API.Symbol[] symbols){
		gamelookup.TryGetValue(game.asset_name, out object x);//getstype
		
		if(x == typeof(slot_machine)){
			slot_machine tempCast = (slot_machine)machine;
			tempCast.UpdateSpin(symbols);
			GD.Print("Flex Code works");
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//menu or browsing slot machines
		
			//get slot machine name
			//ask server for game info
			//set game stats
	}
	
	
}
