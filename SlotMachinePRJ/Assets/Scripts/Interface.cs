using Godot;
using System;
using System.Collections.Generic;
public partial class Interface : Control
{
	public game_selection slotmachine_Sel_Menu;
	Node2D BackgroundNode;
	public Program program;
	public MainMenu MainMenu;
	public UI_headboard Headboard;
	public login_interface loginPage;
	float speed = 0.15f;
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		BackgroundNode = GetNode<Node2D>("BackgroundNode");
		MainMenu = GetNode<MainMenu>("MainMenuNode/Control");
		Headboard = GetNode<UI_headboard>("ProfileNode/Profile_UI_Headboard");
		MainMenu.program = program;
		slotmachine_Sel_Menu = GetNode<game_selection>("Game_Selection_Node/GameSelection");
		OpenLogin();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		BackgroundNode.Rotate( (float)(speed * GetProcessDeltaTime()));
		
	}
	public void OpenMainMenu(){
		//Configure Interface assets for mainmenu
		MainMenu.Show();
		slotmachine_Sel_Menu.Show();
		closeLogin();
	}
	public void CloseMainMenu(){
		//Configure Interface assets
		MainMenu.Hide();
		slotmachine_Sel_Menu.Hide();
		
	}
	public void closeLogin(){
		loginPage.Hide(); 
		Headboard.Show();
	}
	public void OpenLogin(){
		loginPage.Show();
		Headboard.Hide();
	}

	public void UpdateUserInfo(API.User userinfo){
		Headboard.UpdateUserInfo(userinfo);
	}
}
