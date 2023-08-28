using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Data.Common;
using System.Security.Permissions;

namespace SlotMachineServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    
    public class User
    {
        private Socket client;
        public bool loggedIn = false;
        private SlotMachineLogic activity;
        private AccountDataObj account;
        public string username;
        public int slot_credits = 0;
        public int vip_credits = 0;
        public string date_created;
        public string last_TOS_date_accepted;
        public User(Socket client)//created on runtime by client connection
        {
            this.client = client;
        }
        public User()//for xml serialization and deserialization
        {

        }
        public void AssignConnectedAccount(AccountDataObj acc)
        {
            slot_credits = acc.slot_credits;
            vip_credits = acc.vip_credits;
            date_created = acc.date_created;
            last_TOS_date_accepted = acc.last_TOS_date_accepted;
            username = acc.username;
            account = acc;
            loggedIn = true;
        }
        public AccountDataObj GetConnectedAccount()
        {
            return account;
        }

        public void AssignGame(SlotMachineLogic game)
        {
            activity = game;
        }
        public SlotMachineLogic.Symbol[] Spin()
        {
            bool win;
            SlotMachineLogic game = GetGame();
            SlotMachineLogic.Symbol[] symbols = new SlotMachineLogic.Symbol[game.numberOfVerticalOutputs * 3];
            symbols = game.spin(out win);
            if( win == true)
            {
                slot_credits += game.payout;
            }
            //MessageBox.Show(symbols[0].texture);
            return symbols;
         }
        public SlotMachineLogic GetGame()
        {
            return activity;
        }
        public Socket GetSocket()
        {
            return client;
        }


    }
    
    public class SlotMachineLogic
    {
        public string gameName = "";
        public float winToLoseRatio = 0.5f;
        public int numberOfVerticalOutputs = 3;
        public int numberOfSymbols = 12;
        public int payout = 3;
        List<Symbol> inplay = new List<Symbol>();
        public class Symbol
        {
            public string texture;
            public bool winningSymbol;
        }

        public SlotMachineLogic(string gameName, string[] Symbol_textures, int numberOfOutputs_row, int numberOfSymbols, float winToLoseRatio,int payout)
        {
            this.gameName = gameName;
            this.numberOfVerticalOutputs = numberOfOutputs_row;
            this.numberOfSymbols = numberOfSymbols;
            this.winToLoseRatio = winToLoseRatio;
            this.payout = payout;

            for (int i = 0; i < numberOfSymbols; i++)
            {
                Symbol cur = new Symbol();
                if (numberOfSymbols*winToLoseRatio <= i)
                {
                    //winning symbols
                    cur.texture = Symbol_textures[0];
                    cur.winningSymbol = true;
                    inplay.Add(cur);
                }
                else
                {
                    //loosing symbols
                    cur.texture = Symbol_textures[1];
                    cur.winningSymbol = false;
                    inplay.Add(cur);
                }
            }
        }
        public Symbol[] spin( out bool isWin)
        {
            /*
             * //0 and 1 represent the diffrent types of symbols
            collection [0,1,0,1,0,1,0,0,1] 

            //Outputs will be printed top to bottom left to right on client
                 ___ ___ ___  
                | 0 | 1 | 0 |
                | 1 | 0 | 0 |
                |_0_|_1_|_1_|
            //checking for win on client would be based on this formula
            int i = 1
            int[] winning row = int[numberofrows]{i,i+=3,i+=3...}
            example: 
            collection 
            if(collection[1].iswining,collection[4].iswining,collection[7].iswining)
             */
            Symbol[] OutputCollection = new Symbol[numberOfVerticalOutputs * 3];

            Random rnd = new Random();
            Symbol[] BlackBox = inplay.ToArray();       
            for (int index = 0; index < OutputCollection.Length;)
            {
                int RND_Selection = rnd.Next(0, BlackBox.Length);
                if (BlackBox[RND_Selection] != null)
                {
                    OutputCollection[index] = BlackBox[RND_Selection];
                    BlackBox[RND_Selection] = null;
                    index++;
                }
            }
            
            int i = 1;
            bool[] winning_row = new bool[numberOfVerticalOutputs];
            for (int index = 0; index < winning_row.Length; index++)
            {
                winning_row[index] = OutputCollection[i].winningSymbol;
                i += 3;
            }
            isWin = true;
            foreach(bool results in winning_row)
            {
                if(results != true)
                {
                    isWin = false;
                    break;
                }
            }
            return OutputCollection;
        }


    }
}
