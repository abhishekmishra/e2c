using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public enum Direction
    {
        N = 0, E, W, S
    }

    public class SimpleCleaningAgent : ICleaningAgent
    {
        int row, col;
        bool isDirty = false;

        // IAgentCommand lastCommand;
        bool commandSuccessful = true;
        string commandFailureReason;
        Direction direction = Direction.E;
        Random rnd = new Random();

        public int id { get; set; }

        public SimpleCleaningAgent()
        {
            // do nothing
        }

        public void CommandResult(bool success, string failureReason)
        {
            commandSuccessful = success;
            commandFailureReason = failureReason;
        }

        (int r, int c) NewLocation()
        {
            if (direction == Direction.E)
            {
                return (row, col + 1);
            }
            else if (direction == Direction.W)
            {
                return (row, col - 1);
            }
            else if (direction == Direction.N)
            {
                return (row - 1, col);
            }
            else if (direction == Direction.S)
            {
                return (row + 1, col);
            }
            return (row, col);
        }

        public IAgentCommand GetCommand()
        {
            if(isDirty)
            {
                return new CleanCommand(id, row, col);
            } else
            {
                if(commandSuccessful)
                {
                    direction = (Direction)rnd.Next(0, 4);
                    (int r, int c) = NewLocation();
                    return new MoveToComand(id, r, c);
                } 
                else
                {
                    direction = (Direction)rnd.Next(0, 4);
                    (int r, int c) = NewLocation();
                    return new MoveToComand(id, r, c);
                }
            }
        }

        public void SetLocation(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public void SetLocationDirty(bool isDirty)
        {
            this.isDirty = isDirty;
        }
    }
}
