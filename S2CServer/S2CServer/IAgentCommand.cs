using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public interface IAgentCommand
    {
        string GetName();

        (int row, int col) GetLocation();
    }
}
