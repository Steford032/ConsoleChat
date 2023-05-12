using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConsoleServer
{
    class Program
    {

        static async Task Main(string[] args)
        {

            TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888); // сервер для прослушивания
            List<ClientObject> clients = new List<ClientObject>(); // все подключения
           
            ServerObject server = new ServerObject();
            await server.ListenAsync();
        }
    }
}
