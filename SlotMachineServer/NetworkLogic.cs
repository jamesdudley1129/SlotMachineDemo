using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SlotMachineServer
{
    public class NetworkLogic
    {
        List<User> users = new List<User>();
        Dictionary<Socket, string> command_queue = new Dictionary<Socket, string>();
        ServerMain server;
        public NetworkLogic(ServerMain server)
        {
            this.server = server;
            DataBase.LoadTextureData();
            DataBase.LoadGameData();
            DataBase.LoadAccountData();
            DataBase.LoadTermsofServiceData();
        }
        public void AddToCommandQueue(Socket socket, string transmission)
        {
        //trim string
        //add to queue
        tryAgain:
            try
            {
                command_queue.Add(socket, transmission);
            }
            catch (Exception)
            {
                goto tryAgain;
            }
        }
        public void Proccess_commands()//runs in loop
        {
            string command = "";
            Socket client;
            //determins this loops task
            GetNextCommand(out client, out command);
            //checks to see if user is present in userslist
            User user = FindUserBySocket(client);
            if (client != null && command != "" && command != null)
            {
                if (user == null)
                {
                    string[] credentials;
                    IncommingCommands i_cmd = Universal_API_Logic.GetCommandLookup(command);
                    switch (i_cmd)
                    {
                        case IncommingCommands.Error:
                            break;
                        case IncommingCommands.Login:
                            user = new User(client);
                            credentials = API.TextToValue.UserLogin(command);
                            //MessageBox.Show("LoginResults:" + DataBase.AttemptLogin(credentials[0], credentials[1]).ToString());
                            if (DataBase.AttemptLogin(credentials[0], credentials[1], credentials[2], user))//succesfull
                            {
                                //adds the user to the userlist
                                users.Add(user);
                                //responds to client that last task was successfull 
                                server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.voidTaskSuccesfull, API.TextToValue.GetVoidType(i_cmd)));
                            }
                            else
                            {
                                //commands the client to try logging in again
                                server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.RequestLogin, null));
                            }
                            //login(user)
                            break;
                        case IncommingCommands.CreateAccount:
                            //sets values for server
                            user = new User(client);
                            credentials = API.TextToValue.CreateAccount(command);//returns: {email, password, user}
                            if (DataBase.CreateAccount(credentials[0], credentials[1], credentials[2]))
                            {
                                DataBase.AttemptLogin(credentials[0], credentials[1], credentials[2], user);
                                users.Add(user);
                                //responds to client
                                server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.voidTaskSuccesfull, API.TextToValue.GetVoidType(i_cmd)));
                            }
                            else
                            {
                                //responds to client
                                server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.CreateAUser, null));
                            }
                            break;
                        case IncommingCommands.GetBufferSize:
                            //responds to client
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.SendBufferSize, API.TextToValue.GetBufferSize(command)));
                            break;
                        default:
                            break;
                    }

                }
                else
                {

                    IncommingCommands i_cmd = Universal_API_Logic.GetCommandLookup(command);
                    if (DataBase.isTOS_UpdateCondition(user) && (i_cmd != IncommingCommands.GetTermsOfService || i_cmd != IncommingCommands.AcceptTermsOfService))
                    {
                        //if TOS check fails and user isnt requesting to resolve it throw API error context api.task.tos.out-of-date{}
                        server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.RequestTOS, null));
                    }
                    switch (i_cmd)
                    {
                        case IncommingCommands.Error:
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.Error, null));
                            break;
                        case IncommingCommands.GetTermsOfService:
                            //responds to client
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.SendTermsOfService, null));
                            break;
                        case IncommingCommands.AcceptTermsOfService:
                            //responds to client
                            user.last_TOS_date_accepted = DateTime.Now.ToString();
                            DataBase.UpdateUserAccount(user);   
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.voidTaskSuccesfull, API.TextToValue.GetVoidType(i_cmd)));
                            break;
                        case IncommingCommands.GetUserData:
                            //coins, username
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.SendUserData, null));
                            break;
                        case IncommingCommands.GetBufferSize:
                            //Avalable Games (Texture path for value ,Audio path..ect)
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.SendBufferSize, API.TextToValue.GetBufferSize(command)));
                            break;
                        case IncommingCommands.GetAllGameData:
                            //Avalable Games (Texture path for value ,Audio path..ect)
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.SendAllGameData, null));
                            break;
                        case IncommingCommands.SelectGame:
                            SlotMachineLogic x = API.TextToValue.SelectGame(command);
                            //MessageBox.Show("NetLogic(ln138) Game.name Value = " + x.gameName);
                            user.AssignGame(x);
                            //Texture path for value ,Audio path for value ect,Text Value, ect...
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.SendSelectedGame, null));
                            break;
                        case IncommingCommands.GetSpinData:
                            //Spin results, or error if user inssificent funds  
                            server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.SendSpinData, null));
                            break;
                        case IncommingCommands.SaveData:

                            if (DataBase.UpdateUserAccount(user))
                            {
                                server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.voidTaskSuccesfull, API.TextToValue.GetVoidType(i_cmd)));
                            }
                            else
                            {
                                server.SendResponse(client, API.PrintCommand(user, OutGoingCommands.Error, null));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public void GetNextCommand(out Socket client, out string command)
        {
        TryAgain:
            if (command_queue.Keys.Count > 0)
            {
            
                try
                {
                
                    KeyValuePair<Socket, string> pair = command_queue.First<KeyValuePair<Socket, string>>();
                    //grabs first instance of type in container
                    client = pair.Key;
                    command = pair.Value;

                    command_queue.Remove(client);
                    //does this remove all instances of commands from client if there is multiple in queue???
                    //MessageBox.Show("CommandQueNotEmpty Key:" +client.ToString() + "| cmd :" + command );
                    
                }
                catch (Exception)
                {

                    goto TryAgain;
                }
            }
            else
            {
                
                client = null;
                command = "";
            }
        }
        public void CloseUser(Socket client)
        {
            //ReleaseData Regarding client
            users.Remove(FindUserBySocket(client));
        }
        public User FindUserBySocket(Socket socket)
        {
            User current = null;
            for (int index = 0; index < users.Count; index++)
            {
                //MessageBox.Show(users[index].client + "|" + socket);
                if (users[index].GetSocket() == socket)
                {
                    
                    current = users[index];
                }
            }
            return current;
        }
    }
}
