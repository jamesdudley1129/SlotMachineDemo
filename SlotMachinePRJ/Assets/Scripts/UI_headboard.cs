using Godot;
using System;

public partial class UI_headboard : ReferenceRect
{
	Label UserName;
	Label Slogin;
	Label BaseCoinBallence;
	Label VIPCoinBallence;
	API.User user;
	public override void _Ready()
	{
		UserName = GetNode<Label>("VBoxContainer/HBoxContainer/Profile/TextDevider/Username");
		Slogin = GetNode<Label>("VBoxContainer/HBoxContainer/Profile/TextDevider/Slogin");
		BaseCoinBallence = GetNode<Label>("VBoxContainer/HBoxContainer/Currency/VerticalDevider/Coins/CoinDevider/BaseCoin/Ballence");
		VIPCoinBallence = GetNode<Label>("VBoxContainer/HBoxContainer/Currency/VerticalDevider/Coins/CoinDevider/VIPCoin/Ballence");
	}
	public override void _Process(double delta)
	{
		if(user != null){
			BaseCoinBallence.Text = ShortCredits(user.slot_credits);
			VIPCoinBallence.Text = ShortCredits(user.vip_credits);
			UserName.Text = user.username.ToString();
			Slogin.Text = user.date_created.ToString();
		}
	}
	public void UpdateUserInfo(API.User user){
		this.user = user;
		BaseCoinBallence.Text = ShortCredits(user.slot_credits);
		VIPCoinBallence.Text = ShortCredits(user.vip_credits);
		UserName.Text = user.username.ToString();
		Slogin.Text = user.date_created.ToString();
	}
	public string ShortCredits(int credits){
		if(credits > 1000){
			if(credits > 1000000){
				if(credits > 1000000000){
					if(credits > 999999998){
					return (credits/999999999).ToString() + "ERROR";
					
					}
					return (credits/1000000000).ToString() + "T";

				}
				return (credits/1000000).ToString() + "M";
			}
			return (credits/1000).ToString() + "K";
		}
		return credits.ToString();
	}
	
	
}
