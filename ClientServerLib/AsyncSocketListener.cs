using System;
using System.Net;
using System.Net.Sockets;
using System.Text; 

namespace ClientServerLib
{
    public class AsyncSocketListener
    {
        public static ManualResetEvent ManualThreadState = new ManualResetEvent(false);
        public const int LocalEndPoint = 11000;
        public bool ActiveState { get; set; } 
        public Socket Handler { get; private set; }
        public AsyncSocketListener()
        {

        }
        public void BeginListening()
        {
            IPHostEntry iPHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = iPHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                ActiveState = true;  
                while (ActiveState)
                {
                    ManualThreadState.Reset();
                    Console.WriteLine("Waiting for connections...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    ManualThreadState.WaitOne(); 
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
        public void AcceptCallback(IAsyncResult ar)
        {
            ManualThreadState.Set(); 

            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // create the state object 
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize,
                0, new AsyncCallback(ReadCallback), state); 
        }
        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty; 

            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar); 

            if(bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // check for end of file tag
                content = state.sb.ToString(); 
                // true if all content has been read 
                if(content.IndexOf("<EOF>") > -1)
                {
                    // perform action here
                    ParseMessage(content, state, ar); 
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, 
                        new AsyncCallback(ReadCallback), state);
                }
            }
        }
        private static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler); 
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close(); 
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
        private void TurnOff(StateObject state)
        {
            try
            {
                state.workSocket.Shutdown(SocketShutdown.Both);
                state.workSocket.Close();
                ActiveState = false;
                Console.WriteLine("Server terminated."); 
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString()); 
            }
        }
        private void AddNumbers(string[] message)
        {
            try
            {
                // if the add command is received, values will be sent 
                // delimited by a ;
                // First value will be add command
                // subsequent values will be the values to add 
                double result = 0; 

                for(int i = 1; i < message.Length; i++)
                {
                    result += Convert.ToDouble(message[i]); 
                }
                Console.WriteLine(result.ToString());
                Console.WriteLine("Acknowledged?");
                Console.ReadLine(); 
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString()); 
            }
        }
        private void Echo(string[] message)
        {
            try
            {
                for(int i = 1; i < message.Length; i++)
                {
                    Console.WriteLine(message[i]); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString()); 
                throw;
            }
        }
        private void ReturnToSender(StateObject state, string[] message)
        {
            try
            {
                Send(state.workSocket, message[2..^1].ToString());
            }
            catch (Exception e)
            {
                e.ToString(); 
            } 
        }
        private void ParseMessage(string content, StateObject state, IAsyncResult ar)
        {
            // convert content to message enum 
            // step 1: remove <EOF> from end of string 
            string[] message = content.Split(';');
            var messageEnum = Enum.Parse(typeof(Communication.Messages), message[0]);
            switch (messageEnum)
            {
                case Communication.Messages.TurnOff:
                    TurnOff(state);
                    break;
                case Communication.Messages.AddNumbers:
                    AddNumbers(message);
                    break;
                case Communication.Messages.Echo:
                    Echo(message);
                    break;
                case Communication.Messages.ReturnToSender:
                    ReturnToSender(state, message); 
                    break;
                default:
                    break;
            }
        }


    }
}
