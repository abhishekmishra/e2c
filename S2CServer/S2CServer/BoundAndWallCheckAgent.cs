using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class BoundAndWallCheckAgent : SimpleAgentBase
    {
        private Matrix<double> wallSpace;
        public BoundAndWallCheckAgent(Dictionary<string, string> args) : base(args)
        {
            
        }

        public override void CommandResult(bool success, string failureReason, 
            SimulationErrorCode errCode, Coords loc)
        {
            commandSuccessful = success;
            commandFailureReason = failureReason;
            errorCode = errCode;
            Location = loc;

            if (errCode == SimulationErrorCode.SIM_ERR_MOVE_TO_WALL)
            {
                wallSpace[Location.Row, Location.Column] = 1;
            }
        }


        public override IAgentCommand NextCommand(Coords location, bool isDirty)
        {
            if (wallSpace == null)
            {
                wallSpace = Matrix<Double>.Build.Dense(SpaceSize.Row, SpaceSize.Column);
            }

            if (isDirty)
            {
                return new CleanCommand(AgentId, location.Row, location.Column);
            }
            else
            {
                int r, c;
                do
                {
                    (r, c) = NewLocation(location);
                } while (r < 0 || r >= SpaceSize.Row
                        || c < 0 || c >= SpaceSize.Column
                        || wallSpace[r, c] == 1);
                return new MoveToCommand(AgentId, r, c);
            }
        }
    }
}
