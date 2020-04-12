using System;

namespace S2CServer
{
    class Program
    {
        static void Main(string[] args)
        {

            new SimulationController();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Created new space to clean -> ");
            var sp = new Space(10, 10, 0.0, 0.3);
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
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        sp.moveAgent(agent, r, c + i);
                        bool cleaned = sp.clean(agent, r, c + i);
                        if (cleaned)
                        {
                            Console.WriteLine("Cleaned [" + r + ", " + (c + i) + "]");
                        }
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine();
            sp.printSpace();
        }
    }
}
