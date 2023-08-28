using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;

namespace SlotMachineServer
{

    public static class API
    {
        
        public static Dictionary<string, IncommingCommands> lookup = new Dictionary<string, IncommingCommands>()
        {
            ["api.task.createuser"] = IncommingCommands.CreateAccount,
            ["api.task.login"] = IncommingCommands.Login,
            ["api.task.getgamedata"] = IncommingCommands.GetAllGameData,
            ["api.task.getuserdata"] = IncommingCommands.GetUserData,
            ["api.task.selectgame"] = IncommingCommands.SelectGame,
            ["api.task.spin"] = IncommingCommands.GetSpinData,
            ["api.task.getbuffersize"] = IncommingCommands.GetBufferSize,
            ["api.task.savedata"] = IncommingCommands.SaveData,
            ["api.task.gettermsofservice"] = IncommingCommands.GetTermsOfService,
            ["api.task.accepttos"] = IncommingCommands.AcceptTermsOfService

        };
        public static string PrintCommand(User user ,OutGoingCommands command, string Args)
        {
            string result = "error";
            
            switch (command)
            {
                case OutGoingCommands.Error:
                    result = "api.task.error{"+Args+"}:";
                    break;
                case OutGoingCommands.RequestTOS:
                    result = "api.task.call.gettermsofservice{}:";
                    break;
                case OutGoingCommands.RequestLogin:
                    result = "api.task.call.login{}:";
                    break;
                case OutGoingCommands.CreateAUser:
                    result = "api.task.call.create_a_user{}:";
                    break;
                case OutGoingCommands.voidTaskSuccesfull:
                    result = "api.task.void.succesfull{"+ Args + "}:";
                    break;
                case OutGoingCommands.SendUserData:
                    result = "api.task.UserInfo{" + ValueToText.UserDataString(user) + "}:";
                    break;
                case OutGoingCommands.SendBufferSize:
                    result = "api.task.BufferSize{" + Args + "}:";
                    //MessageBox.Show("Sending -->" + result);
                    break;
                case OutGoingCommands.SendAllGameData:
                    result = "";
                    result = "api.task.AllGameData{"+ValueToText.AllGameData()+"}:";
                    break;
                case OutGoingCommands.SendSelectedGame:
                    result = "";
                    result = "api.task.SelectedGameData{" + ValueToText.SelectedGameData(user) + "}:";
                    break;
                case OutGoingCommands.SendSpinData:
                    result = "";
                    result = "api.task.SpinData{" +ValueToText.SpinData(user) + "}:";
                    break;
                case OutGoingCommands.SendTermsOfService:
                    result = "api.task.TermsOfService{"+ValueToText.TOS()+"}:";
                    break;
                default:
                    result = "error";
                    break;
            }

            return result;
        }

        
       
        public class TextToValue
        {
            
