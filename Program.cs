using System;

namespace kate
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("KATE is starting...");
            new Game("127.0.0.1", 5555);           
            Console.ReadLine();
        }
    }
}
