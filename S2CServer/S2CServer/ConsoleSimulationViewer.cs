using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace S2CCore
{
    public class ConsoleSimulationViewer : ISimulationViewer
    {
        public void CommandExecuted(int agentId, string command)
        {
            Console.WriteLine("Agent {0} command {1} done.", agentId, command);
        }

        public void CommandFailed(int agentId, string command, string reason)
        {
            Console.WriteLine("Agent {0} command {1} failed: {2}", agentId, command, reason);
        }

        public void NewRound(int roundNum)
        {
            Console.Clear();
            Console.WriteLine("Round #" + roundNum);
        }

        public void ShowMessage(string msg)
        {
            throw new NotImplementedException();
        }

        public void ShowState(Matrix<double> space, Matrix<double> agentSpace)
        {
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
    }
}
