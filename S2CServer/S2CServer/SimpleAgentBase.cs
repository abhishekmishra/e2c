using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public abstract class SimpleAgentBase:ICleaningAgent
    {
        protected bool commandSuccessful = true;
        protected string commandFailureReason;
        protected SimulationErrorCode errorCode;
        protected Coords Location;
        protected Direction direction = Direction.E;
        protected Random rnd = new Random();
        Dictionary<string, string> args;

        public SimpleAgentBase(Dictionary<string, string> args)
        {
            this.args = args;
        }

        public int AgentId { get; set; }

        public Coords SpaceSize { get; set; }

        public virtual void CommandResult(bool success, string failureReason, 
            SimulationErrorCode errCode, Coords loc)
        {
            commandSuccessful = success;
            commandFailureReason = failureReason;
            errorCode = errCode;
            Location = loc;
        }

        protected (int r, int c) NewLocation(Coords l)
        {
            //move in a random direction
            direction = (Direction)rnd.Next(0, 4);

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

        public abstract IAgentCommand NextCommand(Coords location, bool isDirty);
    }
}
