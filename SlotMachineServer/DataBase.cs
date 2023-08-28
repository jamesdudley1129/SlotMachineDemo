using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Globalization;
using System.Xml.Schema;

namespace SlotMachineServer
{
    static public class DataBase
    {
        //----------------------Values--------------------------//
        internal static List<AccountDataObj> accounts = new List<AccountDataObj>();
        public static char[] illigalChars = new char[] { '{', '}', ':', ',', '\\' };
        public static List<GameDataObj> games = new List<GameDataObj>();
        public static TextureData textureData;
        public static TermsOfService TOS;
        //------------------------IO----------------------------//
        static string root = Environment.CurrentDirectory;
        static string xmlGamesPath = root + @"\Resources\Games.xml";
        static string xmlAccountsPath = root + @"\Resources\Accounts.xml";
        static string xmlTexturesPath = root + @"\Resources\Textures.xml";
        public static void LoadTextureData()
        {
            StreamReader stream = new StreamReader(xmlTexturesPath);
            XmlRootAttribute root = new XmlRootAttribute();
            XmlSerializer serializer = new XmlSerializer(typeof(TextureData), root);
            //deserialize
            textureData = (TextureData)serializer.Deserialize(stream);
        }
        public static void LoadGameData()
        {
            StreamReader stream = new StreamReader(xmlGamesPath);
            XmlRootAttribute root = new XmlRootAttribute();
            XmlSerializer serializer = new XmlSerializer(typeof(List<GameDataObj>), root);
            //deserialize
            games = (List<GameDataObj>)serializer.Deserialize(stream);
        }
        public static void LoadAccountData()
        {
            StreamReader stream = new StreamReader(xmlAccountsPath);
            XmlRootAttribute root = new XmlRootAttribute();
            XmlSerializer serializer = new XmlSerializer(typeof(List<AccountDataObj>), root);
            //deserialize
            accounts = (List<AccountDataObj>)serializer.Deserialize(stream);
        }
        public static void LoadTermsofServiceData()
        {
            TOS = new TermsOfService(DateTime.Now.ToString(), "Terms Of Service: User Security is Vary important never share your credentials with anyone. Violations inlcude but are not limited to; cheating, doxxing, leaking credentaial information, failure to comply with your local or federal goverments ordinance, and knowingly not reporting users associated with any of the violations mentioned are subject to Account deleteion and perminate Account banning from all services without refund. By contenuing You the (user) accept these Terms Of Services.");
        }
        public static void WriteAccountData(AccountDataObj account)
        {
            accounts.Add(account);
            StreamWriter stream = new StreamWriter(xmlAccountsPath);
            XmlRootAttribute root = new XmlRootAttribute();
            XmlSerializer serializer = new XmlSerializer(typeof(List<AccountDataObj>), root);
            serializer.Serialize(stream, accounts);
        }
        //-------------------Runtime Task-----------------------//
        public static bool AttemptLogin(string email, string password, string username, User usr)
        {
            bool matched = false;
            foreach (AccountDataObj acc in accounts)
            {
                //attempt login with email
                if (acc.email.ToString() == email.ToString() && acc.password.ToString() == password.ToString())
                {
                    matched = true;
                    usr.AssignConnectedAccount(acc);
                }//attempt login with username
                else if (acc.username.ToString() == username.ToString() && acc.password.ToString() == password.ToString())
                {
                    matched = true;
                    usr.AssignConnectedAccount(acc);
                }
            } 
            return matched;
        }
        public static bool CreateAccount(string email,string password,string username)
        {
            //Is email is real or not. I am not sure how you would do this though.
            if (email.Length < 5 || password.Length < 5 || username.Length < 5)
            {
                return false;
            }
            if (!email.Contains("@") || !email.Contains(".")) 
            {
                return false;
            }
            foreach (AccountDataObj acc in accounts)
            {
                if (acc.email == email || acc.username == username)
                {
                    return false;
                }
            }
            if(username == password || email == password || email == username)
            {
                return false;
            }
            foreach (char c in illigalChars) {
                if (username.Contains(c)|| password.Contains(c))
                {
                    return false;
                }
            }
            AccountDataObj account = new AccountDataObj();
            account.email = email;
            account.username = username;
            account.password = password;
            account.date_created = DateTime.Now.Date.ToShortDateString();
            account.slot_credits = 0;
            account.vip_credits = 200;
            accounts.Add(account);
            WriteAccountData(account);
            return true;
        }
        public static bool UpdateUserAccount(User user)
        {
            AccountDataObj acc = user.GetConnectedAccount();
            if (acc != null)
            {
                acc.last_TOS_date_accepted = user.last_TOS_date_accepted;
                acc.slot_credits = user.slot_credits;
                acc.vip_credits  = user.vip_credits; 
                //do i need to push or replace the account already in the list??
                return true;
            }
            return false;
        }
        public static bool SaveAllData()
        {
            //not saving anything yet
            if (true)
            {
                return true;
            }
            return false;
        }
        public static bool isTOS_UpdateCondition(User user)
        {
            if(DateTime.TryParse(user.last_TOS_date_accepted, out DateTime lastAccepted) && DateTime.TryParse(TOS.Date, out DateTime lastTOS_Update))
            {
                //parse completed
               if(DateTime.Compare(lastAccepted, lastTOS_Update) < 0) // last accepted is older last terms of service change (0 == same date && newer date > 0) 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;//if we fail to parse we need to update the date anyways.
        }
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
    public class AccountDataObj
    {
        public string email;
        public string username;
        public string password;
        public string date_created;
        public string last_TOS_date_accepted;
        public int slot_credits;
        public int vip_credits;
        public AccountDataObj(){}
    }
    public class TextureData
    {
        public TextureData(){ }
        public string frameTexturesPath;
        public List<string> frameTextures = new List<string>();
        public string artTexturesPath;
        public List<string> artTextures = new List<string>();
        public string headBoardTexturesPath;
        public List<string> headBoardTextures = new List<string>();
        public string symbolTexturesPath;
        public List<string> symbolTextures = new List<string>();
        public string backgroundTexturesPath;
        public List<string> backgroundTextures = new List<string>();
    }
    public class TermsOfService
    {
        public string Date;
        public string Terms;
        public TermsOfService(){ }//for serializer
        public TermsOfService(string Date, string Terms) 
        { this.Date = Date; 
          this.Terms = Terms;
        }//for manualOverride

    }
}
