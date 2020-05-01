using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace S2CCore
{
    public class SpiralAgent : SimpleAgentBase
    {
        private readonly List<Direction> Directions
            = new List<Direction>() {
                Direction.N,
                Direction.E,
                Direction.S,
                Direction.W
        };

        private int index = 0;

        private int prevSpiralLeg = 0;

        private int currentSpiralLeg = 0;

        private Direction CurrentDirection()
        {
            return Directions[index];
        }

        // This section of code from
        // https://stackoverflow.com/questions/33781853/circular-lists-in-c-sharp/33782325
        #region
        private Direction NextDirection()
        {
            index++; // increment index
            index %= Directions.Count; // clip index (turns to 0 if index == items.Count)
                                       // as a one-liner:
            /* index = (index + 1) % items.Count; */

            return CurrentDirection();
        }

        private Direction PreviousItem()
        {
            index--; // decrement index
            if (index < 0)
            {
                index = Directions.Count - 1; // clip index (sadly, % cannot be used here, because it is NOT a modulus operator)
            }
            // or above code as a one-liner:
            /* index = (items.Count+index-1)%items.Count; */ // (credits to Matthew Watson)

            return CurrentDirection();
        }
        #endregion


        private Coords NextLocation(Coords l)
        {
            Coords loc = l;
            switch (Directions[index])
            {
                case Direction.N:
                    loc = new Coords(l.Row - 1, l.Column);
                    break;
                case Direction.E:
                    loc = new Coords(l.Row, l.Column + 1);
                    break;
                case Direction.S:
                    loc = new Coords(l.Row + 1, l.Column);
                    break;
                case Direction.W:
                    loc = new Coords(l.Row, l.Column - 1);
                    break;
            }
            if (loc.Row < 0 || loc.Row >= SpaceSize.Row
                        || loc.Column < 0 || loc.Column >= SpaceSize.Column)
            {
                throw new ArgumentOutOfRangeException("Cannot move in this direction");
            }
            return loc;
        }

        public SpiralAgent(Dictionary<string, string> args) : base(args)
        {
        }

        public override IAgentCommand NextCommand(Coords location, bool isDirty)
        {
            if (isDirty)
            {
                return new CleanCommand(AgentId, location.Row, location.Column);
            }

            bool turn = false;
            bool resetSpiral = false;
            if (currentSpiralLeg > prevSpiralLeg)
            {
                turn = true;
            }
            if (commandSuccessful = false && (
                errorCode == SimulationErrorCode.SIM_ERR_MOVE_TO_WALL ||
                errorCode == SimulationErrorCode.SIM_ERR_OUT_OF_RANGE))
            {
                turn = true;
                resetSpiral = true;
            }
            if (turn)
            {
                Turn();
            }
            if (resetSpiral)
            {
                ResetSpiral();
            }

            bool goodLoc = false;
            Coords l = null;
            do
            {
                try
                {
                    l = NextLocation(location);
                    goodLoc = true;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Turn();
                    ResetSpiral();
                }
            } while (!goodLoc);
            currentSpiralLeg += 1;
            return new MoveToCommand(AgentId, l.Row, l.Column);
        }

        private void ResetSpiral()
        {
            prevSpiralLeg = 0;
        }

        private void Turn()
        {
            NextDirection();
            prevSpiralLeg = currentSpiralLeg;
            currentSpiralLeg = 0;
        }
    }
}
