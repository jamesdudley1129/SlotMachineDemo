using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
public class Client{

    public static string DNS_IP_of_server = IPAddress.Loopback.ToString();
    public static int listening_port_of_Server = 100;
    
    public static class Buffer{
        public static byte[] buffer = new byte[1024];
        internal static bool isResized = false;
        public static void SetBufferSize(byte[] size){
            isResized = true;
            buffer = size;
        }

        public static void ResetBufferSize(){
            buffer = new byte[1024];
        }
    }
    Socket local  = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
    Socket server;
    IPEndPoint remote_IP = new IPEndPoint(IPAddress.Loopback,listening_port_of_Server);

    public string response = "Error no response....";
    public bool connected = false;
    public bool In_Progress = false;
    public bool Is_ReceaveMSG = false;

    public Program program;
    public Client(Program program){
        this.program = program;
        BeginConnection();
        
    }
    private void BeginConnection(){
        In_Progress = true;
        local = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        local.BeginConnect(remote_IP,new AsyncCallback(ConnectionCallback),local);
        response = "Attempting Connection: Server " + DNS_IP_of_server.ToString(); 
        
    }

    private void ConnectionCallback(IAsyncResult ar){
        try{
            server = (Socket)ar.AsyncState;
            local.EndConnect(ar);
            connected = true;
            response = "Connected to server Socket: " + server.RemoteEndPoint.ToString(); 
            In_Progress = false;
            program.networkStatus = NetStatus.connected;
        }
        catch(Exception x)
       {
            response = x.Message;
            connected = false;
            Is_ReceaveMSG = false;
            In_Progress = false;
       }
        
    }
    public void SendMsg(string msg){
            if(msg == null || msg == ""){
                msg = "ping";
            }
            byte[] data = Encoding.ASCII.GetBytes(msg,0,msg.Length);
            server.BeginSend(data,0,data.Length,SocketFlags.None,new AsyncCallback(SendCallback),server);
        

        
    }

    private void SendCallback(IAsyncResult AR){ 
       try
       {
        server.EndSend(AR);
        response = "sending";
        Is_ReceaveMSG = !Is_ReceaveMSG;
        In_Progress = false;
       }
       catch(Exception x)
       {
        if(x.HResult == -2147467259){
                //server.Close();
                connected = false;
        }
        response = x.Message;
        connected = false;
        Is_ReceaveMSG = false;
        In_Progress = false;
        
       }
        
        
    }
    
    public void ReceaveMSG(){
        local.BeginReceive(Buffer.buffer,0,Buffer.buffer.Length,SocketFlags.None,new AsyncCallback(ReciveCallback),local);
    }

    private void ReciveCallback(IAsyncResult ar){

        try{

            int recived = local.EndReceive(ar);
            byte[] data = new byte[recived];
            Array.Copy(Buffer.buffer, data, recived);
            string text = Encoding.ASCII.GetString(data);
            program.ProcessResponse(text);//takes too long to complete respond ends up sending a...
            //...new message before callback finnished deserializing
            response = "received "+ text;
            Is_ReceaveMSG = !Is_ReceaveMSG;
        }
        catch(Exception x)
        {
            
            if(x.HResult == -2147467259){
                //server.Close();
                connected = false;
            }
            response = "failed to process response... " + x.Message;
            Is_ReceaveMSG = false;
            In_Progress = false;
            
        }

    }
    public void update(string upload){

        
        if(In_Progress == false){
            if(!connected){
                BeginConnection();
            }else{
                if(Is_ReceaveMSG){
                    In_Progress = true;
                    ReceaveMSG();
                }else{
                    In_Progress = true;
                    SendMsg(upload);
                }   
            }
             
        }
    }

/* why when forcing closing works??
    public void Disconnect(){
        In_Progress = true;
        server.BeginDisconnect(true,new AsyncCallback(DisconnectCallback),server);
        response = "Closing Connection: Server " + DNS_IP_of_server.ToString(); 
    }
    private void DisconnectCallback(IAsyncResult ar){
        In_Progress = false;
        server.EndDisconnect(ar);
        connected = false;
    }
*/
}