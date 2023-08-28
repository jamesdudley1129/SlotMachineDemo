using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
public partial class Program : Node2D
{
	

	Client client;
	public static API.API_Status _Status = API.API_Status.start;
	public NetStatus networkStatus = NetStatus.awaiting_connection;


	public string[] name_symbole_png = new string[]{
		"Symbol001.png","Symbol002.png"
	};
	GameManager gm;
	Interface i_face;

	public override void _Ready()
	{
		gm = this.GetNode<GameManager>("GameMgr");
		i_face = this.GetNode<Interface>("Interface");
		gm.program = this;
		i_face.program = this;
		client = new Client(this);
	}
	public bool[]  StartUpCheckList = new Boolean[]//Loggedin,isBufferLoaded,GamesLoaded,UserDataCollected
	{
		false,false,false,false
	};
	public void StartUpProcedure(){
		StartUpCheckList[1] =  Client.Buffer.isResized;
		if(!StartUpCheckList[0]){
			//Make Sure Login Screen is active		
			if(false){
			LoginDev();//Debug Purposes
			}else{
			client.update(null);//ping
			}
		}
		if(StartUpCheckList[0]){
			//SetLoginScreen = false
			
			if(!StartUpCheckList[1]){
				//Step one Request Buffersize
				GetBufferSize(API.API_incomming.AllGameData);
			}
			else if (!StartUpCheckList[2]){
				//Step two request AllGameData
				GetAllGames();
			}
			else if (!StartUpCheckList[3]){
				//Step three request AllGameData
				GetUserData();
			}else{
				_Status = API.API_Status.RunningNormal;
			}
		}
		
	}

	//Running Logic
	public override void _Process(double delta)
	{	
		switch (_Status)
		{
			case API.API_Status.start:
				StartUpProcedure();
			break;
			case API.API_Status.RunningNormal:
				client.update(null);
			break;
			case API.API_Status.WaitingforServer:
			//Waiting for server, Clients last request is in Server que
			break;
			default:
			GD.PrintErr("Program.cs --> _Process (_Status) switch statement returning Defualt...");
			break;
		}
	}
	
	//RequestFromGameElements
	public void EventGameSelect(API.GameDataObj game){
		client.update(API.Format_SelectGameDataCMD(game.name));
	}
	public void GetSpinData(){
		client.update(API.Format_SpinDataCMD());
	}
	public void ExitGame(){
		gm.StopGame();
		i_face.OpenMainMenu();
		client.update(API.Format_SaveDataCMD());
	}
	//SendData to Server
	public void GetUserData(){
		client.update(API.Format_UserDataCMD());
	}
	public void CreateUser(string email, string password, string username){
		client.update(API.Format_CreateUserCMD(email,password,username));
	}
	public void LoginDev(){
		string email = "jamesdudley1129@gmail.com";
		string password = "pass1234";
		client.update(API.Format_LoginCMD(email,password,""));
	}
	public void Login(string email, string password,string username){
		client.update(API.Format_LoginCMD(email,password,username));
	}
	public void GetAllGames(){
		client.update(API.Format_GameDataCMD());
	}
	public void SendSelectGame(string gamename){
		client.update(API.Format_SelectGameDataCMD(gamename));
	}

	public void GetBufferSize(API.API_incomming sizetarget){
		string output = API.Format_Buffersize(sizetarget);
		client.update(output);
	}
	public void gettermsofservice(){
		client.update(API.GetTermsOfService());
	}
	public void AcceptTermsOfService(){
		client.update(API.AcceptTermsOfService());
	}
	//Process incoming Responses from Server
	
	public void ProcessResponse(string transmission){
		
		if(transmission == "ping"){ client.In_Progress= false; return;}else{
			API.API_incomming type;
			type = API.GetNetMsgType(transmission);
			
			transmission = API.UnwarpTransmission(transmission);
			//GD.PrintErr(type + "|\n" +transmission);
			switch (type)//not the correct order but temperary as i havnt finnished development
			{
				case API.API_incomming.Error:
				APIFailure(transmission);
				break;
				case API.API_incomming.RequestTOS:
				ServerResponse(transmission);
				gettermsofservice();
				break;
				case API.API_incomming.TermsOfService:
				UpdateTermsOfService(transmission);
				break;
				case API.API_incomming.RequestLogin:
				ServerResponse(transmission);
				// Display Login Page
				break;
				case API.API_incomming.CreateAUser:
				//Notify User To create Account
				ServerResponse(transmission);
				break;
				case API.API_incomming.voidTask:
				//Call function to Debug What task was succesfull
				ServerReturnedVoid(API.Deserialize.voidTaskResponse(transmission));
				break;
				case API.API_incomming.UserData:
				//API.Deserialize
				API.User response = API.Deserialize.user(transmission);
				UpdateUserData(response);
				break;
				case API.API_incomming.BufferSize:
					UpdateClientBuffer(transmission);
				break;
				case API.API_incomming.AllGameData:
				//retrive Games data from transmission API.Deserialize
				List<API.GameDataObj> games = API.Deserialize.Games(transmission);
				UpdateAllGameData(games);
				break;
				case API.API_incomming.SelectedGame:
				//retrive SelectedGame data from transmission API.Deserialize
				API.GameDataObj sel_game = API.Deserialize.SelectedGame(transmission);
				UpdateSelectedGameData(sel_game);
				break;
				case API.API_incomming.SpinData:
				//retrive Spin data from transmission API.Deserialize
				API.Symbol[] spindata = API.Deserialize.spinData(transmission);
				UpdateSpinData(spindata);
				break;
				default:
				break;
			}
			 client.In_Progress= false;
			 
		}
		
	}
	//Update Game Data

	public void UpdateClientBuffer(string transmission){
		Client.Buffer.SetBufferSize(new byte[API.Deserialize.bufferSize(transmission)]);
		StartUpCheckList[1] =  Client.Buffer.isResized;
	}
	public void ServerReturnedVoid(API.Void_Task_Type type){
		if(networkStatus == NetStatus.connected){
		networkStatus = NetStatus.logged_in;
		StartUpCheckList[0] = (networkStatus == NetStatus.logged_in);
		}
		switch(type)
		{
			case API.Void_Task_Type.login:
						i_face.loginPage.LoginResponse(true);
				break;
			case API.Void_Task_Type.save:
					GD.Print("Server Saved Succesffuly...");
				break;
			default:
				break;
		}
		
	}
	public void UpdateTermsOfService(string transmission){
		GD.Print("~~Need to prompt user and wait for acceptence~~TOS:"+ transmission);
		AcceptTermsOfService();
	}
	public void APIFailure(string transmission){
		GD.PrintErr("API Failed to Decode: " + transmission);
		_Status = API.API_Status.FailedError;
	}
	public void ServerResponse(string transmission){
		GD.PushWarning("Server Responded With: " + transmission);
	}
	public void UpdateSpinData(API.Symbol[] spinData){
		gm.UpdateOutput(spinData);
	}
	public void UpdateUserData(API.User UserData){
		StartUpCheckList[3] = true;//isUserDataLoaded
		i_face.UpdateUserInfo(UserData);
	}
	public void UpdateAllGameData(List<API.GameDataObj> allGameData){
		StartUpCheckList[2] = true;//isAllGameDataLoaded
		i_face.slotmachine_Sel_Menu.UpdateGameList(allGameData);
	}
	public void UpdateSelectedGameData(API.GameDataObj GameData){
		i_face.CloseMainMenu();
		i_face.CloseMainMenu();
		gm.StartGame(GameData);
	}
}
public enum NetStatus{
	awaiting_connection,connected,logged_in
}
