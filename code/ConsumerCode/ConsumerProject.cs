using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace ConsumerProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Consumer...");

            // Connect to the named pipe created by the producer
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.In))
            {
                Console.WriteLine("Connecting to Producer...");
                pipeClient.Connect(); // Connect to the producer
                Console.WriteLine("Connected to Producer!");

                try
                {
                    using (StreamReader reader = new StreamReader(pipeClient))
                    {
                        string message;
                        while ((message = reader.ReadLine()) != null) // Read messages from the pipe
                        {
                            if (message == "END")
                            {
                                Console.WriteLine("Received termination signal. Exiting Consumer.");
                                break;
                            }

                            Console.WriteLine($"Received: {message}");
                        }
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