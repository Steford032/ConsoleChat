using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ConsoleServer
{
    class ServerObject
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
        List<ClientObject> clients = new List<ClientObject>();

        protected internal void RemoveConnection(string id)
        {
            ClientObject? client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null) clients.Remove(client);
            client?.Close();
        }

        //Метод для прослушивания входящих подключений
        protected internal async Task ListenAsync()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Server is started. Waiting for connections...");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    clients.Add(clientObject);
                    Task.Run(clientObject.ProcessAsyns);
                }
                
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        //Метод для трансляции сообщений подключенным объектам
        protected internal async Task SendMessage(string message, string SenderId)
        {
            foreach(var client in clients)
            {
                if(client.Id != SenderId)
                {
                    await client.writer.WriteLineAsync(message);

                    //Асинхронная очистка буфера потока и запись буферизованных даных
                    //на базовое устройство
                    await client.writer.FlushAsync();
                }
            }
        }

        protected internal void Disconnect()
        {
            foreach (var client in clients)
            {
                client.Close();
            }
            tcpListener.Stop();
        }
    }
}
