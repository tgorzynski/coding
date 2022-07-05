using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;


namespace Server
{
    class Server
    {
        static Socket listenerSocket;
        static List<ClientData> _clients;

        static void Main(string[] args)
        {
            Console.WriteLine("IP Servera :" + Packet.GetIP4Address());
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //automat
            _clients = new List<ClientData>(); //same

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Packet.GetIP4Address()), 8887); // użycie .Net aby utworzyć soket


            listenerSocket.Bind(ip);

            Thread listenThread = new Thread(ListenThread); // wielowatkowość
            listenThread.Start(); // automat

        }


        static void ListenThread()
        {

            for (; ;)
            {

                listenerSocket.Listen(0); // tzw. listener 
                _clients.Add(new ClientData(listenerSocket.Accept())); //akceptacja

            }
        }

        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;

            byte[] Buffer;
            int readBytes;


            for (; ; ) 
            {
                try // rozwiazanie frezzowania sie servera w przypadku rozlaczenia klienta
                {
                    Buffer = new byte[clientSocket.SendBufferSize];

                    readBytes = clientSocket.Receive(Buffer); // informacje o wielkosci

                    if (readBytes > 0)
                    {
                        Packet packet = new Packet(Buffer);
                        DataManager(packet);

                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Klient rozłączył się...");
                    Console.ReadLine();
                    Environment.Exit(0);    
                    
                }
            }
        }

        // datamanager

         public static void DataManager(Packet p)
        {
               switch(p.packetType)
            { 

                case PacketType.Chat:
                    foreach(ClientData c in _clients)
                    {
                        c.clientSocket.Send(p.ToBytes());
                    }    
                    break;
            }
        }


    }

    class ClientData
    {
        public Socket clientSocket;
        public Thread clientThread;
        public string id; // identyfikacja klienta


        public ClientData()
        {
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();   
        }
        public ClientData(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            Packet p = new Packet(PacketType.Registration, "serwer");
            p.Gdata.Add(id);
            clientSocket.Send(p.ToBytes());
        }
    }
}