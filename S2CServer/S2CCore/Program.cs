using System;

namespace S2CCore
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var sc = new Simulation();
            sc.AddView(new ConsoleSimulationViewer());
            sc.RunForeground();
        }
    }
}
