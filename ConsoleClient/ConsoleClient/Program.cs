using System;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleClient
{

    class Program
    {
    
        static async Task Main(string[] args)
        {
            
            string host = "localhost";
            int port = 8888;
            TcpClient client = new TcpClient();
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine($"Welcome to che chat, {name}");

            StreamWriter writer = null;
            StreamReader reader = null;

            try
            {
                client.Connect(host, port);
                reader = new StreamReader(client.GetStream());
                writer = new StreamWriter(client.GetStream());
                if (writer is null || reader is null) return;
                Task.Run(() => ReceiveMessageAsync(reader));

                await SendMessageAsync(writer, name);

            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            writer?.Close();
            reader?.Close();
        }

        static async Task SendMessageAsync(StreamWriter writer, string name)
        {
            //Procedure for sending messages

            await writer.WriteLineAsync(name);
            await writer.FlushAsync();
            Console.WriteLine("Press 'Enter' for sending a message");
            while (true)
            {
                string message = Console.ReadLine().Trim();
                if (message == "")
                    continue;
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
        }

        static async Task ReceiveMessageAsync(StreamReader reader)
        {
            //Procedure for receiving messages
            while (true)
            {
                try
                {
                    string? message = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(message)) continue;
                    Print(message);
                }
                catch
                {
                    break;
                }
            }
        }

        static void Print(string message)
        {
            if (OperatingSystem.IsWindows())
            {
                var position = Console.GetCursorPosition();
                int left = position.Left;
                int top = position.Top;
                //Copying already printed symbols to the next line
                Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
                //Placing cursor int the beggining of line
                Console.SetCursorPosition(0, top);
                Console.WriteLine(message);
                //Moving cursor to the next line
                Console.SetCursorPosition(left, top + 1);
            }
            else Console.WriteLine(message);

        }
    }
}
