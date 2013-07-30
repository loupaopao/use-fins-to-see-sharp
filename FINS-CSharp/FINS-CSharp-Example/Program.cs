using FINS_CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FINS_CSharp_Example
{
    class Program
    {
        public Program()
        {
            // We are here (static IP)
            FINS.IsAt("172.25.1.6");

            // We are talking to the PLC (static IP)
            FINS.TalksTo("172.25.1.5");

            // Create a client
            Client client = new Client(9600);

            // Setup receiver
            client.MessageReceived += client_MessageReceived;

            // Write to PLC
            client.Prepare().WriteTo(FINS.MemoryType.E).From(0).Data("HELLO!").Finish();

            // Read from PLC
            client.Prepare().Read(FINS.MemoryType.E).From(0).Limit(200).Finish();

            // We're stuck in a loop, reading forever, but that's fine - we'll close the client.

            client.Close();
        }

        static void Main(string[] args)
        {
            new Program();
        }

        void client_MessageReceived(byte[] message)
        {
            Console.WriteLine("Received: ");
            for (int i = 0; i < message.Length; i++)
            {
                Console.Write(message[i] + " ");
            }
            Console.WriteLine();
        }
    }
}
