using Godot;
using System;
using System.Collections.Generic;
public partial class slot_machine : Node2D
{
	//contain refrences to textures
	//contain refrences to play area
	//contain refrences to outputs
	public class PlayArea{
		public Node2D node;//assigned
		public Node2D[] VerticalOuputs;//assigned
		public Texture2D BackgroundPannel;//assigned
		public String TopText;//assigned
		public Texture2D TargetSymbol;//assigned
		public string BottomText;//assigned
		public PlayArea(Node2D node){
			this.node = node;
		}
	}
	public GameManager gm;
	public API.GameDataObj game;
	public PlayArea play;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GameManager.instance;
		var loaded = GD.Load<PackedScene>(IO.assetPath + IO.slot_machine_assets.play_area_asset);
		var instance = loaded.Instantiate();
		AddChild(instance);
		play = new PlayArea((Node2D)instance);
	}
	public void Assign(){
		game = gm.game;
		Setup_PlayArea();
	}
	public void Setup_PlayArea(){
		GD.Print("GOOD START ASSIGN");
		play.BackgroundPannel = IO.LoadBackgroundTexture(game.PlayAreaBackgroundTexture);
		play.TargetSymbol = IO.LoadSymbolTexture(game.symbolTextures[0]);
		play.TopText = "Match "+game.numOfOutputs+"X in a Row with the target:";
		play.BottomText = "& Win"+game.PayoutModifier+"X";
		play.VerticalOuputs = new Node2D[game.numOfOutputs];
		for(int i = 0; i < game.numOfOutputs; i++){
			var loaded = GD.Load<PackedScene>(IO.assetPath + IO.slot_machine_assets.output_asset);
			var V_Output = loaded.Instantiate();
			play.node.GetNode<Node2D>("OutputPannel/RollerAttachment"+ (i).ToString()).AddChild(V_Output);
			GD.PrintErr(V_Output.GetPath());
			play.VerticalOuputs[i] = (Node2D)V_Output;
		}
	//play.node.Connect()
		Display_Update_PlayArea();
	}
	public void Display_Update_PlayArea(){
		play.node.GetNode<MeshInstance2D>("Background").Texture = play.BackgroundPannel;
		play.node.GetNode<MeshInstance2D>("PayoutPannel/TargetSymbol").Texture = play.TargetSymbol;
		play.node.GetNode<RichTextLabel>("PayoutPannel/TextTop").Text = play.TopText;
		play.node.GetNode<RichTextLabel>("PayoutPannel/TextBottom").Text = play.BottomText;
		//Set vertical outputs Randomly
	}
	public void UpdateOutputDisplay(){

	}
	public override void _Process(double delta){
		UpdateNullOutputs();
	}
	public void UpdateNullOutputs(){
		if(play.VerticalOuputs[game.numOfOutputs-1].GetNode<MeshInstance2D>("Roller Item2").Texture == null){
			Texture2D[] textures = new Texture2D[game.symbolTextures.Count];
			for (int i = 0; i < game.symbolTextures.Count; i++)
			{
				textures[i] = IO.LoadSymbolTexture(game.symbolTextures[i]);
			}
			GD.PrintErr(textures);
			RandomNumberGenerator x = new RandomNumberGenerator();
			play.VerticalOuputs[0].GetNode<MeshInstance2D>("Roller Item0").Texture = textures[x.RandiRange(0,textures.Length-1)];
			play.VerticalOuputs[0].GetNode<MeshInstance2D>("Roller Item1").Texture = textures[x.RandiRange(0,textures.Length-1)];
			play.VerticalOuputs[0].GetNode<MeshInstance2D>("Roller Item2").Texture = textures[x.RandiRange(0,textures.Length-1)];
			play.VerticalOuputs[1].GetNode<MeshInstance2D>("Roller Item0").Texture = textures[x.RandiRange(0,textures.Length-1)];
			play.VerticalOuputs[1].GetNode<MeshInstance2D>("Roller Item1").Texture = textures[x.RandiRange(0,textures.Length-1)];
			play.VerticalOuputs[1].GetNode<MeshInstance2D>("Roller Item2").Texture = textures[x.RandiRange(0,textures.Length-1)];
			play.VerticalOuputs[2].GetNode<MeshInstance2D>("Roller Item0").Texture = textures[x.RandiRange(0,textures.Length-1)];
			play.VerticalOuputs[2].GetNode<MeshInstance2D>("Roller Item1").Texture = textures[x.RandiRange(0,textures.Length-1)];
			play.VerticalOuputs[2].GetNode<MeshInstance2D>("Roller Item2").Texture = textures[x.RandiRange(0,textures.Length-1)];
		}
	}
	//spinning animation
	public void UpdateSpin(API.Symbol[] symbols){
		//SetOutput(symbols);
		int index = 0;
		foreach (Node2D V_output in play.VerticalOuputs)
		{
			V_output.GetNode<MeshInstance2D>("Roller Item1").Texture = IO.LoadSymbolTexture(symbols[index].texture);
			index++;
			V_output.GetNode<MeshInstance2D>("Roller Item0").Texture = IO.LoadSymbolTexture(symbols[index].texture);
			index++;
			V_output.GetNode<MeshInstance2D>("Roller Item2").Texture = IO.LoadSymbolTexture(symbols[index].texture);
			index++;
		}
	}
	private void _on_spin_button_pressed()
	{
		gm.program.GetSpinData();
		gm.program.GetUserData();
	}
	private void _on_button_pressed()
	{
		gm.program.ExitGame();
	}
}


