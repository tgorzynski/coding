using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace Client
{
    class Client
    {
        public static Socket master;
        public static string name;
        public static string id;

        static void Main(string[] args)
        {


          
                Console.WriteLine("Wpisz swoje imie");
                name = Console.ReadLine();





        // Console.WriteLine("Wpisz swoje imie");
        //    name = Console.ReadLine();

        X: Console.Clear();
           Console.WriteLine("Podaj IP serwera: "); // wprowadzenie odpowiedniego numeru IP
           string ip = Console.ReadLine();

             master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

             IPEndPoint ip2 = new IPEndPoint(IPAddress.Parse(ip), 8887);
            
            try
            {
                master.Connect(ip2);                        // sprawdzenie polaczenia
            }
            catch
            {
                Console.WriteLine("Serwer Status: Offline. ");
                Console.WriteLine("Zamykanie, ponawiam połączenie...");
                Thread.Sleep(5000); 
                goto X;
            }

            Thread t = new Thread(Data_IN);
            t.Start();

            for(; ; )
            {
                Console.Write(" *** ");
                string input = Console.ReadLine();
               

                Packet p = new Packet(PacketType.Chat, id);
                p.Gdata.Add(name);
                p.Gdata.Add(input);
                master.Send(p.ToBytes());
            }
        }

        static void Data_IN()
        {
            byte[] Buffer;
            int readBytes;

            for (; ; )
            {
                try  
                {
                    Buffer = new byte[master.SendBufferSize];
                    readBytes = master.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        DataManager(new Packet(Buffer));
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Utracono połączenie z serwerem...");
                    Console.ReadLine ();
                    
                }
            }
        }


        static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Registration:
                    id = p.Gdata[0];
                    break;

                case PacketType.Chat:
                   // ConsoleColor c = Console.ForegroundColor;
                  // Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(p.Gdata[0]+ ": " + p.Gdata[1]);
                  //  Console.ForegroundColor = c;
                    break;
            }
        }

    }
}