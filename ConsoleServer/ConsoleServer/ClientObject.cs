using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ConsoleServer
{
    class ClientObject
    {
        protected internal string Id = Guid.NewGuid().ToString();
        protected internal StreamReader reader { get; }
        protected internal StreamWriter writer { get; }

        TcpClient client;
        ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            var stream = client.GetStream();

            reader = new StreamReader(stream); //Создание потока для получения данных

            writer = new StreamWriter(stream); //Создание потока для отправки данных
        }

        public async Task ProcessAsyns()
        {
            try
            {
                string? userName = await reader.ReadLineAsync();
                string? message = $"{userName} joined the chat";

                //Отправка сообщения всем {Field: message} о присоединение пользоваталем чата 
                await server.SendMessage(message, Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = await reader.ReadLineAsync();
                        if (message == null) continue;
                        message = $"{userName}: {message}";
                        Console.WriteLine(message);
                        await server.SendMessage(message, Id);
                    }
                    catch
                    {
                        message = $"User '{userName}' leaved the chat";
                        Console.WriteLine(message);
                        await server.SendMessage(message, Id);
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            finally
            {
                server.RemoveConnection(Id);
            }
        }

        protected internal void Close()
        {
            writer.Close();
            reader.Close();
            client.Close();
        }
    }
}
