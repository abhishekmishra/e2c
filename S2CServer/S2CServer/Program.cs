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
            int agent = 0;
            int r = 0, c = 0;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    agent = sp.initAgent(i, i);
                    r = c = i;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                break;
            }
            sp.printSpace();
            if (agent > 0)
            {
                sp.moveAgent(agent, r, c+1);
            }
            sp.printSpace();
        }
    }
}
