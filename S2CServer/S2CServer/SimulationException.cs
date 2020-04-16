using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public enum SimulationErrorCode
    {
        SIM_ERR_UNKNOWN = 0,
        SIM_ERR_AGENT_DOES_NOT_EXIST,
        SIM_ERR_LOCATION_NOT_ADJACENT,
        SIM_ERR_MOVE_TO_WALL,
        SIM_ERR_AGENT_COLLISION,
        SIM_ERR_LOCATION_NOT_DIRTY,
        SIM_ERR_AGENT_NOT_AT_LOCATION,
        SIM_ERR_NO_SUCH_AGENT,
        SIM_ERR_OUT_OF_RANGE,
        SIM_ERR_SIM_RUNNING
    }

    public class SimulationException : Exception
    {
        public SimulationErrorCode ErrorCode { get; private set; }
        public Coords Location { get; private set; }

        public SimulationException(string msg, SimulationErrorCode errCode, Coords loc) : base(msg)
        {
            ErrorCode = errCode;
            Location = loc;
        }
    }
}
