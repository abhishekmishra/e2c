using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;


namespace S2CServer
{
    /*
    * The space to clean is a 2D space at the moment where there are three kinds of areas
    * i. Empty space (Multiples of negative even numbers), -x/2 + 1 indicates number of days this was clean
    * ii. Dirty space (Multiples of positive even numbers), x/2 indicates number of days this was dirty
    * iii. Wall/obstacle indicated by 1
    * iv. The cleaners can be assigned any odd positive or negative number other than 1
    *
    * The space of integers is kept free for new additions to information of the space.
    */

    public class Space
    {
        public static int NODATA = 0;
        public static int WALL = 1;
        public static int DIRTY = 2;
        private Matrix<Double> space;
        private Matrix<Double> agentSpace;
        private int rows, columns;
        private double wallP, dirtyP;

        private Dictionary<int, AgentState> agents;

        public Space(int r, int c, double wallProb = 0.2, double dirtyProb = 0.3)
        {
            rows = r;
            columns = c;
            wallP = wallProb;
            dirtyP = dirtyProb;

            agentSpace = Matrix<Double>.Build.Dense(rows, columns);
            agentSpace.Clear();

            agents = new Dictionary<int, AgentState>();
            createEmptySpace(rows, columns);
            randomWall();
            randomDirty();
        }

        public void createEmptySpace(int rows, int columns)
        {
            space = Matrix<Double>.Build.Dense(rows, columns);
            space.Clear();
        }

        public void randomWall()
        {
            Random rnd = new Random();
            for (int i = 0; i < space.RowCount; i++)
            {
                for (int j = 0; j < space.ColumnCount; j++)
                {
                    double x = rnd.NextDouble();
                    if (x <= wallP)
                    {
                        space[i, j] = WALL;
                    }
                }
            }
        }

        public void randomDirty()
        {
            Random rnd = new Random();
            for (int i = 0; i < space.RowCount; i++)
            {
                for (int j = 0; j < space.ColumnCount; j++)
                {
                    if (space[i, j] != WALL)
                    {
                        double x = rnd.NextDouble();
                        if (x <= dirtyP)
                        {
                            space[i, j] = DIRTY;
                        }
                    }
                }
            }
        }

        public void printSpace()
        {
            for (int i = 0; i < space.RowCount; i++)
            {
                for (int j = 0; j < space.ColumnCount; j++)
                {
                    if (space[i, j] == NODATA)
                    {
                        Console.Write("▢");
                    }
                    if (space[i, j] == WALL)
                    {
                        Console.Write("◉");
                    }
                    if (space[i, j] == DIRTY)
                    {
                        Console.Write("▩");
                    }
                    if (agentSpace[i, j] == NODATA)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write((char)('A' + (agentSpace[i, j] - 2)/2));
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }

        // commands for agents
        public int query(int row, int col)
        {
            return (int)space[row, col];
        }

        public bool isDirty(int row, int col)
        {
            return query(row, col) == DIRTY;
        }

        public bool isClean(int row, int col)
        {
            return !isDirty(row, col);
        }

        public bool canPlaceAgent(int row, int col)
        {
            if (space[row, col] == NODATA || space[row, col] == DIRTY)
            {
                return true;
            }
            return false;
        }

        public int initAgent(int row, int col)
        {
            if (canPlaceAgent(row, col))
            {
                int agentId = -1;
                if (agents.Keys.Count == 0)
                {
                    agentId = 3;
                }
                else
                {
                    agentId = agents.Keys.Max() + 2;
                }
                Console.WriteLine("Added agentId " + agentId);
                agents.Add(agentId, new AgentState(agentId, row, col));
                agentSpace[row, col] = agentId;
                return agentId;
            }
            else
            {
                throw new ArgumentException("Cannot place an agent at the given row column [" + row + ", " + col + "]");
            }
        }

        public bool moveAgent(int agentId, int trow, int tcol)
        {
            AgentState a = agents[agentId];
            if (a != null)
            {
                if (Math.Abs(trow - a.row) < 2 && Math.Abs(tcol - a.col) < 2)
                {
                    if (space[trow, tcol] != WALL)
                    {
                        if (agentSpace[trow, tcol] == NODATA)
                        {
                            agentSpace[a.row, a.col] = NODATA;
                            agentSpace[trow, tcol] = agentId;
                            a.moveTo(trow, tcol);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
