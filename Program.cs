using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace c_sharp_async_intro
{
    class Program
    {
        static void Main(string[] args)
        {
            TAPAsync();
            EAP();
            
            Console.ReadLine();
        }

        static void EAP()
        {
            var handler = new ExampleHandler();

            handler.OnTriggerCompleted += (sender, e) =>
            {
                Console.WriteLine($"Triggered at: { DateTime.Now.ToLongTimeString()}");
            };

            handler.Start(3000);

            Console.WriteLine($"Start waiting at {DateTime.Now.ToLongTimeString()}");
            Console.WriteLine($"Processing...");
        }

        static async void TAPAsync()
        {
            string filePath = "bigFile.txt";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Create a big file
            FileStream fs = new FileStream(filePath, FileMode.CreateNew);
            fs.Seek(1024 * 1024, SeekOrigin.Begin);
            fs.WriteByte(0);
            fs.Close();

            var task = ReadFileAsync(filePath);

            Console.WriteLine("A synchronous message");

            int length = await task;

            Console.WriteLine("Total file length: " + length);
            Console.WriteLine("After reading message");
        }

        static async Task<int> ReadFileAsync(string file)
        {
            Console.WriteLine("Start reading file");

            int length = 0;

            using(StreamReader reader = new StreamReader(file))
            {
                string fileContent = await reader.ReadToEndAsync();
                length = fileContent.Length;
            }

            Console.WriteLine("Finished reading file");

            return length;
        }
    }

    public class ExampleHandler
    {
        public event EventHandler OnTriggerCompleted;

        public void Start(int timeout)
        {
            var timer = new Timer(new TimerCallback((state) =>
            {
                OnTriggerCompleted?.Invoke(null, null);
            }));

            timer.Change(timeout, 0);
        }
    }
}
