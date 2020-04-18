using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace S2CCore
{
    public class ConsoleSimulationViewer : ISimulationViewer
    {
        public void SimAborted()
        {
            Console.WriteLine("Simulation Aborted");
        }

        public void SimStarted(int simNum, string name)
        {
            Console.WriteLine("Simulation Started");
        }

        public void SimComplete()
        {
            Console.WriteLine("Simulation Complete");
        }

        public void ShowState(int simRound, List<IAgentCommand> commands,
            Matrix<Double> space, Matrix<Double> agentSpace)
        {
            Console.Clear();
            Console.WriteLine("Round #" + simRound);

            foreach (var command in commands)
            {
                if(command.Status)
                {
                    Console.WriteLine("Agent {0} command {1} done.", command.AgentId, command);
                }
                else
                {
                    Console.WriteLine("Agent {0} command {1} failed: {2}", command.AgentId, command, command.FailureReason);
                }
            }

            for (int i = 0; i < space.RowCount; i++)
            {
                for (int j = 0; j < space.ColumnCount; j++)
                {
                    if (space[i, j] == Space.NODATA)
                    {
                        Console.Write("▢");
                    }
                    if (space[i, j] == Space.WALL)
                    {
                        Console.Write("◉");
                    }
                    if (space[i, j] == Space.DIRTY)
                    {
                        Console.Write("▩");
                    }
                    if (agentSpace[i, j] == Space.NODATA)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write((char)('A' + (agentSpace[i, j] - 2) / 2));
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
            Thread.Sleep(50);
        }

        public void ShowMessage(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
