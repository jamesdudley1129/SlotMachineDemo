using SlotMachineServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static SlotMachineServer.API;

namespace SlotMachineServer
{
    internal class Universal_API_Logic
    {
        public void ProcessResponse(string cmd)
        {
            GetCommandLookup(cmd);
        }
        public static IncommingCommands GetCommandLookup(string cmd)
        {
            IncommingCommands result = IncommingCommands.Error;
            string CMD_api = cmd.Remove(cmd.IndexOf('{'), cmd.Length - cmd.IndexOf('{'));
            lookup.TryGetValue(CMD_api.ToLower(), out result);
            //MessageBox.Show(result.ToString());
            return result;
        }
    }
}
enum API_ID
{
    API_Error, SignIn, CreateAccount, UpdateAccount, GameInfo, TermsOfService, BufferSize,
}
enum API_SlotMachine
{
    AllGamesData, GameSelectionData, SpinData,
}
enum API_User_ID 
{
    CreateUser, UserData, SaveData
}

public enum IncommingCommands
{
    CreateAccount,Login,GetAllGameData,GetUserData,SelectGame,GetSpinData,GetBufferSize,SaveData,AcceptTermsOfService,GetTermsOfService,Error 
}
public enum OutGoingCommands
{
    Error,RequestTOS,RequestLogin,CreateAUser,SendTermsOfService , voidTaskSuccesfull, SendUserData, SendAllGameData, SendSelectedGame, SendSpinData, SendBufferSize, 
}
/*
if (user == null)
{
    string[] credentials;
    API.IncommingCommands i_cmd = API.GetCommandType(command);
    switch (i_cmd)
    {
        case API.IncommingCommands.Error:
            break;
        case API.IncommingCommands.Login:
            user = new User(client);
            credentials = API.TextToValue.UserLogin(command);
            //MessageBox.Show("LoginResults:" + DataBase.AttemptLogin(credentials[0], credentials[1]).ToString());
            if (DataBase.AttemptLogin(credentials[0], credentials[1], credentials[2], user))//succesfull
            {
                //adds the user to the userlist
                users.Add(user);
                //responds to client that last task was successfull 
                server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.voidTaskSuccesfull, API.TextToValue.GetVoidType(i_cmd)));
            }
            else
            {
                //commands the client to try logging in again
                server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.RequestLogin, null));
            }
            //login(user)
            break;
        case API.IncommingCommands.CreateAccount:
            //sets values for server
            user = new User(client);
            credentials = API.TextToValue.CreateAccount(command);//returns: {email, password, user}
            if (DataBase.CreateAccount(credentials[0], credentials[1], credentials[2]))
            {
                DataBase.AttemptLogin(credentials[0], credentials[1], credentials[2], user);
                users.Add(user);
                //responds to client
                server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.voidTaskSuccesfull, API.TextToValue.GetVoidType(i_cmd)));
            }
            else
            {
                //responds to client
                server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.CreateAUser, null));
            }
            break;
        case API.IncommingCommands.GetBufferSize:
            //responds to client
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.SendBufferSize, API.TextToValue.GetBufferSize(command)));
            break;
        default:
            break;
    }

}
else
{

    API.IncommingCommands i_cmd = API.GetCommandType(command);
    if (DataBase.isTOS_UpdateCondition(user) && (i_cmd != API.IncommingCommands.GetTermsOfService || i_cmd != API.IncommingCommands.AcceptTermsOfService))
    {
        //if TOS check fails and user isnt requesting to resolve it throw API error context api.task.tos.out-of-date{}
        server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.Error, "api.task.call.gettermsofservice"));
    }
    switch (i_cmd)
    {
        case API.IncommingCommands.Error:
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.Error, null));
            break;
        case API.IncommingCommands.GetTermsOfService:
            //responds to client
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.SendTermsOfService, null));
            break;
        case API.IncommingCommands.AcceptTermsOfService:
            //responds to client
            user.last_TOS_date_accepted = DateTime.Now.ToString();
            DataBase.UpdateUserAccount(user);
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.voidTaskSuccesfull, API.TextToValue.GetVoidType(i_cmd)));
            break;
        case API.IncommingCommands.GetUserData:
            //coins, username
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.SendUserData, null));
            break;
        case API.IncommingCommands.GetBufferSize:
            //Avalable Games (Texture path for value ,Audio path..ect)
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.SendBufferSize, API.TextToValue.GetBufferSize(command)));
            break;
        case API.IncommingCommands.GetAllGameData:
            //Avalable Games (Texture path for value ,Audio path..ect)
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.SendAllGameData, null));
            break;
        case API.IncommingCommands.SelectGame:
            SlotMachineLogic x = API.TextToValue.SelectGame(command);
            //MessageBox.Show("NetLogic(ln138) Game.name Value = " + x.gameName);
            user.AssignGame(x);
            //Texture path for value ,Audio path for value ect,Text Value, ect...
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.SendSelectedGame, null));
            break;
        case API.IncommingCommands.GetSpinData:
            //Spin results, or error if user inssificent funds
            server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.SendSpinData, null));
            break;
        case API.IncommingCommands.SaveData:

            if (DataBase.UpdateUserAccount(user))
            {
                server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.voidTaskSuccesfull, API.TextToValue.GetVoidType(i_cmd)));
            }
            else
            {
                server.SendResponse(client, API.PrintCommand(user, API.OutGoingCommands.Error, null));
            }
            break;
        default:
            break;
    }
}
        */