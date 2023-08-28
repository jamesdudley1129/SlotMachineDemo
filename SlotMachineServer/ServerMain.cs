using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Windows.Forms;

namespace SlotMachineServer
{
    public class ServerMain
    {
        public static byte[] tempbuffer = new byte[1024];  
        private static Socket server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        private static List<Socket> clients= new List<Socket>();
        public static NetworkLogic NetLogic;

        public ServerMain()
        {
            awaiting_connection(server);
            NetLogic = new NetworkLogic(this);
        }
        public void awaiting_connection(Socket server)
        {
            //the ipaddress and port the server is listening on for connection request
            server.Bind(new IPEndPoint(IPAddress.Any, 100));
            //how many connection request can be pending before server starts declining connections
            server.Listen(0);
            //(starts a new thread?) calls AcceptConnection when a connection request is recived
            server.BeginAccept(new AsyncCallback(AcceptConnection),null);

        }
        private static void AcceptConnection(IAsyncResult connector)
        {
            Socket client = server.EndAccept(connector);
            clients.Add(client);
            Logger.Log("Client Connected: Socket " + client.RemoteEndPoint.ToString(),LoggingLevel.LiteEvents);
            
            //tells the local client socket to start listening for transmissions calls from its remote self
            //calls ReciveCallback when transmission is recived
            client.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), client);
            //sets the server to listen for more connections
            server.BeginAccept(new AsyncCallback(AcceptConnection), null);
            
        }
        private static void ReciveCallback(IAsyncResult transmision)
        {
            Socket socket = (Socket)transmision.AsyncState;
            try
            {
                int recived = socket.EndReceive(transmision);
                byte[] data = new byte[recived];
                Array.Copy(tempbuffer, data, recived);
                string text = Encoding.ASCII.GetString(data);

                Logger.Log("Receaving:" + text + ": Socket " + socket.RemoteEndPoint.ToString(), LoggingLevel.Events);

                if (text.ToLower().StartsWith("ping"))
                {
                    byte[] sendData = Encoding.ASCII.GetBytes("ping");
                    socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
                else
                {
                    NetLogic.AddToCommandQueue(socket, text);
                }
            }
            catch (Exception x)
            {
                if (x.HResult == -2147467259)//exception result for a forced closed connection
                {
                    //removes socket from clients
                    clients.Remove(socket);
                }
                Logger.Log("Error:" + x.Message + " | Error ID: "+ x.HResult.ToString() + " | onSocket: " + socket.RemoteEndPoint.ToString(), LoggingLevel.Errors); 
            }
   

        }
        private static void SendCallback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndSend(result);
            socket.BeginReceive(tempbuffer,0,tempbuffer.Length,SocketFlags.None, new AsyncCallback(ReciveCallback),socket);
            Logger.Log("Sending: Socket " + socket.RemoteEndPoint.ToString(), LoggingLevel.Events);
        }
        public void SendResponse(Socket client,string response)
        {
            byte[] sendData = Encoding.ASCII.GetBytes(response);
            client.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
        }
    }
}
