using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FINS_CSharp
{
    public class Client
    {
        private UdpClient client;

        public delegate void onReceiveMessage(byte[] message);
        public event onReceiveMessage MessageReceived;

        public Client(int port)
        {
            client = new UdpClient(port);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(FINS.remoteIp), port);
            client.Connect(endPoint);
            Thread receivingThread = new Thread(new ThreadStart(MessageReceivingThread));
            receivingThread.Start();
        }

        public Client()
        {
            client = new UdpClient(9600);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(FINS.remoteIp), 9600);
            client.Connect(endPoint);
            Thread receivingThread = new Thread(new ThreadStart(MessageReceivingThread));
            receivingThread.Start();
        }

        private void MessageReceivingThread()
        {
            while (true)
            {
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 9599);
                byte[] contents = client.Receive(ref remote);

                if (contents.Length > 0)
                {
                    string messageAsBytes = BitConverter.ToString(contents);
                    MessageReceived(contents);
                }
            }
        }

        public FINS Prepare()
        {
            return new FINS(this).Create();
        }

        public void Send(byte[] i)
        {
            client.Send(i, i.Length);
        }

        public void Send(int[] i)
        {
            byte[] result = new byte[i.Length * sizeof(int)];
            Buffer.BlockCopy(i, 0, result, 0, result.Length);
            client.Send(result, result.Length);
        }

        public void Send(float[] i)
        {
            byte[] result = new byte[i.Length * sizeof(float)];
            Buffer.BlockCopy(i, 0, result, 0, result.Length);
            client.Send(result, result.Length);
        }

        public void Send(char[] i)
        {
            byte[] result = new byte[i.Length * sizeof(char)];
            Buffer.BlockCopy(i, 0, result, 0, result.Length);
            client.Send(result, result.Length);
        }

        public void Send(string i)
        {
            byte[] result = new byte[i.Length * sizeof(char)];
            Buffer.BlockCopy(i.ToCharArray(), 0, result, 0, result.Length);
            client.Send(result, result.Length);
        }
    }
}
