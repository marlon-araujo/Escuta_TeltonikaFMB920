using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Escuta_Teltonika
{
    public class Receiver
    {

        public bool IsStopRequested { get; set; }
        private int _portToRun;
        public async Task<List<Task<bool>>> StartReceiverAsync(int port)
        {
            return await Task.Factory.StartNew(() =>
            {
                _portToRun = port;
                // use this IP for LAN. for Public IP. We have to choose the Any IP.
                //var selfIp = GetSelfIp();

                List<Task<bool>> taskList = new List<Task<bool>>();
                //  IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                TcpListener listener = new TcpListener(IPAddress.Any, _portToRun);
                listener.Start();
                while (!IsStopRequested)
                {
                    Socket client = listener.AcceptSocket();
                    taskList.Add(ProcessAsync(client));
                }
                return taskList;
            });
        }

        internal async Task<bool> ProcessAsync(Socket clientSocket)
        {
            StreamWriter sr = new StreamWriter("D:\\AVL\\AVL.txt");
            bool result = false;
            //  IMsg msg = null;

            return await Task.Factory.StartNew(() =>
            {
                var ns = new NetworkStream(clientSocket);

                byte[] buffer = new byte[1024];
                int length;
                string total = "";
                while ((length = ns.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string msgFromClient = Encoding.Default.GetString(buffer.Take(length).ToArray()).Trim();

                    byte[] msg = new byte[] { 0x01 };
                    // Send back a response.
                    ns.Write(msg, 0, msg.Length);
                    //if (!string.IsNullOrEmpty(msgFromClient))
                    //{
                    //    msg = Parser.Parser.ParseTheMsg(msgFromClient);
                    //    list.Enqueue(Tuple.Create(msgFromClient, msg));
                    //    WriteRawData();
                    //    sendmsgtocachethroughUPD(msgFromClient, msg);
                    //}
                }
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Disconnect(true);
                // sr.Close();
                return true;
            });
        }
        //private IPAddress GetSelfIp()
        //{
        //    //Task.Delay(10000).Wait();
        //    string hostname = Dns.GetHostName();
        //    IPHostEntry entry = Dns.GetHostEntry(hostname);
        //    var ip = entry.AddressList.First(c => c.AddressFamily == AddressFamily.InterNetwork);
        //    if (ip == null)
        //    {
        //        throw new Exception("No IPv4 address for server");
        //    }
        //    return ip;
        //}

    }
}
