using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace IPCProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Producer...");

            // Create a named pipe for communication
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
            {
                Console.WriteLine("Waiting for Consumer to connect...");
                pipeServer.WaitForConnection(); // Wait for the consumer to connect
                Console.WriteLine("Consumer connected!");

                try
                {
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        writer.AutoFlush = true;

                        // Generate and send data to the consumer
                        for (int i = 0; i < 10; i++)
                        {
                            string message = $"Message {i} from Producer at {DateTime.Now}";
                            Console.WriteLine($"Sending: {message}");
                            writer.WriteLine(message); // Send the message
                            Thread.Sleep(1000); // Simulate some delay
                        }

                        // Send a termination message
                        writer.WriteLine("END");
                        Console.WriteLine("All messages sent. Exiting Producer.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}