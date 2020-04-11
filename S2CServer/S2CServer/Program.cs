using System;

namespace S2CServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Created new space to clean -> ");
            var sp = new Space(10, 10, 0.1, 0.3);
            sp.printSpace();
        }
    }
}
