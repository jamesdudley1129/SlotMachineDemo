using System;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Godot;

public static class API
{
		public enum API_Status
	{
		start ,RunningNormal, WaitingforServer ,FailedError, ResolvingError, Stopped	
	}
	public enum API_incomming {
		Error,TermsOfService, RequestTOS,RequestLogin, CreateAUser, voidTask, UserData, AllGameData, SelectedGame, SpinData, BufferSize,
	}
	public enum Void_Task_Type{
		save,login,error
	}
	public static Dictionary<string,API_incomming> Incominglookup = new Dictionary<string, API_incomming>(){
			["api.task.error"] = API_incomming.Error,
			["api.task.call.login"] = API_incomming.RequestLogin,
			["api.task.call.gettermsofservice"] = API_incomming.RequestTOS,
			["api.task.call.create_a_user"] = API_incomming.CreateAUser,
			["api.task.call.create_a_user"] = API_incomming.CreateAUser,
			["api.task.void.succesfull"] = API_incomming.voidTask,
			["api.task.userinfo"] = API_incomming.UserData,
			["api.task.allgamedata"] = API_incomming.AllGameData,
			["api.task.buffersize"] = API_incomming.BufferSize,
			["api.task.selectedgamedata"] = API_incomming.SelectedGame,
			["api.task.spindata"] = API_incomming.SpinData
	};
	public static string Format_LoginCMD(string email,string password,string username){
			string[] credientails = new string[]{email,password,username};
			/*
			//XmlRootAttribute root = new XmlRootAttribute("root");
			XmlSerializer serializer = new XmlSerializer(typeof(string[]));
			MemoryStream stream = new MemoryStream();
			serializer.Serialize(stream,credientails);
			string[] output = (string[])serializer.Deserialize(XmlReader.Create(stream),);
			*/
			return "api.task.login{"+email+"}{"+password+"}{"+username+"}:";
			

	}
	public static string Format_UserDataCMD(){
			return "api.task.getuserdata{}:";
	}
	public static string GetTermsOfService(){
		return "api.task.gettermsofservice{}:";
	}
	public static string AcceptTermsOfService(){
		return "api.task.accepttos{}:";
	}
	public static string Format_Buffersize(API.API_incomming type){
		
		string result = "";
		switch (type)
		{
			case API_incomming.UserData:
			result = "api.task.getuserdata"; 
			break; 
			case API_incomming.AllGameData: 
			result = "api.task.getgamedata";
			break; 
			case API_incomming.SelectedGame: 
			result = "api.task.selectgame";
			break; 
			case API_incomming.SpinData:
			result = "api.task.spin"; 
			break; 
			case API_incomming.BufferSize: //Cant see the usecase of of now but whatever for the sake of standerdization
			result = "api.task.getbuffersize";
			break; 
			default: 
			break; 
		}
		return "api.task.getbuffersize{"+ result +"}:";
	}
	public static string Format_GameDataCMD(){
			return "api.task.getgamedata{}:";
	}
	public static string Format_SpinDataCMD(){
			return "api.task.spin{}:";
	}
	public static string Format_SelectGameDataCMD(string gamename){
			return "api.task.selectgame{"+gamename+"}:";
	}
	public static string Format_CreateUserCMD(string email, string password, string username){
		return "api.task.createuser{"+ email +"}{"+ password +"}{"+ username +"}:";
	}
	public static string Format_SaveDataCMD(){
		return "api.task.savedata{}:";
	}
	public static API_incomming GetNetMsgType(string transmission){
		API_incomming type = API_incomming.Error;
		transmission = transmission.Remove(transmission.IndexOf('{'),transmission.Length - transmission.IndexOf('{'));
		Incominglookup.TryGetValue(transmission.ToLower(), out type);
		return type;
	}
	public static string UnwarpTransmission(string transmission){
		transmission = transmission.Remove(0,transmission.IndexOf('{')+1);
		transmission = transmission.Remove(transmission.IndexOf('}'),(transmission.Length - transmission.LastIndexOf(':'))+1);
		return transmission;
	}
	public static class Deserialize{
		public static XmlRootAttribute root = new XmlRootAttribute("root");
		public static XmlSerializer User_serializer = new XmlSerializer(typeof(User),root);
		public static XmlSerializer GameData_serializer = new XmlSerializer(typeof(GameDataObj),root);
		public static XmlSerializer All_GameData_serializer = new XmlSerializer(typeof(List<GameDataObj>),root);
		public static int bufferSize(string command){
			int bytes = 0;
			int.TryParse(command, out bytes);
			if (bytes < 1024){bytes = 1024;}
			return bytes;
		}
		public static User user(string transmission){
			StringReader sr = new StringReader(transmission);
			return (User)User_serializer.Deserialize(sr);
		}
		public static GameDataObj SelectedGame(string transmission){

			StringReader sr = new StringReader(transmission);
			GameDataObj game = (GameDataObj)GameData_serializer.Deserialize(sr);
			return game;
		}
		public static List<GameDataObj> Games(string transmission){
			StringReader sr = new StringReader(transmission);
			List<GameDataObj> games = (List<GameDataObj>)All_GameData_serializer.Deserialize(sr);
			return games;
		}
		public static Symbol[] spinData(string transmission){
			XmlRootAttribute root = new XmlRootAttribute("root");
			XmlSerializer serializer = new XmlSerializer(typeof(Symbol[]),root);
			StringReader sr = new StringReader(transmission);
			Symbol[] spin = (Symbol[])serializer.Deserialize(sr);
			return spin;
		}
		//could be revamped by adding a string to enum lookup unfortuanatly there isnt a enum yet for outgoing transmissions
		public static Void_Task_Type voidTaskResponse(string transmission){
			if(transmission.Contains("task.save")){
				return Void_Task_Type.save;
			}
			if(transmission.Contains("task.login")){
				return Void_Task_Type.login;
			}
			GD.PrintErr("API Error [voidTaskResponse] unexpected response");
			return Void_Task_Type.error;
		}
	}
	public class Symbol{
		public string texture;
		public bool winningSymbol;
		public Symbol(){}
	}
	public class GameDataObj
	{
		public GameDataObj(){}
		public string name;
		public string asset_name;
		public int numOfSymbols;
		public float winLoseRatio;
		public int numOfOutputs;
		public int PayoutModifier;
		//machine textures
		public string frameTexture;
		public string artTexture;
		public string headBoardTexture;
		public string backgroundTexture;
		//play AreaTextures
		public List<string> symbolTextures = new List<string>();
		public string PlayAreaBackgroundTexture;
		public string GamekeyBackgroundAsset;
	}
	public class User{
		public bool loggedIn = false;
		public string username;
		public int slot_credits = 0;
		public int vip_credits = 0;
		public string date_created;
		public User(){}
	}
}

	
