using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading;
using System.Text; 

namespace ClientServerLib
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        public Socket workSocket = null; 
    }
}