            public static string[] CreateAccount(string command)
            {
                //"api.task.createuser{" + email + "}{" + password + "}{" + username + "}:"
                string username = "";
                string password = "";
                string email = "";
                //deserialize here
                command = command.Remove(0, command.IndexOf('{') + 1);
                
                string[] credintails = command.Split('{');
                //credintails = <email>} <password>} <username>}:

                //value = cred[x].remove(<represends starting pos and the remaining length after starting pos>)
                email = credintails[0].Remove(credintails[0].IndexOf('}'), credintails[0].Length - credintails[0].IndexOf('}'));
                password = credintails[1].Remove(credintails[1].IndexOf('}'), credintails[1].Length - credintails[1].IndexOf('}'));
                username = credintails[2].Remove(credintails[2].IndexOf('}'), credintails[2].Length - credintails[2].IndexOf('}'));

                return new string[] { email, password ,username};
            }
            public static string[] UserLogin(string command)
            {
                //deserialize here
                command = command.Remove(0, command.IndexOf('{') +1);
                string[] split = command.Split('{');
                //email
                split[0] = split[0].Remove(split[0].IndexOf('}'), split[0].Length - split[0].IndexOf('}'));
                //password
                split[1] = split[1].Remove(split[1].IndexOf('}'), split[1].Length - split[1].IndexOf('}'));
                //username
                split[2] = split[2].Remove(split[2].IndexOf('}'), split[2].Length - split[2].IndexOf('}'));
                
                return split;
            }
            public static SlotMachineLogic SelectGame(string command)
            {
                //isolate the game name passed in the network transmision
                command = command.Remove(0, command.IndexOf("{") + 1);
                string game_name = command.Remove(command.IndexOf("}"), command.Length - command.IndexOf(":") +1);
                //MessageBox.Show("API.TTV.SelectGame(ln112) Game.name Value = " + game_name);
                //find game with same name
                GameDataObj refrence;
                SlotMachineLogic slotLogic = null;
                foreach (GameDataObj lookup in DataBase.games)
                {
                    if(lookup.name == game_name)
                    {
                        refrence = lookup;
                        slotLogic = new SlotMachineLogic(
                            refrence.name, refrence.symbolTextures.ToArray(), 
                            refrence.numOfOutputs, refrence.numOfSymbols, 
                            refrence.winLoseRatio, refrence.PayoutModifier);
                        break;
                    }
                }
                return slotLogic;
                                

            }
            public static string GetBufferSize(string command)
            {
                command = command.Remove(0, command.IndexOf('{')+1);
                command = command.Remove(command.IndexOf('}'), command.Length - command.IndexOf('}'));
                //finds the task that the client wants the buffer size for
                string bufferSize = command;
                //MessageBox.Show("read you fucking idiot.." + bufferSize);
                IncommingCommands targetTask;
                lookup.TryGetValue(bufferSize.ToLower(),out targetTask);
                //targetTask = IncommingCommands.GetAllGameData;
                switch (targetTask)
                {
                    case IncommingCommands.Error:
                        bufferSize = "1024";
                        break;
                    case IncommingCommands.Login:
                        bufferSize = "1024";
                        break;
                    case IncommingCommands.CreateAccount:
                        bufferSize = "1024";
                        break;
                    case IncommingCommands.GetUserData:
                        bufferSize = "1024";
                        break;
                    case IncommingCommands.GetAllGameData:
                        bufferSize = ASCIIEncoding.ASCII.GetBytes("api.task.AllGameData{" + ValueToText.AllGameData() + "}:").Length.ToString();
                        break;
                    case IncommingCommands.SelectGame:
                        bufferSize = "1024";
                        break;
                    case IncommingCommands.GetSpinData:
                        bufferSize = "1024";
                        break;
                    case IncommingCommands.GetBufferSize:
                        bufferSize = "1024";
                        break;
                    case IncommingCommands.SaveData:
                        bufferSize = "1024";
                        break;
                    case IncommingCommands.GetTermsOfService:
                        bufferSize = ASCIIEncoding.ASCII.GetBytes("api.task.gettermsofservice{" +ValueToText.TOS()+"}:").Length.ToString();
                        break;
                    default:
                        break;
                }
                return bufferSize;
            }
            public static string GetVoidType(IncommingCommands type)
            {
                switch (type)
                {
                case IncommingCommands.Login:
                    return "task.login";
                    break;
                case IncommingCommands.SaveData:
                    return "task.save";
                    break;
                case IncommingCommands.CreateAccount:
                    return "task.login";
                    break;
                case IncommingCommands.AcceptTermsOfService:
                    return "task.accepttos";
                    break;
                    default:
                    return "task.error";
                    break;
                }
            }
        }
        private class ValueToText
        {
            public static string TOS()
            {
                return DataBase.TOS.Date.ToString() + DataBase.TOS.Terms;
            }
            public static string UserDataString(User user)
            {
                XmlRootAttribute root = new XmlRootAttribute("root");
                XmlSerializer serializer = new XmlSerializer(typeof(User), root);
                StringBuilder sb = new StringBuilder();
                StringWriter test = new StringWriter(sb);
                serializer.Serialize(test, user);
                test.Flush();
                test.Close();
                test.Dispose();
                return sb.ToString();
            }
            public static string AllGameData()
            {
                XmlRootAttribute root = new XmlRootAttribute("root");
                XmlSerializer serializer = new XmlSerializer(typeof(List<GameDataObj>),root);
                StringBuilder x = new StringBuilder();
                StringWriter sw = new StringWriter(x);
                serializer.Serialize(sw, DataBase.games);
                sw.Flush();
                sw.Close();
                sw.Dispose();
                //string results  = (string)serializer.Deserialize(sr);

                return sw.ToString();
            }
            public static string SelectedGameData(User user)
            {

                GameDataObj data = null;
                foreach (GameDataObj game in DataBase.games)
                {
                    if (game.name == user.GetGame().gameName) {
                        data = game;
                        break;
                    }
                }
                XmlRootAttribute root = new XmlRootAttribute("root");
                XmlSerializer serializer = new XmlSerializer(typeof(GameDataObj),root);
                StringBuilder sb = new StringBuilder();
                StringWriter SW = new StringWriter(sb);
                serializer.Serialize(SW, data);
                SW.Flush();
                SW.Close();
                SW.Dispose();
                return SW.ToString();
            }
            public static string SpinData(User user)
            {
                XmlRootAttribute root = new XmlRootAttribute("root");
                XmlSerializer serializer = new XmlSerializer(typeof(SlotMachineLogic.Symbol[]),root);
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter();
                serializer.Serialize(sw, user.Spin());
                sw.Flush();
                sw.Close();
                sw.Dispose();
                //MessageBox.Show(sw.ToString());
                return sw.ToString();
            }
        }

    }

}
