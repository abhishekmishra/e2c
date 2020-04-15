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
        bool commandSuccessful = true;
        string commandFailureReason;
        Direction direction = Direction.E;
        Random rnd = new Random();

        public int AgentId { get; set; }
        public Coords SpaceSize { get; set; }

        public SimpleCleaningAgent(Dictionary<string, string> args)
        {
            // do nothing
        }

        public void CommandResult(bool success, string failureReason)
        {
            commandSuccessful = success;
            commandFailureReason = failureReason;
        }

        (int r, int c) NewLocation(Coords l)
        {
            if (direction == Direction.E)
            {
                return (l.Row, l.Column + 1);
            }
            else if (direction == Direction.W)
            {
                return (l.Row, l.Column - 1);
            }
            else if (direction == Direction.N)
            {
                return (l.Row - 1, l.Column);
            }
            else if (direction == Direction.S)
            {
                return (l.Row + 1, l.Column);
            }
            return (l.Row, l.Column);
        }

        public IAgentCommand NextCommand(Coords location, bool isDirty)
        {
            if (isDirty)
            {
                return new CleanCommand(AgentId, location.Row, location.Column);
            }
            else
            {
                if (commandSuccessful)
                {
                    direction = (Direction)rnd.Next(0, 4);
                    (int r, int c) = NewLocation(location);
                    return new MoveToComand(AgentId, r, c);
                }
                else
                {
                    direction = (Direction)rnd.Next(0, 4);
                    (int r, int c) = NewLocation(location);
                    return new MoveToComand(AgentId, r, c);
                }
            }
        }
    }
}
