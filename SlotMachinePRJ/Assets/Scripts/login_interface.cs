using Godot;
using System;

public partial class login_interface : ReferenceRect
{
	// Called when the node enters the scene tree for the first time.
	Program program;
	Interface I_face;
	LineEdit email_input_field;
	LineEdit username_input_field;
	LineEdit password_input_field;
	Label output_disp;
	public override void _Ready()
	{
		program = GetNode<Program>("/root/Main/");
		I_face = GetNode<Interface>("/root/Main/Interface");
		email_input_field = GetNode<LineEdit>("email_Inp");
		username_input_field = GetNode<LineEdit>("username_Inp");
		password_input_field = GetNode<LineEdit>("password_Inp");
		output_disp = GetNode<Label>("Output_Msg");
		I_face.loginPage = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public void LoginResponse(bool accepted){
		if(accepted){
			I_face.OpenMainMenu();
		}else{
			output_disp.Text = "Failed to log in. \nCheck Username and password and try again or create a new account.";
		}
	}
	private void _on_auto_fill_btn_pressed()
	{
		program.LoginDev();
	}
	private void _on_login_btn_pressed()
	{
		program.Login(email_input_field.Text,password_input_field.Text,username_input_field.Text);
	}
	private void _on_create_btn_pressed()
	{
		program.CreateUser(email_input_field.Text,password_input_field.Text,username_input_field.Text);
	}

}


