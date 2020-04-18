using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;


namespace S2CCore
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
        public Matrix<Double> space { get; private set; }
        public Matrix<Double> agentSpace { get; private set; }
        private int rows, columns;
        private double wallP, dirtyP;
        private Random rnd = new Random();

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
            CreateEmptySpace(rows, columns);
            RandomWall();
            RandomDirty();
        }

        public void CreateEmptySpace(int rows, int columns)
        {
            space = Matrix<Double>.Build.Dense(rows, columns);
            space.Clear();
        }

        public void RandomWall()
        {
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

        public void RandomDirty()
        {
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

        // commands for agents
        public int Query(int row, int col)
        {
            return (int)space[row, col];
        }

        public bool IsDirty(int row, int col)
        {
            return Query(row, col) == DIRTY;
        }

        public bool IsWall(int row, int col)
        {
            return Query(row, col) == WALL;
        }

        public bool HasAgent(int row, int col)
        {
            var val = Query(row, col);
            if (val > 2 && val % 2 == 1)
            {
                return true;
            }
            return false;
        }

        public bool IsClean(int row, int col)
        {
            return !IsDirty(row, col);
        }

        public bool CanPlaceAgent(int row, int col)
        {
            if (space[row, col] == NODATA || space[row, col] == DIRTY)
            {
                return true;
            }
            return false;
        }

        public (int row, int col) WhereAmI(int agentId)
        {
            AgentState a = agents[agentId];
            if (a != null)
            {
                return (a.row, a.col);
            }
            return (-1, -1);
        }

        public int InitAgent()
        {
            int countTries = 0;

            while (countTries < 10)
            {
                int row = rnd.Next(0, rows);
                int col = rnd.Next(0, columns);

                if (CanPlaceAgent(row, col))
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
                countTries += 1;
            }
            throw new SimulationException("Cannot place the next agent. No suitable location found.",
                SimulationErrorCode.SIM_ERR_UNKNOWN, null);
        }

        public void MoveAgent(int agentId, int trow, int tcol)
        {
            CheckRange(trow, tcol);
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
                        }
                        else
                        {
                            throw new SimulationException("There is already an agent there!",
                                SimulationErrorCode.SIM_ERR_AGENT_COLLISION, new Coords(trow, tcol));
                        }
                    }
                    else
                    {
                        throw new SimulationException("Cannot move into a WALL!",
                                SimulationErrorCode.SIM_ERR_MOVE_TO_WALL, new Coords(trow, tcol));
                    }
                }
                else
                {
                    throw new SimulationException("New location is not adjacent!",
                                SimulationErrorCode.SIM_ERR_LOCATION_NOT_ADJACENT, new Coords(trow, tcol));
                }
            }
            else
            {
                throw new SimulationException("Agent does not exist!",
                                SimulationErrorCode.SIM_ERR_NO_SUCH_AGENT, new Coords(trow, tcol));
            }
        }

        public void Clean(int agentId, int row, int col)
        {
            CheckRange(row, col);
            var agent = agents[agentId];
            if (agent != null)
            {
                if (agent.row == row && agent.col == col)
                {
                    if (IsDirty(row, col))
                    {
                        space[row, col] = NODATA;
                    }
                    else
                    {
                        throw new SimulationException("Location is not dirty!",
                                SimulationErrorCode.SIM_ERR_LOCATION_NOT_DIRTY, new Coords(row, col));
                    }
                }
                else
                {
                    throw new SimulationException("Agent is not at the location to clean!",
                                SimulationErrorCode.SIM_ERR_AGENT_NOT_AT_LOCATION, new Coords(row, col));
                }
            }
            else
            {
                throw new SimulationException("Agent does not exist!",
                                SimulationErrorCode.SIM_ERR_NO_SUCH_AGENT, new Coords(row, col));
            }
        }

        public int TotalDirty()
        {
            int count = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (space[i, j] == DIRTY)
                    {
                        count += 1;
                    }
                }
            }
            return count;
        }

        public bool HasDirty()
        {
            return TotalDirty() > 0;
        }

        public void CheckRange(int row, int col)
        {
            if (row < 0 || row >= this.rows || col < 0 || col >= this.columns)
            {
                throw new SimulationException("Row/column is out of range: " + CoordsToString(row, col),
                                SimulationErrorCode.SIM_ERR_OUT_OF_RANGE, new Coords(row, col));
            }
        }

        public static String CoordsToString(int row, int col)
        {
            return "[" + row + ", " + col + "]";
        }
    }
}
